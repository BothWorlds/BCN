using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Infrastructure;
using Oqtane.Shared;

namespace BCN.Server.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class InquiryUploadController : ModuleControllerBase
{
    private const long MaxFileBytes = 12 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".pdf"
    };
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "New", "In progress", "Quoted", "Completed", "Archived", "Spam"
    };

    private readonly IWebHostEnvironment _environment;

    public InquiryUploadController(
        ILogManager logger,
        IHttpContextAccessor accessor,
        IWebHostEnvironment environment) : base(logger, accessor)
    {
        _environment = environment;
    }

    [HttpPost]
    [AllowAnonymous]
    [RequestSizeLimit(MaxFileBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileBytes + 1024 * 1024)]
    public async Task<IActionResult> Post([FromForm] IFormFile file, [FromForm] string requestId)
    {
        if (!Guid.TryParseExact(requestId, "N", out _)) return BadRequest("The upload session is invalid. Please refresh the page and try again.");
        if (file == null || file.Length == 0) return BadRequest("Choose a file to upload.");
        if (file.Length > MaxFileBytes) return BadRequest("Each file must be 12 MB or smaller.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) return BadRequest("Use a JPG, PNG, WebP, or PDF file.");

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var relativeFolder = Path.Combine("uploads", "inquiries", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"), requestId);
        var uploadFolder = Path.GetFullPath(Path.Combine(webRoot, relativeFolder));
        var expectedRoot = Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar;
        if (!uploadFolder.StartsWith(expectedRoot, StringComparison.OrdinalIgnoreCase)) return BadRequest();

        Directory.CreateDirectory(uploadFolder);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var targetPath = Path.Combine(uploadFolder, storedName);
        await using (var stream = System.IO.File.Create(targetPath))
        {
            await file.CopyToAsync(stream);
        }

        var url = "/" + Path.Combine(relativeFolder, storedName).Replace('\\', '/');
        var originalName = Path.GetFileName(file.FileName).Replace("\r", "").Replace("\n", "");
        return Ok(new { url, originalName });
    }

    [HttpPost("submit/{moduleId:int}")]
    [AllowAnonymous]
    [RequestSizeLimit(256 * 1024)]
    public async Task<IActionResult> Submit(int moduleId, [FromBody] InquirySubmissionModel submission)
    {
        if (moduleId <= 0) return BadRequest("The contact form is not configured correctly.");
        if (submission == null) return BadRequest("The request could not be read.");
        if (!TryValidateModel(submission)) return ValidationProblem(ModelState);

        var now = DateTime.UtcNow;
        submission.Id = Guid.NewGuid().ToString("N");
        submission.ReferenceNumber = $"SSA-{now:yyyyMMdd}-{submission.Id[..6].ToUpperInvariant()}";
        submission.SubmittedUtc = now;
        submission.ModuleId = moduleId;
        submission.ContactName = submission.ContactName.Trim();
        submission.Company = (submission.Company ?? string.Empty).Trim();
        submission.EmailAddress = submission.EmailAddress.Trim();
        submission.PhoneNumber = submission.PhoneNumber.Trim();
        submission.ProjectType = submission.ProjectType.Trim();
        submission.ProjectLocation = (submission.ProjectLocation ?? string.Empty).Trim();
        submission.ApproximateSize = submission.ApproximateSize.Trim();
        submission.Colors = submission.Colors.Trim();
        submission.Timeline = submission.Timeline.Trim();
        submission.Message = (submission.Message ?? string.Empty).Trim();
        submission.UploadedFiles = (submission.UploadedFiles ?? new List<InquiryFileModel>())
            .Where(file => file != null &&
                !string.IsNullOrWhiteSpace(file.Url) &&
                file.Url.StartsWith("/uploads/inquiries/", StringComparison.OrdinalIgnoreCase))
            .Select(file => new InquiryFileModel
            {
                Url = file.Url.Trim(),
                OriginalName = Path.GetFileName(file.OriginalName ?? string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
            })
            .Take(5)
            .ToList();

        var folder = GetSubmissionDataFolder(moduleId, true);
        var targetPath = Path.Combine(folder, $"{now:yyyyMMddHHmmssfff}-{submission.Id}.json");
        var temporaryPath = targetPath + ".tmp";
        await using (var stream = System.IO.File.Create(temporaryPath))
        {
            await JsonSerializer.SerializeAsync(stream, submission, JsonOptions);
        }
        System.IO.File.Move(temporaryPath, targetPath, true);

        return Ok(new { submission.ReferenceNumber, submission.SubmittedUtc });
    }

    [HttpGet("submissions/{moduleId:int}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> GetSubmissions(int moduleId)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId)) return Forbid();

        var folder = GetSubmissionDataFolder(moduleId, false);
        if (!Directory.Exists(folder)) return Ok(Array.Empty<InquirySubmissionModel>());

        var submissions = new List<InquirySubmissionModel>();
        foreach (var path in Directory.EnumerateFiles(folder, "*.json")
            .OrderByDescending(Path.GetFileName)
            .Take(200))
        {
            try
            {
                await using var stream = System.IO.File.OpenRead(path);
                var submission = await JsonSerializer.DeserializeAsync<InquirySubmissionModel>(stream, JsonOptions);
                if (submission != null) submissions.Add(submission);
            }
            catch (JsonException)
            {
                // A damaged record should not prevent staff from viewing the other requests.
            }
        }

        return Ok(submissions.OrderByDescending(item => item.SubmittedUtc));
    }

    [HttpPost("submissions/{moduleId:int}/{id}/status")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> UpdateStatus(int moduleId, string id, [FromBody] InquiryStatusModel update)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId)) return Forbid();
        if (!Guid.TryParseExact(id, "N", out _)) return BadRequest("The request ID is invalid.");
        if (update == null || !AllowedStatuses.Contains(update.Status ?? string.Empty)) return BadRequest("Choose a valid request status.");

        var path = FindSubmissionPath(moduleId, id);
        if (path == null) return NotFound("The request could not be found.");

        InquirySubmissionModel submission;
        await using (var stream = System.IO.File.OpenRead(path))
        {
            submission = await JsonSerializer.DeserializeAsync<InquirySubmissionModel>(stream, JsonOptions);
        }
        if (submission == null) return NotFound("The request could not be read.");

        submission.Status = AllowedStatuses.First(status => status.Equals(update.Status, StringComparison.OrdinalIgnoreCase));
        submission.UpdatedUtc = DateTime.UtcNow;
        await WriteSubmission(path, submission);
        return Ok(submission);
    }

    [HttpDelete("submissions/{moduleId:int}/{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> DeleteSubmission(int moduleId, string id)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId)) return Forbid();
        if (!Guid.TryParseExact(id, "N", out _)) return BadRequest("The request ID is invalid.");

        var path = FindSubmissionPath(moduleId, id);
        if (path == null) return NotFound("The request could not be found.");

        InquirySubmissionModel submission = null;
        try
        {
            await using var stream = System.IO.File.OpenRead(path);
            submission = await JsonSerializer.DeserializeAsync<InquirySubmissionModel>(stream, JsonOptions);
        }
        catch (JsonException)
        {
            // The record can still be removed even if its JSON is damaged.
        }

        if (submission != null && !string.Equals(submission.Status, "Archived", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(submission.Status, "Spam", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Archive the request or mark it as spam before deleting it permanently.");
        }

        System.IO.File.Delete(path);
        if (submission != null) DeleteUploadedFiles(submission.UploadedFiles);
        return NoContent();
    }

    private string FindSubmissionPath(int moduleId, string id)
    {
        var folder = GetSubmissionDataFolder(moduleId, false);
        if (!Directory.Exists(folder)) return null;
        return Directory.EnumerateFiles(folder, $"*-{id}.json").SingleOrDefault();
    }

    private static async Task WriteSubmission(string path, InquirySubmissionModel submission)
    {
        var temporaryPath = path + ".tmp";
        await using (var stream = System.IO.File.Create(temporaryPath))
        {
            await JsonSerializer.SerializeAsync(stream, submission, JsonOptions);
        }
        System.IO.File.Move(temporaryPath, path, true);
    }

    private void DeleteUploadedFiles(IEnumerable<InquiryFileModel> files)
    {
        var webRoot = Path.GetFullPath(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"));
        var uploadRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads", "inquiries")) + Path.DirectorySeparatorChar;
        foreach (var file in files ?? Array.Empty<InquiryFileModel>())
        {
            if (string.IsNullOrWhiteSpace(file.Url)) continue;
            var relativePath = file.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var path = Path.GetFullPath(Path.Combine(webRoot, relativePath));
            if (!path.StartsWith(uploadRoot, StringComparison.OrdinalIgnoreCase)) continue;
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder) && !Directory.EnumerateFileSystemEntries(folder).Any()) Directory.Delete(folder);
        }
    }

    private string GetSubmissionDataFolder(int moduleId, bool createFolder)
    {
        var folder = Path.GetFullPath(Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "BCN",
            "inquiries",
            moduleId.ToString()));
        var expectedRoot = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data")) + Path.DirectorySeparatorChar;
        if (!folder.StartsWith(expectedRoot, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Invalid inquiry location.");
        if (createFolder) Directory.CreateDirectory(folder);
        return folder;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public class InquirySubmissionModel
    {
        public string Id { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime SubmittedUtc { get; set; }
        public int ModuleId { get; set; }
        public string Status { get; set; } = "New";
        public DateTime? UpdatedUtc { get; set; }

        [Required, StringLength(50)]
        public string ContactName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required, Phone, StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ProjectType { get; set; } = string.Empty;

        [StringLength(150)]
        public string ProjectLocation { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ApproximateSize { get; set; } = string.Empty;

        [Range(1, 9999)]
        public int Quantity { get; set; }

        [Required, StringLength(250)]
        public string Colors { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Timeline { get; set; } = string.Empty;

        [StringLength(2500)]
        public string Message { get; set; } = string.Empty;

        public List<InquiryFileModel> UploadedFiles { get; set; } = new();
    }

    public class InquiryStatusModel
    {
        public string Status { get; set; } = string.Empty;
    }

    public class InquiryFileModel
    {
        public string Url { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
    }
}
