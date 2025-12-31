using System.Security.Claims;

namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Separate Oqtane-based authentication service
/// Inspired by Oqtane CMS authentication logic
/// Completely isolated from the main authentication system
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
    private OqtaneUserSession? _currentSession;

    public OqtaneAuthService(ILogger<OqtaneAuthService> logger)
    {
        _logger = logger;
    }

    public bool IsAuthenticated => _currentSession != null;

    /// <summary>
    /// Authenticate user using Oqtane-style logic
    /// For demo purposes, accepts any username with password "oqtane123"
    /// In a real implementation, this would validate against a separate user store
    /// </summary>
    public Task<bool> AuthenticateAsync(string username, string password)
    {
        _logger.LogInformation("Oqtane Auth: Authentication attempt for user {Username}", username);

        // Oqtane-style authentication logic
        // Demo implementation: accept any username with specific password
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Oqtane Auth: Empty username or password");
            return Task.FromResult(false);
        }

        // Simple authentication for demo - in real scenario, this would check against Oqtane user database
        if (password == "oqtane123")
        {
            _currentSession = new OqtaneUserSession
            {
                UserId = GenerateUserId(username),
                Username = username,
                DisplayName = $"Oqtane User - {username}",
                Email = $"{username}@oqtane.local",
                AuthenticatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Oqtane Auth: User {Username} authenticated successfully", username);
            return Task.FromResult(true);
        }

        _logger.LogWarning("Oqtane Auth: Authentication failed for user {Username}", username);
        return Task.FromResult(false);
    }

    public Task<OqtaneUserSession?> GetCurrentOqtaneUserAsync()
    {
        return Task.FromResult(_currentSession);
    }

    public Task LogoutAsync()
    {
        _logger.LogInformation("Oqtane Auth: User {Username} logged out", _currentSession?.Username ?? "Unknown");
        _currentSession = null;
        return Task.CompletedTask;
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
/// Separate from the main authentication system
/// </summary>
public class OqtaneUserSession
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AuthenticatedAt { get; set; }
}
