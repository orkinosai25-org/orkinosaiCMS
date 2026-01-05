using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Sites;

/// <summary>
/// Represents a modern page layout with sections and blocks
/// Similar to SharePoint modern page layouts
/// </summary>
public class PageLayout : BaseEntity
{
    /// <summary>
    /// Page this layout belongs to
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// Layout configuration as JSON
    /// Stores section structure, column widths, etc.
    /// </summary>
    public string LayoutConfiguration { get; set; } = "{}";

    /// <summary>
    /// Whether this is the active layout for the page
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Version number for layout versioning
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Navigation to page
    /// </summary>
    public Page? Page { get; set; }

    /// <summary>
    /// Sections in this layout
    /// </summary>
    public ICollection<PageSection> Sections { get; set; } = new List<PageSection>();
}
