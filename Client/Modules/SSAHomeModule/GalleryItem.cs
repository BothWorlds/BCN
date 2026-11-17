using System.Collections.Generic;

namespace BCN.SSAHomeModule;

public class GalleryItem
{
    public string ImageUrl { get; set; } = "";
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public string AltText { get; set; } = "";
}

public class GalleryUploadResult
{
    public string Url { get; set; } = "";
}

public class GallerySettingsModel
{
    public string Heading { get; set; } = "";
    public string Intro { get; set; } = "";
    public List<GalleryItem> Items { get; set; } = new();
    public string ReviewRequestUrl { get; set; } = "";
    public List<ReviewItem> Reviews { get; set; } = new();
}

public class ReviewItem
{
    public string Quote { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string ProjectType { get; set; } = "";
}
