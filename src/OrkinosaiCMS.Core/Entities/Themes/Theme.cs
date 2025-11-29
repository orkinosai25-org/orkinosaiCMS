using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Sites;

/// <summary>
/// Represents a visual theme for the CMS
/// </summary>
public class Theme : BaseEntity
{
    /// <summary>
    /// Theme name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Theme description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Version of the theme
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Author of the theme
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Path to theme assets (CSS, images, etc.)
    /// </summary>
    public string AssetsPath { get; set; } = string.Empty;

    /// <summary>
    /// Preview thumbnail URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Whether the theme is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether this is a system theme
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Default settings as JSON
    /// </summary>
    public string? DefaultSettings { get; set; }
}
