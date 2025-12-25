using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using OrkinosaiCMS.Core.Interfaces.Services;

namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Custom authentication state provider for admin users with JWT token validation
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(
        ProtectedSessionStorage sessionStorage,
        IUserService userService,
        IJwtTokenService jwtTokenService,
        ILogger<CustomAuthenticationStateProvider> logger)
    {
        _sessionStorage = sessionStorage;
        _userService = userService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
            if (!userSessionResult.Success || userSessionResult.Value == null)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            var userSession = userSessionResult.Value;
            
            // Validate JWT token
            if (!string.IsNullOrEmpty(userSession.JwtToken))
            {
                var principal = _jwtTokenService.ValidateToken(userSession.JwtToken);
                if (principal != null)
                {
                    // JWT is valid
                    // If not failsafe mode, verify user still exists and is active
                    if (!userSession.IsFailsafeMode)
                    {
                        try
                        {
                            var user = await _userService.GetByIdAsync(userSession.UserId);
                            if (user == null || !user.IsActive || user.IsDeleted)
                            {
                                _logger.LogWarning("User {UserId} is no longer valid, invalidating session", userSession.UserId);
                                return await Task.FromResult(new AuthenticationState(_anonymous));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not verify user {UserId}, database may be unavailable", userSession.UserId);
                            // If database is unavailable but JWT is valid, allow access
                        }
                    }
                    
                    return await Task.FromResult(new AuthenticationState(principal));
                }
            }
            
            // Token invalid or missing
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(UserSession? userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession != null)
        {
            await _sessionStorage.SetAsync("UserSession", userSession);
            
            // Validate JWT and create principal
            if (!string.IsNullOrEmpty(userSession.JwtToken))
            {
                var principal = _jwtTokenService.ValidateToken(userSession.JwtToken);
                claimsPrincipal = principal ?? _anonymous;
            }
            else
            {
                claimsPrincipal = _anonymous;
            }
        }
        else
        {
            await _sessionStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    // Keep for backward compatibility but not used with JWT
    private ClaimsPrincipal CreateClaimsPrincipal(UserSession userSession)
    {
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
            new Claim(ClaimTypes.Name, userSession.Username),
            new Claim(ClaimTypes.Email, userSession.Email),
            new Claim("DisplayName", userSession.DisplayName),
            new Claim(ClaimTypes.Role, userSession.Role)
        }, "CustomAuth");

        if (userSession.IsFailsafeMode)
        {
            claimsIdentity.AddClaim(new Claim("FailsafeMode", "true"));
        }

        return new ClaimsPrincipal(claimsIdentity);
    }
}

public class UserSession
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string JwtToken { get; set; } = string.Empty;
    public bool IsFailsafeMode { get; set; }
}
