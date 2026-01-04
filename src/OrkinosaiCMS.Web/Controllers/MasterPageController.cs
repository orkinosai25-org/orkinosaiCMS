using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Shared.DTOs;

namespace OrkinosaiCMS.Web.Controllers;

/// <summary>
/// API Controller for master page management, used by admin UI
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class MasterPageController : ControllerBase
{
    private readonly IMasterPageService _masterPageService;
    private readonly ILogger<MasterPageController> _logger;

    public MasterPageController(IMasterPageService masterPageService, ILogger<MasterPageController> logger)
    {
        _masterPageService = masterPageService;
        _logger = logger;
    }

    /// <summary>
    /// Get all master pages
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MasterPage>), 200)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var masterPages = await _masterPageService.GetAllAsync();
            return Ok(masterPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all master pages");
            return StatusCode(500, new { message = "Error retrieving master pages" });
        }
    }

    /// <summary>
    /// Get master pages by site
    /// </summary>
    [HttpGet("site/{siteId}")]
    [ProducesResponseType(typeof(IEnumerable<MasterPage>), 200)]
    public async Task<IActionResult> GetBySite(int siteId)
    {
        try
        {
            var masterPages = await _masterPageService.GetBySiteAsync(siteId);
            return Ok(masterPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting master pages by site {SiteId}", siteId);
            return StatusCode(500, new { message = "Error retrieving master pages" });
        }
    }

    /// <summary>
    /// Get default master page for a site
    /// </summary>
    [HttpGet("site/{siteId}/default")]
    [ProducesResponseType(typeof(MasterPage), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDefaultForSite(int siteId)
    {
        try
        {
            var masterPage = await _masterPageService.GetDefaultForSiteAsync(siteId);
            if (masterPage == null)
                return NotFound(new { message = $"No default master page found for site {siteId}" });

            return Ok(masterPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting default master page for site {SiteId}", siteId);
            return StatusCode(500, new { message = "Error retrieving default master page" });
        }
    }

    /// <summary>
    /// Get master page by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MasterPage), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var masterPage = await _masterPageService.GetByIdAsync(id);
            if (masterPage == null)
                return NotFound(new { message = $"Master page with ID {id} not found" });

            return Ok(masterPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting master page by ID {MasterPageId}", id);
            return StatusCode(500, new { message = "Error retrieving master page" });
        }
    }

    /// <summary>
    /// Create a new master page
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MasterPage), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateMasterPageDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Master page name is required" });

            if (string.IsNullOrWhiteSpace(dto.ComponentPath))
                return BadRequest(new { message = "Component path is required" });

            var masterPage = new MasterPage
            {
                SiteId = dto.SiteId,
                Name = dto.Name,
                Description = dto.Description,
                ComponentPath = dto.ComponentPath,
                ThumbnailUrl = dto.ThumbnailUrl,
                IsDefault = dto.IsDefault,
                ContentZones = dto.ContentZones
            };

            var created = await _masterPageService.CreateAsync(masterPage);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating master page");
            return StatusCode(500, new { message = "Error creating master page" });
        }
    }

    /// <summary>
    /// Update an existing master page
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MasterPage), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMasterPageDto dto)
    {
        try
        {
            var masterPage = await _masterPageService.GetByIdAsync(id);
            if (masterPage == null)
                return NotFound(new { message = $"Master page with ID {id} not found" });

            if (!string.IsNullOrWhiteSpace(dto.Name))
                masterPage.Name = dto.Name;

            if (dto.Description != null)
                masterPage.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.ComponentPath))
                masterPage.ComponentPath = dto.ComponentPath;

            if (dto.ThumbnailUrl != null)
                masterPage.ThumbnailUrl = dto.ThumbnailUrl;

            if (dto.IsDefault.HasValue)
                masterPage.IsDefault = dto.IsDefault.Value;

            if (!string.IsNullOrWhiteSpace(dto.ContentZones))
                masterPage.ContentZones = dto.ContentZones;

            var updated = await _masterPageService.UpdateAsync(masterPage);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating master page {MasterPageId}", id);
            return StatusCode(500, new { message = "Error updating master page" });
        }
    }

    /// <summary>
    /// Delete a master page
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var masterPage = await _masterPageService.GetByIdAsync(id);
            if (masterPage == null)
                return NotFound(new { message = $"Master page with ID {id} not found" });

            await _masterPageService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting master page {MasterPageId}", id);
            return StatusCode(500, new { message = "Error deleting master page" });
        }
    }

    /// <summary>
    /// Set master page as default for its site
    /// </summary>
    [HttpPost("{id}/set-default")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetAsDefault(int id)
    {
        try
        {
            await _masterPageService.SetAsDefaultAsync(id);
            return Ok(new { message = "Master page set as default successfully" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Master page not found: {MasterPageId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting master page {MasterPageId} as default", id);
            return StatusCode(500, new { message = "Error setting master page as default" });
        }
    }
}
