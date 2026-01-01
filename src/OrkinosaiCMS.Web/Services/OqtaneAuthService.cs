using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Separate Oqtane-based authentication service
/// Inspired by Oqtane CMS authentication logic
/// Now integrated with the main authentication system
/// </summary>
public interface IOqtaneAuthService
{
    Task<bool> AuthenticateAsync(string username, string password);
    Task<OqtaneUserSession?> GetCurrentOqtaneUserAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}

public class OqtaneAuthService : IOqtaneAuthService
{
    private readonly ILogger<OqtaneAuthService> _logger;
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private OqtaneUserSession? _currentSession;

    public OqtaneAuthService(
        ILogger<OqtaneAuthService> logger,
        AuthenticationStateProvider authStateProvider,
        IJwtTokenService jwtTokenService)
    {
        _logger = logger;
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        _jwtTokenService = jwtTokenService;
    }

    public bool IsAuthenticated
    {
        get
        {
            // Check authentication state from the provider instead of relying on in-memory session
            var authState = _authStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }
    }

    /// <summary>
    /// Authenticate user using Oqtane-style logic
    /// For demo purposes, accepts any username with password "oqtane123"
    /// Creates proper authentication state with JWT token and claims
    /// Admins get administrator role, others get user role
    /// </summary>
    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        _logger.LogInformation("Oqtane Auth: Authentication attempt for user {Username}", username);

        // Oqtane-style authentication logic
        // Demo implementation: accept any username with specific password
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Oqtane Auth: Empty username or password");
            return false;
        }

        // Simple authentication for demo - in real scenario, this would check against Oqtane user database
        if (password == "oqtane123")
        {
            var userId = GenerateUserId(username);
            var displayName = $"Oqtane User - {username}";
            var email = $"{username}@oqtane.local";
            
            // Determine role - if username contains "admin" (case insensitive), grant Administrator role
            var role = username.Contains("admin", StringComparison.OrdinalIgnoreCase) 
                ? "Administrator" 
                : "User";

            _currentSession = new OqtaneUserSession
            {
                UserId = userId,
                Username = username,
                DisplayName = displayName,
                Email = email,
                Role = role,
                AuthenticatedAt = DateTime.UtcNow
            };

            // Generate JWT token for this Oqtane user
            var jwtToken = _jwtTokenService.GenerateToken(
                userId: userId,
                username: username,
                email: email,
                displayName: displayName,
                role: role,
                isFailsafeMode: false);

            // Create user session for the main authentication system
            var userSession = new UserSession
            {
                UserId = userId,
                Username = username,
                Email = email,
                DisplayName = displayName,
                Role = role,
                JwtToken = jwtToken,
                IsFailsafeMode = false
            };

            // Update authentication state so the user is logged in across the entire app
            await _authStateProvider.UpdateAuthenticationState(userSession);

            _logger.LogInformation("Oqtane Auth: User {Username} authenticated successfully with role {Role}", username, role);
            return true;
        }

        _logger.LogWarning("Oqtane Auth: Authentication failed for user {Username}", username);
        return false;
    }

    public async Task<OqtaneUserSession?> GetCurrentOqtaneUserAsync()
    {
        // If we have an in-memory session, return it
        if (_currentSession != null)
        {
            return _currentSession;
        }

        // Otherwise, retrieve from authentication state claims
        try
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = user.FindFirst(ClaimTypes.Name)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var displayName = user.FindFirst("DisplayName")?.Value;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(username))
                {
                    _currentSession = new OqtaneUserSession
                    {
                        UserId = int.TryParse(userId, out var id) ? id : 0,
                        Username = username,
                        DisplayName = displayName ?? username,
                        Email = email ?? $"{username}@oqtane.local",
                        Role = role ?? "User",
                        AuthenticatedAt = DateTime.UtcNow
                    };

                    return _currentSession;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current Oqtane user from authentication state");
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("Oqtane Auth: User {Username} logged out", _currentSession?.Username ?? "Unknown");
        _currentSession = null;
        
        // Clear authentication state in the main system
        await _authStateProvider.UpdateAuthenticationState(null);
    }

    private int GenerateUserId(string username)
    {
        // Generate a consistent user ID based on username hash
        // In real implementation, this would come from database
        return Math.Abs(username.GetHashCode() % 10000) + 10000;
    }
}

/// <summary>
/// Represents an Oqtane user session
/// Now integrated with the main authentication system
/// </summary>
public class OqtaneUserSession
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime AuthenticatedAt { get; set; }
}
