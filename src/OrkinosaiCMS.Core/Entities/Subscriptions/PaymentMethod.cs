using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Subscriptions;

/// <summary>
/// Represents a payment method for a customer.
/// Redesigned from Mosaic with improved security and data validation.
/// </summary>
public class PaymentMethod : BaseEntity
{
    /// <summary>
    /// Customer ID (foreign key)
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Stripe payment method ID (unique identifier from Stripe)
    /// </summary>
    public string StripePaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// Payment method type (e.g., "card", "bank_account")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Card brand (e.g., "visa", "mastercard", "amex")
    /// Only applicable for card type
    /// </summary>
    public string? CardBrand { get; set; }

    /// <summary>
    /// Last 4 digits of card (for display purposes only)
    /// Only applicable for card type
    /// </summary>
    public string? CardLast4 { get; set; }

    /// <summary>
    /// Card expiration month (1-12)
    /// Only applicable for card type
    /// </summary>
    public int? CardExpMonth { get; set; }

    /// <summary>
    /// Card expiration year
    /// Only applicable for card type
    /// </summary>
    public int? CardExpYear { get; set; }

    /// <summary>
    /// Whether this is the default payment method for the customer
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Navigation property to customer
    /// </summary>
    public Customer? Customer { get; set; }
}
