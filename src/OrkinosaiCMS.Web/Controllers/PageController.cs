using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Shared.DTOs;

namespace OrkinosaiCMS.Web.Controllers;

/// <summary>
/// API Controller for page management, used by admin UI
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class PageController : ControllerBase
{
    private readonly IPageService _pageService;
    private readonly ILogger<PageController> _logger;

    public PageController(IPageService pageService, ILogger<PageController> logger)
    {
        _pageService = pageService;
        _logger = logger;
    }

    /// <summary>
    /// Get all pages
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Page>), 200)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var pages = await _pageService.GetAllAsync();
            return Ok(pages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all pages");
            return StatusCode(500, new { message = "Error retrieving pages" });
        }
    }

    /// <summary>
    /// Get pages by site
    /// </summary>
    [HttpGet("site/{siteId}")]
    [ProducesResponseType(typeof(IEnumerable<Page>), 200)]
    public async Task<IActionResult> GetBySite(int siteId)
    {
        try
        {
            var pages = await _pageService.GetBySiteAsync(siteId);
            return Ok(pages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pages by site {SiteId}", siteId);
            return StatusCode(500, new { message = "Error retrieving pages" });
        }
    }

    /// <summary>
    /// Get published pages
    /// </summary>
    [HttpGet("site/{siteId}/published")]
    [ProducesResponseType(typeof(IEnumerable<Page>), 200)]
    public async Task<IActionResult> GetPublished(int siteId)
    {
        try
        {
            var pages = await _pageService.GetPublishedPagesAsync(siteId);
            return Ok(pages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting published pages for site {SiteId}", siteId);
            return StatusCode(500, new { message = "Error retrieving published pages" });
        }
    }

    /// <summary>
    /// Get draft pages
    /// </summary>
    [HttpGet("site/{siteId}/drafts")]
    [ProducesResponseType(typeof(IEnumerable<Page>), 200)]
    public async Task<IActionResult> GetDrafts(int siteId)
    {
        try
        {
            var pages = await _pageService.GetDraftPagesAsync(siteId);
            return Ok(pages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting draft pages for site {SiteId}", siteId);
            return StatusCode(500, new { message = "Error retrieving draft pages" });
        }
    }

    /// <summary>
    /// Get page by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Page), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
                return NotFound(new { message = $"Page with ID {id} not found" });

            return Ok(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting page by ID {PageId}", id);
            return StatusCode(500, new { message = "Error retrieving page" });
        }
    }

    /// <summary>
    /// Get child pages
    /// </summary>
    [HttpGet("{id}/children")]
    [ProducesResponseType(typeof(IEnumerable<Page>), 200)]
    public async Task<IActionResult> GetChildren(int id)
    {
        try
        {
            var pages = await _pageService.GetChildPagesAsync(id);
            return Ok(pages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting child pages for page {PageId}", id);
            return StatusCode(500, new { message = "Error retrieving child pages" });
        }
    }

    /// <summary>
    /// Create a new page
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Page), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreatePageDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Page title is required" });

            if (string.IsNullOrWhiteSpace(dto.Path))
                return BadRequest(new { message = "Page path is required" });

            var page = new Page
            {
                SiteId = dto.SiteId,
                ParentId = dto.ParentId,
                Title = dto.Title,
                Path = dto.Path,
                Content = dto.Content,
                MasterPageId = dto.MasterPageId,
                Order = dto.Order,
                IsPublished = dto.IsPublished,
                ShowInNavigation = dto.ShowInNavigation,
                MetaDescription = dto.MetaDescription,
                MetaKeywords = dto.MetaKeywords,
                IconCssClass = dto.IconCssClass,
                RequiredPermission = dto.RequiredPermission
            };

            var created = await _pageService.CreateAsync(page);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating page");
            return StatusCode(500, new { message = "Error creating page" });
        }
    }

    /// <summary>
    /// Update an existing page
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Page), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePageDto dto)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
                return NotFound(new { message = $"Page with ID {id} not found" });

            if (dto.ParentId.HasValue)
                page.ParentId = dto.ParentId;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                page.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Path))
                page.Path = dto.Path;

            if (dto.Content != null)
                page.Content = dto.Content;

            if (dto.MasterPageId.HasValue)
                page.MasterPageId = dto.MasterPageId;

            if (dto.Order.HasValue)
                page.Order = dto.Order.Value;

            if (dto.IsPublished.HasValue)
                page.IsPublished = dto.IsPublished.Value;

            if (dto.ShowInNavigation.HasValue)
                page.ShowInNavigation = dto.ShowInNavigation.Value;

            if (dto.MetaDescription != null)
                page.MetaDescription = dto.MetaDescription;

            if (dto.MetaKeywords != null)
                page.MetaKeywords = dto.MetaKeywords;

            if (dto.IconCssClass != null)
                page.IconCssClass = dto.IconCssClass;

            if (dto.RequiredPermission != null)
                page.RequiredPermission = dto.RequiredPermission;

            var updated = await _pageService.UpdateAsync(page);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating page {PageId}", id);
            return StatusCode(500, new { message = "Error updating page" });
        }
    }

    /// <summary>
    /// Delete a page
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
                return NotFound(new { message = $"Page with ID {id} not found" });

            await _pageService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting page {PageId}", id);
            return StatusCode(500, new { message = "Error deleting page" });
        }
    }

    /// <summary>
    /// Publish a page
    /// </summary>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            await _pageService.PublishAsync(id);
            return Ok(new { message = "Page published successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing page {PageId}", id);
            return StatusCode(500, new { message = "Error publishing page" });
        }
    }

    /// <summary>
    /// Unpublish a page (set to draft)
    /// </summary>
    [HttpPost("{id}/unpublish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Unpublish(int id)
    {
        try
        {
            await _pageService.UnpublishAsync(id);
            return Ok(new { message = "Page unpublished successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing page {PageId}", id);
            return StatusCode(500, new { message = "Error unpublishing page" });
        }
    }

    /// <summary>
    /// Reorder a page
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Reorder([FromBody] ReorderPageDto dto)
    {
        try
        {
            await _pageService.ReorderAsync(dto.PageId, dto.NewOrder);
            return Ok(new { message = "Page reordered successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", dto.PageId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering page {PageId}", dto.PageId);
            return StatusCode(500, new { message = "Error reordering page" });
        }
    }

    /// <summary>
    /// Move a page to a different parent
    /// </summary>
    [HttpPost("move")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Move([FromBody] MovePageDto dto)
    {
        try
        {
            await _pageService.MoveAsync(dto.PageId, dto.NewParentId);
            return Ok(new { message = "Page moved successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Page not found");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid move operation");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving page {PageId}", dto.PageId);
            return StatusCode(500, new { message = "Error moving page" });
        }
    }
}
