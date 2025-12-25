using OrkinosaiCMS.Core.Common;

namespace OrkinosaiCMS.Core.Entities.Subscriptions;

/// <summary>
/// Represents an invoice for a subscription.
/// Redesigned from Mosaic with improved tracking and audit trail.
/// </summary>
public class Invoice : BaseEntity
{
    /// <summary>
    /// Subscription ID (foreign key)
    /// </summary>
    public int SubscriptionId { get; set; }

    /// <summary>
    /// Stripe invoice ID (unique identifier from Stripe)
    /// </summary>
    public string StripeInvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable invoice number (from Stripe)
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Invoice status (draft, open, paid, uncollectible, void)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Invoice amount due in cents
    /// </summary>
    public int AmountDue { get; set; }

    /// <summary>
    /// Amount paid in cents
    /// </summary>
    public int AmountPaid { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Invoice PDF URL (from Stripe)
    /// </summary>
    public string? InvoicePdfUrl { get; set; }

    /// <summary>
    /// Hosted invoice URL for customer to view/pay (from Stripe)
    /// </summary>
    public string? HostedInvoiceUrl { get; set; }

    /// <summary>
    /// Date invoice was finalized (UTC)
    /// </summary>
    public DateTime? FinalizedAt { get; set; }

    /// <summary>
    /// Date invoice was paid (UTC)
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Due date for invoice (UTC)
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Navigation property to subscription
    /// </summary>
    public Subscription? Subscription { get; set; }
}
