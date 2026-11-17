using System;
using System.Collections.Generic;
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
public class GalleryUploadController : ModuleControllerBase
{
    private const long MaxImageBytes = 12 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly IWebHostEnvironment _environment;

    public GalleryUploadController(
        ILogManager logger,
        IHttpContextAccessor accessor,
        IWebHostEnvironment environment) : base(logger, accessor)
    {
        _environment = environment;
    }

    [HttpGet("{moduleId:int}")]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Get(int moduleId)
    {
        var path = GetGalleryDataPath(moduleId, false);
        if (!System.IO.File.Exists(path)) return NotFound();

        await using var stream = System.IO.File.OpenRead(path);
        var gallery = await JsonSerializer.DeserializeAsync<GallerySettingsModel>(stream, JsonOptions);
        return gallery == null ? NotFound() : Ok(gallery);
    }

    [HttpPut("{moduleId:int}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> Put(int moduleId, [FromBody] GallerySettingsModel gallery)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId)) return Forbid();
        if (gallery == null) return BadRequest();

        gallery.Heading = (gallery.Heading ?? string.Empty).Trim();
        gallery.Intro = (gallery.Intro ?? string.Empty).Trim();
        gallery.Items = (gallery.Items ?? new List<GalleryItemModel>())
            .Where(item => !string.IsNullOrWhiteSpace(item.ImageUrl))
            .Take(9)
            .ToList();
        gallery.ReviewRequestUrl = NormalizeHttpUrl(gallery.ReviewRequestUrl);
        gallery.Reviews = (gallery.Reviews ?? new List<ReviewItemModel>())
            .Where(review => !string.IsNullOrWhiteSpace(review.Quote) && !string.IsNullOrWhiteSpace(review.CustomerName))
            .Select(review => new ReviewItemModel
            {
                Quote = review.Quote.Trim(),
                CustomerName = review.CustomerName.Trim(),
                ProjectType = (review.ProjectType ?? string.Empty).Trim()
            })
            .Take(8)
            .ToList();

        var path = GetGalleryDataPath(moduleId, true);
        var temporaryPath = path + ".tmp";
        await using (var stream = System.IO.File.Create(temporaryPath))
        {
            await JsonSerializer.SerializeAsync(stream, gallery, JsonOptions);
        }
        System.IO.File.Move(temporaryPath, path, true);
        return Ok(gallery);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    [RequestSizeLimit(MaxImageBytes + 1024 * 1024)]
    public async Task<IActionResult> Post([FromForm] IFormFile file, [FromForm] int moduleId)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId)) return Forbid();
        if (file == null || file.Length == 0) return BadRequest("Choose a photo to upload.");
        if (file.Length > MaxImageBytes) return BadRequest("Photos must be 12 MB or smaller.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) return BadRequest("Use a JPG, PNG, or WebP image.");

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var relativeFolder = Path.Combine("uploads", "gallery", moduleId.ToString());
        var uploadFolder = Path.GetFullPath(Path.Combine(webRoot, relativeFolder));
        var expectedRoot = Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar;
        if (!uploadFolder.StartsWith(expectedRoot, StringComparison.OrdinalIgnoreCase)) return BadRequest();

        Directory.CreateDirectory(uploadFolder);
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}{extension}";
        var targetPath = Path.Combine(uploadFolder, fileName);

        await using (var stream = System.IO.File.Create(targetPath))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/gallery/{moduleId}/{fileName}";
        return Ok(new { url });
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private string GetGalleryDataPath(int moduleId, bool createFolder)
    {
        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var folder = Path.GetFullPath(Path.Combine(webRoot, "uploads", "gallery", moduleId.ToString()));
        var expectedRoot = Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar;
        if (!folder.StartsWith(expectedRoot, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Invalid gallery location.");
        if (createFolder) Directory.CreateDirectory(folder);
        return Path.Combine(folder, "gallery.json");
    }

    private static string NormalizeHttpUrl(string value)
    {
        value = (value ?? string.Empty).Trim();
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)
            ? uri.AbsoluteUri
            : string.Empty;
    }

    public class GallerySettingsModel
    {
        public string Heading { get; set; } = string.Empty;
        public string Intro { get; set; } = string.Empty;
        public List<GalleryItemModel> Items { get; set; } = new();
        public string ReviewRequestUrl { get; set; } = string.Empty;
        public List<ReviewItemModel> Reviews { get; set; } = new();
    }

    public class GalleryItemModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }

    public class ReviewItemModel
    {
        public string Quote { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ProjectType { get; set; } = string.Empty;
    }
}
