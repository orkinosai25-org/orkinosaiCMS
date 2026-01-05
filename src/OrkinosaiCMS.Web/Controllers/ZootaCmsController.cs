using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Services;

namespace OrkinosaiCMS.Web.Controllers;

/// <summary>
/// API controller for Zoota AI Assistant CMS operations
/// Admin-only endpoints for managing CMS content
/// </summary>
[ApiController]
[Route("api/zoota/cms")]
[Authorize(Roles = "Administrator")]
public class ZootaCmsController : ControllerBase
{
    private readonly IPageService _pageService;
    private readonly IContentService _contentService;
    private readonly IUserService _userService;
    private readonly IPageLayoutService _pageLayoutService;
    private readonly IAIImageGenerationService _aiImageService;
    private readonly IMediaService _mediaService;
    private readonly ILogger<ZootaCmsController> _logger;

    public ZootaCmsController(
        IPageService pageService,
        IContentService contentService,
        IUserService userService,
        IPageLayoutService pageLayoutService,
        IAIImageGenerationService aiImageService,
        IMediaService mediaService,
        ILogger<ZootaCmsController> logger)
    {
        _pageService = pageService;
        _contentService = contentService;
        _userService = userService;
        _pageLayoutService = pageLayoutService;
        _aiImageService = aiImageService;
        _mediaService = mediaService;
        _logger = logger;
    }

    #region Pages

    /// <summary>
    /// List all pages
    /// </summary>
    [HttpGet("pages")]
    public async Task<IActionResult> GetPages()
    {
        try
        {
            var pages = await _pageService.GetAllAsync();
            return Ok(new
            {
                success = true,
                data = pages.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Path,
                    p.IsPublished,
                    p.CreatedOn,
                    p.ModifiedOn
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pages");
            return StatusCode(500, new { success = false, message = "Error retrieving pages" });
        }
    }

    /// <summary>
    /// Create a new page
    /// </summary>
    [HttpPost("pages")]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageRequest request)
    {
        try
        {
            var page = new Page
            {
                Title = request.Title,
                Path = request.Path ?? GenerateSlug(request.Title),
                MetaDescription = request.MetaDescription,
                IsPublished = request.IsPublished ?? false,
                ShowInNavigation = request.ShowInNavigation ?? true, // Default to showing in navigation
                SiteId = 1, // Default site
                CreatedBy = User.Identity?.Name ?? "admin",
                CreatedOn = DateTime.UtcNow
            };

            var created = await _pageService.CreateAsync(page);
            
            return Ok(new
            {
                success = true,
                message = $"Page '{created.Title}' created successfully with routing at '/{created.Path}' and navigation setup",
                data = new { created.Id, created.Title, created.Path, created.ShowInNavigation, created.IsPublished }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating page");
            return StatusCode(500, new { success = false, message = "Error creating page" });
        }
    }

    /// <summary>
    /// Update an existing page
    /// </summary>
    [HttpPut("pages/{id}")]
    public async Task<IActionResult> UpdatePage(int id, [FromBody] UpdatePageRequest request)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
            {
                return NotFound(new { success = false, message = "Page not found" });
            }

            if (!string.IsNullOrEmpty(request.Title))
                page.Title = request.Title;
            
            if (!string.IsNullOrEmpty(request.Path))
                page.Path = request.Path;
            
            if (request.MetaDescription != null)
                page.MetaDescription = request.MetaDescription;
            
            if (request.IsPublished.HasValue)
                page.IsPublished = request.IsPublished.Value;

            page.ModifiedBy = User.Identity?.Name ?? "admin";
            page.ModifiedOn = DateTime.UtcNow;

            await _pageService.UpdateAsync(page);

            return Ok(new
            {
                success = true,
                message = $"Page '{page.Title}' updated successfully",
                data = new { page.Id, page.Title, page.Path }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating page");
            return StatusCode(500, new { success = false, message = "Error updating page" });
        }
    }

    /// <summary>
    /// Delete a page
    /// </summary>
    [HttpDelete("pages/{id}")]
    public async Task<IActionResult> DeletePage(int id)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
            {
                return NotFound(new { success = false, message = "Page not found" });
            }

            var title = page.Title;
            await _pageService.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = $"Page '{title}' deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting page");
            return StatusCode(500, new { success = false, message = "Error deleting page" });
        }
    }

