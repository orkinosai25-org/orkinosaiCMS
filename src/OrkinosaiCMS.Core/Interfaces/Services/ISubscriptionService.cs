using OrkinosaiCMS.Core.Entities.Subscriptions;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for managing subscriptions.
/// Redesigned from Mosaic with improved validation and state management.
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Get subscription by ID
    /// </summary>
    Task<Subscription?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get subscription by Stripe subscription ID
    /// </summary>
    Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all subscriptions for a customer
    /// </summary>
    Task<IEnumerable<Subscription>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active subscription for a customer
    /// </summary>
    Task<Subscription?> GetActiveSubscriptionAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new subscription
    /// </summary>
    Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing subscription
    /// </summary>
    Task<Subscription> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a subscription (immediate or at period end)
    /// </summary>
    Task<Subscription> CancelAsync(int id, bool cancelAtPeriodEnd = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivate a canceled subscription
    /// </summary>
    Task<Subscription> ReactivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all subscriptions (with pagination and filtering)
    /// </summary>
    Task<(IEnumerable<Subscription> Subscriptions, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20,
        SubscriptionStatus? status = null,
        SubscriptionTier? tier = null,
        CancellationToken cancellationToken = default);
}
