using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Subscriptions;

/// <summary>
/// Represents a subscription for a customer.
/// Redesigned from Mosaic with improved status tracking and validation.
/// </summary>
public class Subscription : BaseEntity
{
    /// <summary>
    /// Customer ID (foreign key)
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Stripe subscription ID (unique identifier from Stripe)
    /// </summary>
    public string StripeSubscriptionId { get; set; } = string.Empty;

    /// <summary>
    /// Subscription tier (Free, Starter, Pro, Business)
    /// </summary>
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;

    /// <summary>
    /// Subscription status (Active, Canceled, PastDue, etc.)
    /// </summary>
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    /// <summary>
    /// Billing interval (Monthly or Yearly)
    /// </summary>
    public BillingInterval BillingInterval { get; set; } = BillingInterval.Monthly;

    /// <summary>
    /// Price amount in cents (e.g., 1200 = $12.00)
    /// </summary>
    public int PriceAmount { get; set; }

    /// <summary>
    /// Currency code (e.g., "usd", "eur", "gbp")
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Current billing period start date (UTC)
    /// </summary>
    public DateTime CurrentPeriodStart { get; set; }

    /// <summary>
    /// Current billing period end date (UTC)
    /// </summary>
    public DateTime CurrentPeriodEnd { get; set; }

    /// <summary>
    /// Whether subscription will cancel at period end
    /// </summary>
    public bool CancelAtPeriodEnd { get; set; }

    /// <summary>
    /// Date when subscription was canceled (UTC, if applicable)
    /// </summary>
    public DateTime? CanceledAt { get; set; }

    /// <summary>
    /// Date when subscription trial ends (UTC, if applicable)
    /// </summary>
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Stripe price ID for this subscription (links to Stripe pricing)
    /// </summary>
    public string? StripePriceId { get; set; }

    /// <summary>
    /// Navigation property to customer
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Navigation property to invoices
    /// </summary>
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
