using Microsoft.AspNetCore.Identity;

namespace OrkinosaiCMS.Core.Entities.Identity;

/// <summary>
/// ApplicationUser extends IdentityUser to integrate with ASP.NET Core Identity.
/// This provides robust authentication, password management, and security features.
/// Redesigned from Mosaic with improved error handling and data consistency.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// Display name for the user
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Profile picture URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Last login date (UTC)
    /// </summary>
    public DateTime? LastLoginOn { get; set; }

    /// <summary>
    /// Last IP address used for login (for security auditing)
    /// </summary>
    public string? LastIPAddress { get; set; }

    /// <summary>
    /// Stripe customer ID (if user has subscription)
    /// </summary>
    public string? StripeCustomerId { get; set; }

    /// <summary>
    /// Current subscription tier
    /// Maps to SubscriptionTier enum: Free = 0, Starter = 1, Pro = 2, Business = 3
    /// </summary>
    public int SubscriptionTierValue { get; set; } = 0;

    /// <summary>
    /// Whether the user account is deleted (soft delete for data retention)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Created timestamp (UTC)
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Modified timestamp (UTC)
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Deleted timestamp (UTC)
    /// </summary>
    public DateTime? DeletedOn { get; set; }
}
