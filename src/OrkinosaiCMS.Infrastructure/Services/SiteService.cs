using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service implementation for site management.
/// Redesigned from Mosaic with improved tenant isolation and validation.
/// </summary>
public class SiteService : ISiteService
{
    private readonly IRepository<Site> _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public SiteService(
        IRepository<Site> siteRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Site?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid site ID", nameof(id));

        return await _siteRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Site?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Site name cannot be empty", nameof(name));

        return await _context.Sites
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower() && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Site>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
            throw new ArgumentException("Invalid user ID", nameof(userId));

        // Assuming Site has a CreatedBy field that stores the user ID
        return await _context.Sites
            .Where(s => s.CreatedBy == userId.ToString() && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<Site> CreateAsync(Site site, CancellationToken cancellationToken = default)
    {
        if (site == null)
            throw new ArgumentNullException(nameof(site));

        if (string.IsNullOrWhiteSpace(site.Name))
            throw new ArgumentException("Site name is required", nameof(site));

        // Check if name is available
        var nameExists = await _context.Sites
            .AnyAsync(s => s.Name.ToLower() == site.Name.ToLower() && !s.IsDeleted, cancellationToken);

        if (nameExists)
            throw new InvalidOperationException($"Site name '{site.Name}' is already taken");

        site.CreatedOn = DateTime.UtcNow;
        site.CreatedBy = "System"; // TODO: Replace with actual user context
        
        var result = await _siteRepository.AddAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return result;
    }

    public async Task<Site> UpdateAsync(Site site, CancellationToken cancellationToken = default)
    {
        if (site == null)
            throw new ArgumentNullException(nameof(site));

        if (site.Id <= 0)
            throw new ArgumentException("Invalid site ID", nameof(site));

        site.ModifiedOn = DateTime.UtcNow;
        site.ModifiedBy = "System"; // TODO: Replace with actual user context
        
        _siteRepository.Update(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return site;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid site ID", nameof(id));

        var site = await _siteRepository.GetByIdAsync(id, cancellationToken);
        if (site == null)
            throw new InvalidOperationException($"Site with ID {id} not found");

        // Soft delete
        site.IsDeleted = true;
        site.DeletedOn = DateTime.UtcNow;
        site.ModifiedOn = DateTime.UtcNow;
        site.ModifiedBy = "System"; // TODO: Replace with actual user context
        
        _siteRepository.Update(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsNameAvailableAsync(string name, int? excludeSiteId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Site name cannot be empty", nameof(name));

        var query = _context.Sites.Where(s => s.Name.ToLower() == name.ToLower() && !s.IsDeleted);

        if (excludeSiteId.HasValue)
            query = query.Where(s => s.Id != excludeSiteId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Site> Sites, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

        var query = _context.Sites.AsQueryable();

        if (!includeDeleted)
            query = query.Where(s => !s.IsDeleted);

        query = query.OrderByDescending(s => s.CreatedOn);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var sites = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (sites, totalCount);
    }

    public async Task<object> GetSiteStatisticsAsync(int siteId, CancellationToken cancellationToken = default)
    {
        if (siteId <= 0)
            throw new ArgumentException("Invalid site ID", nameof(siteId));

        var site = await _context.Sites
            .FirstOrDefaultAsync(s => s.Id == siteId && !s.IsDeleted, cancellationToken);

        if (site == null)
            throw new InvalidOperationException($"Site with ID {siteId} not found");

        // Get related statistics
        var pageCount = await _context.Pages
            .CountAsync(p => p.SiteId == siteId && !p.IsDeleted, cancellationToken);

        return new
        {
            SiteId = siteId,
            SiteName = site.Name,
            PageCount = pageCount,
            CreatedOn = site.CreatedOn,
            LastModified = site.ModifiedOn
        };
    }
}
