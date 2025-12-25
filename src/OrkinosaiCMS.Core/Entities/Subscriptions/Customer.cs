using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Subscriptions;

/// <summary>
/// Represents a Stripe customer record.
/// Redesigned from Mosaic with improved validation and data integrity.
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// User ID associated with this customer (foreign key to ApplicationUser)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Stripe customer ID (unique identifier from Stripe)
    /// </summary>
    public string StripeCustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Customer email address (synced with Stripe)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Customer phone number (optional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Default payment method ID from Stripe
    /// </summary>
    public string? DefaultPaymentMethodId { get; set; }

    /// <summary>
    /// Customer's currency (default: "usd")
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Customer's address line 1
    /// </summary>
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Customer's address line 2
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Customer's city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Customer's state/province
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Customer's postal code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Customer's country (ISO 3166-1 alpha-2 code)
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Navigation property to subscriptions
    /// </summary>
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    /// <summary>
    /// Navigation property to payment methods
    /// </summary>
    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
