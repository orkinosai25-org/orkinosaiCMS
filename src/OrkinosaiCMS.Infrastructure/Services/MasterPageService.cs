using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service implementation for master page management operations
/// </summary>
public class MasterPageService : IMasterPageService
{
    private readonly IRepository<MasterPage> _masterPageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public MasterPageService(
        IRepository<MasterPage> masterPageRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _masterPageRepository = masterPageRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<MasterPage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _masterPageRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<MasterPage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _masterPageRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<MasterPage>> GetBySiteAsync(int siteId, CancellationToken cancellationToken = default)
    {
        return await _masterPageRepository.FindAsync(m => m.SiteId == siteId, cancellationToken);
    }

    public async Task<MasterPage?> GetDefaultForSiteAsync(int siteId, CancellationToken cancellationToken = default)
    {
        return await _masterPageRepository.FirstOrDefaultAsync(
            m => m.SiteId == siteId && m.IsDefault, cancellationToken);
    }

    public async Task<MasterPage> CreateAsync(MasterPage masterPage, CancellationToken cancellationToken = default)
    {
        masterPage.CreatedOn = DateTime.UtcNow;
        await _masterPageRepository.AddAsync(masterPage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return masterPage;
    }

    public async Task<MasterPage> UpdateAsync(MasterPage masterPage, CancellationToken cancellationToken = default)
    {
        masterPage.ModifiedOn = DateTime.UtcNow;
        _masterPageRepository.Update(masterPage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return masterPage;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var masterPage = await _masterPageRepository.GetByIdAsync(id, cancellationToken);
        if (masterPage != null)
        {
            _masterPageRepository.Remove(masterPage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SetAsDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        var masterPage = await _masterPageRepository.GetByIdAsync(id, cancellationToken);
        if (masterPage == null)
        {
            throw new ArgumentException($"Master page with ID {id} not found.");
        }

        // Get all master pages for the same site
        var siteMasterPages = await _masterPageRepository.FindAsync(
            m => m.SiteId == masterPage.SiteId, cancellationToken);

        // Set all to non-default
        foreach (var mp in siteMasterPages)
        {
            if (mp.IsDefault)
            {
                mp.IsDefault = false;
                mp.ModifiedOn = DateTime.UtcNow;
                _masterPageRepository.Update(mp);
            }
        }

        // Set the selected one as default
        masterPage.IsDefault = true;
        masterPage.ModifiedOn = DateTime.UtcNow;
        _masterPageRepository.Update(masterPage);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
