using Microsoft.AspNetCore.Components.Authorization;
using OrkinosaiCMS.Core.Interfaces.Services;

namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Service for handling authentication operations with JWT tokens
/// Supports failsafe mode when database/config is unavailable
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    // Failsafe credentials - hardcoded for emergency access
    private const string FAILSAFE_USERNAME = "admin";
    private const string FAILSAFE_PASSWORD = "password123";

    public AuthenticationService(
        AuthenticationStateProvider authStateProvider,
        IUserService userService,
        IRoleService roleService,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        _userService = userService;
        _roleService = roleService;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            _logger.LogInformation("Login attempt started for username: {Username}", username);
            
            // Check if failsafe mode is enabled
            var failsafeModeEnabled = _configuration.GetValue<bool>("Authentication:FailsafeMode:Enabled", true);
            
            // First, try normal authentication if database is available
            bool isDatabaseAvailable = await IsDatabaseAvailableAsync();
            
            _logger.LogInformation("Database availability check result: {IsAvailable}", isDatabaseAvailable);
            
            if (isDatabaseAvailable)
            {
                _logger.LogInformation("Attempting normal authentication for user: {Username}", username);
                
                // Normal authentication flow
                var isValid = await _userService.VerifyPasswordAsync(username, password);
                
                _logger.LogInformation("Password verification result for {Username}: {IsValid}", username, isValid);
                
                if (isValid)
                {
                    var user = await _userService.GetByUsernameAsync(username);
                    if (user != null && user.IsActive)
                    {
                        _logger.LogInformation("User found and active: {Username}, UserId: {UserId}", username, user.Id);
                        
                        // Get user's primary role
                        var userRoles = await _userService.GetUserRolesAsync(user.Id);
                        var primaryRole = "User";
                        
                        if (userRoles != null && userRoles.Any())
                        {
                            primaryRole = userRoles.First().Name;
                            _logger.LogInformation("User {Username} assigned role: {Role}", username, primaryRole);
                        }
                        else
                        {
                            _logger.LogWarning("User {Username} has no roles assigned", username);
                        }

                        // Generate JWT token (not failsafe mode)
                        var token = _jwtTokenService.GenerateToken(
                            user.Id, 
                            user.Username, 
                            user.Email, 
                            user.DisplayName, 
                            primaryRole,
                            isFailsafeMode: false);

                        // Create user session with JWT token
                        var userSession = new UserSession
                        {
                            UserId = user.Id,
                            Username = user.Username,
                            Email = user.Email,
                            DisplayName = user.DisplayName,
                            Role = primaryRole,
                            JwtToken = token,
                            IsFailsafeMode = false
                        };

                        // Update authentication state
                        await _authStateProvider.UpdateAuthenticationState(userSession);

                        // Update last login
                        await _userService.UpdateLastLoginAsync(user.Id);

                        _logger.LogInformation("User {Username} logged in successfully", username);
                        return true;
                    }
                    else if (user != null && !user.IsActive)
                    {
                        _logger.LogWarning("User {Username} exists but is not active", username);
                    }
                    else
                    {
                        _logger.LogWarning("User {Username} not found after password verification succeeded", username);
                    }
                }
                else
                {
                    _logger.LogWarning("Password verification failed for user: {Username}", username);
                }
                
                // If normal auth failed and failsafe is enabled, try failsafe
                if (failsafeModeEnabled && username == FAILSAFE_USERNAME && password == FAILSAFE_PASSWORD)
                {
                    _logger.LogWarning("Normal authentication failed, attempting failsafe login");
                    return await LoginWithFailsafeAsync();
                }
                
                _logger.LogWarning("Login failed for user {Username}: Invalid credentials", username);
                return false;
            }
            else
            {
                // Database unavailable - use failsafe mode if enabled
                _logger.LogWarning("Database unavailable. Attempting failsafe authentication.");
                
                if (failsafeModeEnabled && username == FAILSAFE_USERNAME && password == FAILSAFE_PASSWORD)
                {
                    return await LoginWithFailsafeAsync();
                }
                
                _logger.LogError("Login failed: Database unavailable and failsafe credentials incorrect or failsafe mode disabled");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during login for user {Username}. Exception: {ExceptionType}, Message: {Message}", 
                username, ex.GetType().Name, ex.Message);
            
            // Try failsafe as last resort if enabled
            // Note: Exception is re-thrown only for development error display
            // The middleware checks environment/config before showing detailed errors
            var failsafeModeEnabled = _configuration.GetValue<bool>("Authentication:FailsafeMode:Enabled", true);
            if (failsafeModeEnabled && username == FAILSAFE_USERNAME && password == FAILSAFE_PASSWORD)
            {
                _logger.LogWarning("Exception occurred, attempting failsafe login as last resort");
                return await LoginWithFailsafeAsync();
            }
            
            throw; // Re-throw for development error display - middleware will handle security
        }
    }

    private async Task<bool> LoginWithFailsafeAsync()
    {
        _logger.LogWarning("FAILSAFE MODE: Admin login with hardcoded credentials");
        
        // Generate JWT token with failsafe flag
        var token = _jwtTokenService.GenerateToken(
            userId: 9999, // Special ID for failsafe admin
            username: FAILSAFE_USERNAME,
            email: "admin@failsafe.local",
            displayName: "Failsafe Administrator",
            role: "Administrator",
            isFailsafeMode: true);

        var userSession = new UserSession
        {
            UserId = 9999,
            Username = FAILSAFE_USERNAME,
            Email = "admin@failsafe.local",
            DisplayName = "Failsafe Administrator",
            Role = "Administrator",
            JwtToken = token,
            IsFailsafeMode = true
        };

        await _authStateProvider.UpdateAuthenticationState(userSession);
        
        return true;
    }

    private async Task<bool> IsDatabaseAvailableAsync()
    {
        try
        {
            // Simple check: try to query users
            var users = await _userService.GetAllAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.UpdateAuthenticationState(null);
    }

    public async Task<UserSession?> GetCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return null;
        }

        // Extract required claims
        var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            // Missing or invalid user ID claim - authentication is invalid
            return null;
        }

        return new UserSession
        {
            UserId = userId,
            Username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? string.Empty,
            Email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty,
            DisplayName = user.FindFirst("DisplayName")?.Value ?? string.Empty,
            Role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User"
        };
    }
}
