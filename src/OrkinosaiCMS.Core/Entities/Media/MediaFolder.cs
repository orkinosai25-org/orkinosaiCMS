using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Media;

/// <summary>
/// Represents a folder in the media library for organizing media files
/// </summary>
public class MediaFolder : BaseEntity
{
    /// <summary>
    /// Folder name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the folder
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Parent folder ID (null for root folders)
    /// </summary>
    public int? ParentFolderId { get; set; }

    /// <summary>
    /// Parent folder navigation property
    /// </summary>
    public MediaFolder? ParentFolder { get; set; }

    /// <summary>
    /// Child folders
    /// </summary>
    public ICollection<MediaFolder> ChildFolders { get; set; } = new List<MediaFolder>();

    /// <summary>
    /// Media files in this folder
    /// </summary>
    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    /// <summary>
    /// Full path of the folder (e.g., "/Images/Products")
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
