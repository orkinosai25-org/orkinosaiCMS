namespace OrkinosaiCMS.Shared.DTOs;

/// <summary>
/// DTO for creating new content
/// </summary>
public class CreateContentDto
{
    public int SiteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = "HTML";
    public string? Body { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; }
    public int? AuthorId { get; set; }
}

/// <summary>
/// DTO for updating existing content
/// </summary>
public class UpdateContentDto
{
    public string? Title { get; set; }
    public string? ContentType { get; set; }
    public string? Body { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool? IsPublished { get; set; }
}
