using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrkinosaiCMS.Web.Services;

/// <summary>
/// Implementation of JWT token service for admin authentication
/// Follows Oqtane's JWT/token model: token auth, configurable secret, claims, Blazor-compatible
/// Supports auto-generation of signing keys when config is missing
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Auto-provision JWT signing key if missing (for dev/test/failsafe mode)
        var configuredSecret = _configuration["Jwt:Secret"];
        _secretKey = string.IsNullOrWhiteSpace(configuredSecret) ? GenerateDefaultSecret() : configuredSecret;
        _issuer = _configuration["Jwt:Issuer"] ?? "OrkinosaiCMS";
        _audience = _configuration["Jwt:Audience"] ?? "OrkinosaiCMS";
        
        if (!int.TryParse(_configuration["Jwt:ExpirationMinutes"], out _expirationMinutes))
        {
            _expirationMinutes = 480; // Default 8 hours
        }

        if (_secretKey == GenerateDefaultSecret())
        {
            _logger.LogWarning("JWT Secret not configured. Using auto-generated key. THIS IS NOT SECURE FOR PRODUCTION!");
        }
    }

    public string GenerateToken(int userId, string username, string email, string displayName, string role, bool isFailsafeMode = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim("DisplayName", displayName),
            new Claim(ClaimTypes.Role, role)
        };

        // Mark failsafe mode in claims for warning banner
        if (isFailsafeMode)
        {
            claims.Add(new Claim("FailsafeMode", "true"));
            _logger.LogWarning("JWT token generated in FAILSAFE mode for user: {Username}", username);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Token validation failed");
            return null;
        }
    }

    public bool IsKeyConfigured()
    {
        var configuredSecret = _configuration["Jwt:Secret"];
        return !string.IsNullOrEmpty(configuredSecret);
    }

    /// <summary>
    /// Generates a default secret key for development/testing
    /// WARNING: This should NEVER be used in production
    /// </summary>
    private static string GenerateDefaultSecret()
    {
        // Generate a consistent but secure-enough key for dev/test
        // Uses a fixed GUID so it persists across app restarts in dev
        return "OrkinosaiCMS-Dev-Secret-Key-DO-NOT-USE-IN-PRODUCTION-" + Guid.Parse("12345678-1234-1234-1234-123456789012").ToString();
    }
}
