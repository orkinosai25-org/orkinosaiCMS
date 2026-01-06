using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Sites;

/// <summary>
/// Represents a content block within a page section
/// Can be text, image, media, HTML, or custom widget
/// </summary>
public class PageBlock : BaseEntity
{
    /// <summary>
    /// Section this block belongs to
    /// </summary>
    public int PageSectionId { get; set; }

    /// <summary>
    /// Column index (0-based) within the section
    /// </summary>
    public int ColumnIndex { get; set; }

    /// <summary>
    /// Display order within the column
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Block type (text, image, gallery, video, html, hero, cards)
    /// </summary>
    public string BlockType { get; set; } = "text";

    /// <summary>
    /// Block content as JSON
    /// Structure depends on BlockType
    /// </summary>
    public string Content { get; set; } = "{}";

    /// <summary>
    /// Block settings/configuration as JSON
    /// </summary>
    public string? Settings { get; set; }

    /// <summary>
    /// Navigation to section
    /// </summary>
    public PageSection? PageSection { get; set; }
}
