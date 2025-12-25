using OrkinosaiCMS.Core.Entities.Sites;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for managing sites (multi-tenant CMS).
/// Redesigned from Mosaic with improved tenant isolation and validation.
/// </summary>
public interface ISiteService
{
    /// <summary>
    /// Get site by ID
    /// </summary>
    Task<Site?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get site by name (case-insensitive)
    /// </summary>
    Task<Site?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sites for a user
    /// </summary>
    Task<IEnumerable<Site>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new site
    /// </summary>
    Task<Site> CreateAsync(Site site, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing site
    /// </summary>
    Task<Site> UpdateAsync(Site site, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a site (soft delete)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if site name is available
    /// </summary>
    Task<bool> IsNameAvailableAsync(string name, int? excludeSiteId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sites (with pagination and filtering)
    /// </summary>
    Task<(IEnumerable<Site> Sites, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get site statistics (page count, user count, etc.)
    /// </summary>
    Task<object> GetSiteStatisticsAsync(int siteId, CancellationToken cancellationToken = default);
}