    #endregion

    #region Content

    /// <summary>
    /// List all content items
    /// </summary>
    [HttpGet("content")]
    public async Task<IActionResult> GetContent()
    {
        try
        {
            var content = await _contentService.GetAllAsync();
            return Ok(new
            {
                success = true,
                data = content.Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.ContentType,
                    c.CreatedOn,
                    c.ModifiedOn
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content");
            return StatusCode(500, new { success = false, message = "Error retrieving content" });
        }
    }

    /// <summary>
    /// Create new content
    /// </summary>
    [HttpPost("content")]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentRequest request)
    {
        try
        {
            var content = new Content
            {
                Title = request.Title,
                ContentType = request.ContentType ?? "Document",
                Body = request.Body,
                SiteId = 1, // Default site
                CreatedBy = User.Identity?.Name ?? "admin",
                CreatedOn = DateTime.UtcNow
            };

            var created = await _contentService.CreateAsync(content);

            return Ok(new
            {
                success = true,
                message = $"Content '{created.Title}' created successfully",
                data = new { created.Id, created.Title, created.ContentType }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content");
            return StatusCode(500, new { success = false, message = "Error creating content" });
        }
    }

    /// <summary>
    /// Update existing content
    /// </summary>
    [HttpPut("content/{id}")]
    public async Task<IActionResult> UpdateContent(int id, [FromBody] UpdateContentRequest request)
    {
        try
        {
            var content = await _contentService.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound(new { success = false, message = "Content not found" });
            }

            if (!string.IsNullOrEmpty(request.Title))
                content.Title = request.Title;
            
            if (request.Body != null)
                content.Body = request.Body;

            content.ModifiedBy = User.Identity?.Name ?? "admin";
            content.ModifiedOn = DateTime.UtcNow;

            await _contentService.UpdateAsync(content);

            return Ok(new
            {
                success = true,
                message = $"Content '{content.Title}' updated successfully",
                data = new { content.Id, content.Title }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content");
            return StatusCode(500, new { success = false, message = "Error updating content" });
        }
    }

    /// <summary>
    /// Delete content
    /// </summary>
    [HttpDelete("content/{id}")]
    public async Task<IActionResult> DeleteContent(int id)
    {
        try
        {
            var content = await _contentService.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound(new { success = false, message = "Content not found" });
            }

            var title = content.Title;
            await _contentService.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = $"Content '{title}' deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content");
            return StatusCode(500, new { success = false, message = "Error deleting content" });
        }
    }

    #endregion

    #region Users

    /// <summary>
    /// List all users (excluding passwords)
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(new
            {
                success = true,
                data = users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.DisplayName,
                    u.IsActive,
                    u.CreatedOn,
                    u.LastLoginOn
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new { success = false, message = "Error retrieving users" });
        }
    }

    #endregion

    #region Page Designer

    /// <summary>
    /// Apply a layout template to a page
    /// </summary>
    [HttpPost("pages/{pageId}/layout/template")]
    public async Task<IActionResult> ApplyTemplate(int pageId, [FromBody] ApplyTemplateRequest request)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(pageId);
            if (page == null)
            {
                return NotFound(new { success = false, message = "Page not found" });
            }

            var layout = await _pageLayoutService.ApplyTemplateAsync(pageId, request.TemplateName);

            return Ok(new
            {
                success = true,
                message = $"Template '{request.TemplateName}' applied to page '{page.Title}'",
                data = new { layoutId = layout.Id, pageId, template = request.TemplateName }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying template");
            return StatusCode(500, new { success = false, message = "Error applying template" });
        }
    }

    /// <summary>
    /// Get the layout for a page
    /// </summary>
    [HttpGet("pages/{pageId}/layout")]
    public async Task<IActionResult> GetPageLayout(int pageId)
    {
        try
        {
            var layout = await _pageLayoutService.GetLayoutByPageIdAsync(pageId);
            if (layout == null)
            {
                return NotFound(new { success = false, message = "Layout not found for this page" });
            }

            var sections = await _pageLayoutService.GetSectionsByLayoutIdAsync(layout.Id);
            var sectionData = new List<object>();

            foreach (var section in sections)
            {
                var blocks = await _pageLayoutService.GetBlocksBySectionIdAsync(section.Id);
                sectionData.Add(new
                {
                    section.Id,
                    section.Order,
                    section.SectionType,
                    blocks = blocks.Select(b => new
                    {
                        b.Id,
                        b.BlockType,
                        b.ColumnIndex,
                        b.Order,
                        b.Content
                    })
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    layoutId = layout.Id,
                    pageId = layout.PageId,
                    sections = sectionData
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting page layout");
            return StatusCode(500, new { success = false, message = "Error retrieving page layout" });
        }
    }

    /// <summary>
    /// Add a section to a page layout
    /// </summary>
    [HttpPost("pages/{pageId}/layout/sections")]
    public async Task<IActionResult> AddSection(int pageId, [FromBody] AddSectionRequest request)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(pageId);
            if (page == null)
            {
                return NotFound(new { success = false, message = "Page not found" });
            }

            var layout = await _pageLayoutService.GetLayoutByPageIdAsync(pageId);
            if (layout == null)
            {
                layout = await _pageLayoutService.CreateLayoutAsync(new PageLayout
                {
                    PageId = pageId,
                    IsActive = true
                });
            }

            var existingSections = await _pageLayoutService.GetSectionsByLayoutIdAsync(layout.Id);

            var section = new PageSection
            {
                PageLayoutId = layout.Id,
                Order = existingSections.Count(),
                SectionType = request.SectionType ?? "full-width",
                ColumnConfiguration = GetColumnConfiguration(request.SectionType ?? "full-width")
            };

            var created = await _pageLayoutService.CreateSectionAsync(section);

            return Ok(new
            {
                success = true,
                message = $"Section added to page '{page.Title}'",
                data = new { sectionId = created.Id, created.SectionType, created.Order }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding section");
            return StatusCode(500, new { success = false, message = "Error adding section" });
        }
    }

    /// <summary>
    /// Add a block to a section
    /// </summary>
    [HttpPost("pages/{pageId}/layout/sections/{sectionId}/blocks")]
    public async Task<IActionResult> AddBlock(int pageId, int sectionId, [FromBody] AddBlockRequest request)
    {
        try
        {
            var section = await _pageLayoutService.GetSectionsByLayoutIdAsync(0); // Get section by ID
            var targetSection = section.FirstOrDefault(s => s.Id == sectionId);
            
            if (targetSection == null)
            {
                return NotFound(new { success = false, message = "Section not found" });
            }

            var existingBlocks = await _pageLayoutService.GetBlocksBySectionIdAsync(sectionId);
            var columnBlocks = existingBlocks.Where(b => b.ColumnIndex == (request.ColumnIndex ?? 0));

            var block = new PageBlock
            {
                PageSectionId = sectionId,
                ColumnIndex = request.ColumnIndex ?? 0,
                Order = columnBlocks.Count(),
                BlockType = request.BlockType,
                Content = request.Content ?? GetDefaultBlockContent(request.BlockType)
            };

            var created = await _pageLayoutService.CreateBlockAsync(block);

            return Ok(new
            {
                success = true,
                message = $"{request.BlockType} block added to section",
                data = new { blockId = created.Id, created.BlockType, created.ColumnIndex }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding block");
            return StatusCode(500, new { success = false, message = "Error adding block" });
        }
    }

    /// <summary>
    /// Update block content
    /// </summary>
    [HttpPut("pages/{pageId}/layout/blocks/{blockId}")]
    public async Task<IActionResult> UpdateBlock(int pageId, int blockId, [FromBody] UpdateBlockRequest request)
    {
        try
        {
            var sections = await _pageLayoutService.GetSectionsByLayoutIdAsync(0);
            PageBlock? block = null;

            foreach (var section in sections)
            {
                var blocks = await _pageLayoutService.GetBlocksBySectionIdAsync(section.Id);
                block = blocks.FirstOrDefault(b => b.Id == blockId);
                if (block != null) break;
            }

            if (block == null)
            {
                return NotFound(new { success = false, message = "Block not found" });
            }

            if (!string.IsNullOrEmpty(request.Content))
            {
                block.Content = request.Content;
            }

            await _pageLayoutService.UpdateBlockAsync(block);

            return Ok(new
            {
                success = true,
                message = "Block updated successfully",
                data = new { blockId = block.Id, block.BlockType }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating block");
            return StatusCode(500, new { success = false, message = "Error updating block" });
        }
    }

    /// <summary>
    /// Delete a block
    /// </summary>
    [HttpDelete("pages/{pageId}/layout/blocks/{blockId}")]
    public async Task<IActionResult> DeleteBlock(int pageId, int blockId)
    {
        try
        {
            await _pageLayoutService.DeleteBlockAsync(blockId);

            return Ok(new
            {
                success = true,
                message = "Block deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting block");
            return StatusCode(500, new { success = false, message = "Error deleting block" });
        }
    }

    /// <summary>
    /// Delete a section
    /// </summary>
    [HttpDelete("pages/{pageId}/layout/sections/{sectionId}")]
    public async Task<IActionResult> DeleteSection(int pageId, int sectionId)
    {
        try
        {
            await _pageLayoutService.DeleteSectionAsync(sectionId);

            return Ok(new
            {
                success = true,
                message = "Section deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting section");
            return StatusCode(500, new { success = false, message = "Error deleting section" });
        }
    }

    /// <summary>
    /// Generate an AI image and add it as a hero/banner to the page
    /// </summary>
    [HttpPost("pages/{pageId}/generate-banner")]
    public async Task<IActionResult> GenerateBanner(int pageId, [FromBody] GenerateBannerRequest request)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(pageId);
            if (page == null)
            {
                return NotFound(new { success = false, message = "Page not found" });
            }

            // Generate the image using DALL-E
            _logger.LogInformation("Generating banner image for page {PageId} with prompt: {Prompt}", pageId, request.Prompt);
            
            var fileName = $"banner_{page.Title.ToLower().Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var mediaFile = await _aiImageService.GenerateAndSaveImageAsync(
                request.Prompt,
                fileName,
                null, // root folder
                request.Size ?? "1792x1024"
            );

            // Get or create layout for the page
            var layout = await _pageLayoutService.GetLayoutByPageIdAsync(pageId);
            if (layout == null)
            {
                layout = await _pageLayoutService.CreateLayoutAsync(new PageLayout
                {
                    PageId = pageId,
                    IsActive = true
                });
            }

            // Get existing sections
            var sections = await _pageLayoutService.GetSectionsByLayoutIdAsync(layout.Id);
            var existingSections = sections.ToList();

            // Check if there's already a hero section at the top
            PageSection heroSection;
            if (existingSections.Any() && existingSections.OrderBy(s => s.Order).First().SectionType == "full-width")
            {
                // Use existing first section if it's full-width
                heroSection = existingSections.OrderBy(s => s.Order).First();
                
                // Check if there are existing blocks
                var existingBlocks = await _pageLayoutService.GetBlocksBySectionIdAsync(heroSection.Id);
                var heroBlocks = existingBlocks.Where(b => b.BlockType == "hero").ToList();
                
                if (heroBlocks.Any())
                {
                    // Update existing hero block
                    var heroBlock = heroBlocks.First();
                    heroBlock.Content = $"{{\"title\": \"{request.Title ?? "Welcome"}\", \"subtitle\": \"{request.Subtitle ?? ""}\", \"imageUrl\": \"/uploads/{mediaFile.FileName}\"}}";
                    await _pageLayoutService.UpdateBlockAsync(heroBlock);
                }
                else
                {
                    // Add new hero block
                    var heroBlock = new PageBlock
                    {
                        PageSectionId = heroSection.Id,
                        ColumnIndex = 0,
                        Order = 0,
                        BlockType = "hero",
                        Content = $"{{\"title\": \"{request.Title ?? "Welcome"}\", \"subtitle\": \"{request.Subtitle ?? ""}\", \"imageUrl\": \"/uploads/{mediaFile.FileName}\"}}"
                    };
                    await _pageLayoutService.CreateBlockAsync(heroBlock);
                }
            }
            else
            {
                // Create new hero section at the top
                // Reorder existing sections
                foreach (var section in existingSections)
                {
                    section.Order++;
                    await _pageLayoutService.UpdateSectionAsync(section);
                }

                heroSection = new PageSection
                {
                    PageLayoutId = layout.Id,
                    Order = 0,
                    SectionType = "full-width",
                    ColumnConfiguration = "[{\"width\": \"100%\"}]"
                };
                heroSection = await _pageLayoutService.CreateSectionAsync(heroSection);

                // Add hero block with generated image
                var heroBlock = new PageBlock
                {
                    PageSectionId = heroSection.Id,
                    ColumnIndex = 0,
                    Order = 0,
                    BlockType = "hero",
                    Content = $"{{\"title\": \"{request.Title ?? "Welcome"}\", \"subtitle\": \"{request.Subtitle ?? ""}\", \"imageUrl\": \"/uploads/{mediaFile.FileName}\"}}"
                };
                await _pageLayoutService.CreateBlockAsync(heroBlock);
            }

            return Ok(new
            {
                success = true,
                message = $"Banner generated and added to page '{page.Title}'",
                data = new
                {
                    mediaFileId = mediaFile.Id,
                    imageUrl = $"/uploads/{mediaFile.FileName}",
                    fileName = mediaFile.FileName,
                    prompt = request.Prompt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Configuration error generating banner");
            return StatusCode(500, new { success = false, message = $"Configuration error: {ex.Message}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating banner for page {PageId}", pageId);
            return StatusCode(500, new { success = false, message = "Error generating banner image" });
        }
    }

    #endregion

    #region Helpers

    private string GenerateSlug(string title)
    {
        // Convert to lowercase and remove diacritics
        var slug = title.ToLower().Trim();
        
        // Replace invalid characters with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        
        // Replace multiple spaces or hyphens with single hyphen
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[\s-]+", "-");
        
        // Remove leading and trailing hyphens
        slug = slug.Trim('-');
        
        return slug;
    }

    private string GetColumnConfiguration(string sectionType)
    {
        return sectionType switch
        {
            "two-column" => "[{\"width\": \"50%\"}, {\"width\": \"50%\"}]",
            "three-column" => "[{\"width\": \"33.33%\"}, {\"width\": \"33.33%\"}, {\"width\": \"33.33%\"}]",
            _ => "[{\"width\": \"100%\"}]"
        };
    }

    private string GetDefaultBlockContent(string blockType)
    {
        return blockType switch
        {
            "text" => "{\"html\": \"<p>Enter your text here</p>\"}",
            "image" => "{\"src\": \"\", \"alt\": \"Image\"}",
            "video" => "{\"url\": \"\"}",
            "hero" => "{\"title\": \"Hero Title\", \"subtitle\": \"Subtitle\", \"imageUrl\": \"\"}",
            "html" => "{\"html\": \"<div>Custom HTML</div>\"}",
            "gallery" => "{\"images\": []}",
            "cards" => "{\"title\": \"Card Title\", \"text\": \"Card content\", \"imageUrl\": \"\"}",
            _ => "{}"
        };
    }

    #endregion
}

#region Request Models

public class CreatePageRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? MetaDescription { get; set; }
    public bool? IsPublished { get; set; }
    public bool? ShowInNavigation { get; set; }
}

public class UpdatePageRequest
{
    public string? Title { get; set; }
    public string? Path { get; set; }
    public string? MetaDescription { get; set; }
    public bool? IsPublished { get; set; }
}

public class CreateContentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? Body { get; set; }
}

public class UpdateContentRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class ApplyTemplateRequest
{
    public string TemplateName { get; set; } = string.Empty;
}

public class AddSectionRequest
{
    public string? SectionType { get; set; }
}

public class AddBlockRequest
{
    public string BlockType { get; set; } = string.Empty;
    public int? ColumnIndex { get; set; }
    public string? Content { get; set; }
}

public class UpdateBlockRequest
{
    public string? Content { get; set; }
}

public class GenerateBannerRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Size { get; set; }
}

#endregion
