using OrkinosaiCMS.Core.Entities.Subscriptions;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for managing customers and Stripe integration.
/// Redesigned from Mosaic with improved error handling.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Get customer by ID
    /// </summary>
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customer by user ID
    /// </summary>
    Task<Customer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customer by Stripe customer ID
    /// </summary>
    Task<Customer?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new customer
    /// </summary>
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing customer
    /// </summary>
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a customer (soft delete)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all customers (with pagination)
    /// </summary>
    Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllAsync(
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
}
