using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Sites;

/// <summary>
/// Represents a section (row) in a page layout
/// Each section can contain multiple columns
/// </summary>
public class PageSection : BaseEntity
{
    /// <summary>
    /// Layout this section belongs to
    /// </summary>
    public int PageLayoutId { get; set; }

    /// <summary>
    /// Display order of this section
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Section type (e.g., "full-width", "two-column", "three-column")
    /// </summary>
    public string SectionType { get; set; } = "full-width";

    /// <summary>
    /// Column configuration as JSON (e.g., column widths)
    /// </summary>
    public string ColumnConfiguration { get; set; } = "[]";

    /// <summary>
    /// Background color or CSS class
    /// </summary>
    public string? BackgroundStyle { get; set; }

    /// <summary>
    /// Navigation to layout
    /// </summary>
    public PageLayout? PageLayout { get; set; }

    /// <summary>
    /// Blocks in this section
    /// </summary>
    public ICollection<PageBlock> Blocks { get; set; } = new List<PageBlock>();
}
