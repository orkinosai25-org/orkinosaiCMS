namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for Stripe payment integration.
/// Redesigned from Mosaic with comprehensive error handling and validation.
/// </summary>
public interface IStripeService
{
    /// <summary>
    /// Create a Stripe customer
    /// </summary>
    Task<string> CreateCustomerAsync(string email, string name, string? phone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Stripe customer by ID
    /// </summary>
    Task<object?> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update Stripe customer
    /// </summary>
    Task<bool> UpdateCustomerAsync(string customerId, string? email = null, string? name = null, string? phone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a subscription in Stripe
    /// </summary>
    Task<string> CreateSubscriptionAsync(string customerId, string priceId, int? trialDays = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Stripe subscription by ID
    /// </summary>
    Task<object?> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a subscription in Stripe
    /// </summary>
    Task<bool> CancelSubscriptionAsync(string subscriptionId, bool cancelAtPeriodEnd = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update subscription (e.g., change plan)
    /// </summary>
    Task<bool> UpdateSubscriptionAsync(string subscriptionId, string newPriceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivate a canceled subscription
    /// </summary>
    Task<bool> ReactivateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a payment method
    /// </summary>
    Task<string> CreatePaymentMethodAsync(string customerId, string paymentMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set default payment method for customer
    /// </summary>
    Task<bool> SetDefaultPaymentMethodAsync(string customerId, string paymentMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payment methods for a customer
    /// </summary>
    Task<IEnumerable<object>> GetPaymentMethodsAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detach (remove) a payment method
    /// </summary>
    Task<bool> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices for a customer
    /// </summary>
    Task<IEnumerable<object>> GetInvoicesAsync(string customerId, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoice by ID
    /// </summary>
    Task<object?> GetInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify webhook signature (for security)
    /// </summary>
    bool VerifyWebhookSignature(string payload, string signature, string secret);

    /// <summary>
    /// Process webhook event
    /// </summary>
    Task<bool> ProcessWebhookEventAsync(string eventJson, CancellationToken cancellationToken = default);
}
