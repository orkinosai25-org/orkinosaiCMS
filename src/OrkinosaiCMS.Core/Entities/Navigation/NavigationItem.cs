using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Navigation;

/// <summary>
/// Represents an individual navigation item
/// Inspired by SharePoint navigation items with enhanced flexibility
/// </summary>
public class NavigationItem : BaseEntity
{
    /// <summary>
    /// Menu this item belongs to
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// Parent item ID for hierarchical navigation
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Display label/text for the navigation item
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Navigation URL or path
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Optional page reference (if linking to a CMS page)
    /// </summary>
    public int? PageId { get; set; }

    /// <summary>
    /// Icon CSS class (e.g., "fas fa-home", "bi bi-house")
    /// </summary>
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Display order within parent/menu
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Whether the item is enabled/visible
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether to open link in new window
    /// </summary>
    public bool OpenInNewWindow { get; set; } = false;

    /// <summary>
    /// CSS class for styling
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Tooltip/description text
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Comma-separated list of required roles to view this item
    /// </summary>
    public string? RequiredRoles { get; set; }

    /// <summary>
    /// Required permission to view this item
    /// </summary>
    public string? RequiredPermission { get; set; }

    /// <summary>
    /// Additional custom attributes as JSON
    /// </summary>
    public string? CustomAttributes { get; set; }

    /// <summary>
    /// Navigation menu reference
    /// </summary>
    public NavigationMenu? Menu { get; set; }

    /// <summary>
    /// Parent navigation item
    /// </summary>
    public NavigationItem? Parent { get; set; }

    /// <summary>
    /// Child navigation items
    /// </summary>
    public ICollection<NavigationItem> Children { get; set; } = new List<NavigationItem>();

    /// <summary>
    /// Optional page reference
    /// </summary>
    public Sites.Page? Page { get; set; }
}
