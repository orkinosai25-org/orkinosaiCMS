using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Media;

/// <summary>
/// Represents a media file (image, document, etc.) in the media library
/// </summary>
public class MediaFile : BaseEntity
{
    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File title/display name
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description or alt text
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// File extension (e.g., .jpg, .pdf)
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (e.g., image/jpeg, application/pdf)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Relative path to the file from wwwroot
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Public URL to access the file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail URL for images
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Width in pixels (for images)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Height in pixels (for images)
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Folder ID where this file is stored
    /// </summary>
    public int? FolderId { get; set; }

    /// <summary>
    /// Parent folder navigation property
    /// </summary>
    public MediaFolder? Folder { get; set; }

    /// <summary>
    /// Tags for organizing and searching media
    /// </summary>
    public string? Tags { get; set; }
}
