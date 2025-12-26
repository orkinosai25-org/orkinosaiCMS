using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Navigation;

/// <summary>
/// Represents a navigation menu container in the CMS
/// Similar to SharePoint navigation concept - allows multiple menus per site
/// </summary>
public class NavigationMenu : BaseEntity
{
    /// <summary>
    /// Site this menu belongs to
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Menu name (e.g., "TopNavigation", "Footer", "QuickLaunch")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display title for the menu
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Menu description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Location identifier (Top, Left, Footer, etc.)
    /// </summary>
    public string Location { get; set; } = "Top";

    /// <summary>
    /// Whether this menu is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// CSS class for styling
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Maximum depth level allowed for this menu
    /// </summary>
    public int MaxDepth { get; set; } = 3;

    /// <summary>
    /// Navigation items in this menu
    /// </summary>
    public ICollection<NavigationItem> Items { get; set; } = new List<NavigationItem>();

    /// <summary>
    /// Site reference
    /// </summary>
    public Sites.Site? Site { get; set; }
}
