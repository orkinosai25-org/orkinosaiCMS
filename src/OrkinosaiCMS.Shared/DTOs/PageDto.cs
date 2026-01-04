namespace OrkinosaiCMS.Shared.DTOs;

/// <summary>
/// DTO for creating a new page
/// </summary>
public class CreatePageDto
{
    public int SiteId { get; set; }
    public int? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int? MasterPageId { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public bool ShowInNavigation { get; set; } = true;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? IconCssClass { get; set; }
    public string? RequiredPermission { get; set; }
}

/// <summary>
/// DTO for updating an existing page
/// </summary>
public class UpdatePageDto
{
    public int? ParentId { get; set; }
    public string? Title { get; set; }
    public string? Path { get; set; }
    public string? Content { get; set; }
    public int? MasterPageId { get; set; }
    public int? Order { get; set; }
    public bool? IsPublished { get; set; }
    public bool? ShowInNavigation { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? IconCssClass { get; set; }
    public string? RequiredPermission { get; set; }
}

/// <summary>
/// DTO for reordering pages
/// </summary>
public class ReorderPageDto
{
    public int PageId { get; set; }
    public int NewOrder { get; set; }
}

/// <summary>
/// DTO for moving a page to a different parent
/// </summary>
public class MovePageDto
{
    public int PageId { get; set; }
    public int? NewParentId { get; set; }
}

/// <summary>
/// DTO for creating a master page
/// </summary>
public class CreateMasterPageDto
{
    public int SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentPath { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool IsDefault { get; set; }
    public string ContentZones { get; set; } = "[]";
}

/// <summary>
/// DTO for updating a master page
/// </summary>
public class UpdateMasterPageDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ComponentPath { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool? IsDefault { get; set; }
    public string? ContentZones { get; set; }
}
