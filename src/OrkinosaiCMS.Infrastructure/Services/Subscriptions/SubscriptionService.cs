using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Subscriptions;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services.Subscriptions;

/// <summary>
/// Service implementation for subscription management.
/// Redesigned from Mosaic with improved validation and state management.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly IRepository<Subscription> _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public SubscriptionService(
        IRepository<Subscription> subscriptionRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Subscription?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid subscription ID", nameof(id));

        return await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stripeSubscriptionId))
            throw new ArgumentException("Stripe subscription ID cannot be empty", nameof(stripeSubscriptionId));

        return await _context.Subscriptions
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Subscription>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(customerId));

        return await _context.Subscriptions
            .Where(s => s.CustomerId == customerId && !s.IsDeleted)
            .Include(s => s.Invoices)
            .OrderByDescending(s => s.CreatedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<Subscription?> GetActiveSubscriptionAsync(int customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(customerId));

        return await _context.Subscriptions
            .Where(s => s.CustomerId == customerId &&
                       (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing) &&
                       !s.IsDeleted)
            .OrderByDescending(s => s.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        if (subscription == null)
            throw new ArgumentNullException(nameof(subscription));

        if (subscription.CustomerId <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(subscription));

        subscription.CreatedOn = DateTime.UtcNow;
        subscription.CreatedBy = "System"; // TODO: Replace with actual user context
        
        var result = await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return result;
    }

    public async Task<Subscription> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        if (subscription == null)
            throw new ArgumentNullException(nameof(subscription));

        if (subscription.Id <= 0)
            throw new ArgumentException("Invalid subscription ID", nameof(subscription));

        subscription.ModifiedOn = DateTime.UtcNow;
        subscription.ModifiedBy = "System"; // TODO: Replace with actual user context
        
        _subscriptionRepository.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return subscription;
    }

    public async Task<Subscription> CancelAsync(int id, bool cancelAtPeriodEnd = true, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid subscription ID", nameof(id));

        var subscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (subscription == null)
            throw new InvalidOperationException($"Subscription with ID {id} not found");

        subscription.CancelAtPeriodEnd = cancelAtPeriodEnd;
        if (!cancelAtPeriodEnd)
        {
            subscription.Status = SubscriptionStatus.Canceled;
        }
        subscription.CanceledAt = DateTime.UtcNow;
        subscription.ModifiedOn = DateTime.UtcNow;
        subscription.ModifiedBy = "System"; // TODO: Replace with actual user context

        _subscriptionRepository.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return subscription;
    }

    public async Task<Subscription> ReactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid subscription ID", nameof(id));

        var subscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (subscription == null)
            throw new InvalidOperationException($"Subscription with ID {id} not found");

        if (subscription.Status != SubscriptionStatus.Canceled)
            throw new InvalidOperationException("Only canceled subscriptions can be reactivated");

        subscription.Status = SubscriptionStatus.Active;
        subscription.CancelAtPeriodEnd = false;
        subscription.CanceledAt = null;
        subscription.ModifiedOn = DateTime.UtcNow;
        subscription.ModifiedBy = "System"; // TODO: Replace with actual user context

        _subscriptionRepository.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return subscription;
    }

    public async Task<(IEnumerable<Subscription> Subscriptions, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20,
        SubscriptionStatus? status = null,
        SubscriptionTier? tier = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

        IQueryable<Subscription> query = _context.Subscriptions
            .Where(s => !s.IsDeleted)
            .Include(s => s.Customer);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (tier.HasValue)
            query = query.Where(s => s.Tier == tier.Value);

        query = query.OrderByDescending(s => s.CreatedOn);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var subscriptions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (subscriptions, totalCount);
    }
}
