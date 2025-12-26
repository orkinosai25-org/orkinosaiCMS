namespace OrkinosaiCMS.Shared.DTOs.Navigation;

/// <summary>
/// DTO for creating/updating navigation items
/// </summary>
public class NavigationItemDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? PageId { get; set; }
    public string? IconCssClass { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool OpenInNewWindow { get; set; } = false;
    public string? CssClass { get; set; }
    public string? Description { get; set; }
    public string? RequiredRoles { get; set; }
    public string? RequiredPermission { get; set; }
    public List<NavigationItemDto> Children { get; set; } = new();
}
