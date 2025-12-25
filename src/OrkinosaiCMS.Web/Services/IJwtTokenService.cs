namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Service for JWT token generation and validation
/// Follows Oqtane's JWT/token model for Blazor-compatible authentication
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="username">Username</param>
    /// <param name="email">User email</param>
    /// <param name="displayName">User display name</param>
    /// <param name="role">User role</param>
    /// <param name="isFailsafeMode">Whether this is a failsafe login</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(int userId, string username, string email, string displayName, string role, bool isFailsafeMode = false);

    /// <summary>
    /// Validates a JWT token and returns the claims principal
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Checks if the JWT signing key is configured or auto-generated
    /// </summary>
    /// <returns>True if key is available</returns>
    bool IsKeyConfigured();
}
