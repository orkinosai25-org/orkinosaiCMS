namespace OrkinosaiCMS.Shared.DTOs.Navigation;

/// <summary>
/// DTO for creating/updating navigation menus
/// </summary>
public class NavigationMenuDto
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = "Top";
    public bool IsEnabled { get; set; } = true;
    public string? CssClass { get; set; }
    public int MaxDepth { get; set; } = 3;
}
