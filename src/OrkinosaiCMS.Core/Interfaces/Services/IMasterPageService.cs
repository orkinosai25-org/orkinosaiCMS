using OrkinosaiCMS.Core.Entities.Sites;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for master page management operations
/// </summary>
public interface IMasterPageService
{
    /// <summary>
    /// Get master page by ID
    /// </summary>
    Task<MasterPage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all master pages
    /// </summary>
    Task<IEnumerable<MasterPage>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get master pages by site
    /// </summary>
    Task<IEnumerable<MasterPage>> GetBySiteAsync(int siteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get default master page for a site
    /// </summary>
    Task<MasterPage?> GetDefaultForSiteAsync(int siteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new master page
    /// </summary>
    Task<MasterPage> CreateAsync(MasterPage masterPage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing master page
    /// </summary>
    Task<MasterPage> UpdateAsync(MasterPage masterPage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a master page
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set master page as default for a site
    /// </summary>
    Task SetAsDefaultAsync(int id, CancellationToken cancellationToken = default);
}
