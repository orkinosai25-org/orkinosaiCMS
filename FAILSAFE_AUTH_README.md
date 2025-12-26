# JWT-Based Admin Authentication with Failsafe Mode

## Overview

OrkinosaiCMS implements a robust JWT-based authentication system inspired by Oqtane's approach, with an additional **failsafe mode** that ensures admin access is always available, even when the database or configuration is unavailable.

## Features

### JWT Authentication
- **Token-based authentication** using industry-standard JWT tokens
- **Configurable signing keys** with auto-generation for dev/test environments
- **Claims-based authorization** compatible with Blazor authentication state
- **Configurable token expiration** (default: 8 hours)
- **Secure token validation** with issuer and audience verification

### Failsafe Mode
- **Emergency admin access** when database is unavailable or misconfigured
- **Hardcoded super admin credentials** for initial setup and troubleshooting
- **Visual warning banner** in admin area when running in failsafe mode
- **Automatic fallback** when database operations fail
- **Easy to enable/disable** via configuration

## Configuration

### JWT Settings

Add these settings to `appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "OrkinosaiCMS",
    "Audience": "OrkinosaiCMS",
    "ExpirationMinutes": 480
  }
}
```

**Important Notes:**
- `Secret` should be at least 32 characters for production
- If `Secret` is empty or missing, a development key is auto-generated (NOT SECURE for production!)
- Use environment variables or Azure Key Vault for production secrets

### Failsafe Mode Settings

```json
{
  "Authentication": {
    "FailsafeMode": {
      "Enabled": true
    }
  }
}
```

## Failsafe Mode Credentials

**Important:** Failsafe mode now uses the same credentials as the default demo account for consistency:
- **Username:** `admin`
- **Password:** `Admin@123`

This means you only need to remember one set of credentials. Whether the database is available or not, these credentials will work (when failsafe mode is enabled).

⚠️ **Security Warning:** These credentials are hardcoded in the application. They provide full administrator access and should ONLY be used for:
1. Initial setup on a fresh installation
2. Emergency access when the database is down
3. Troubleshooting configuration issues

**How It Works:**
- When database is available: Credentials authenticate against the database (normal mode)
- When database is unavailable: Credentials fall back to failsafe mode
- Authentication is seamless - users don't need to know which mode is active

## Usage

### Normal Operation (Database Available)

1. Users log in with their regular credentials
2. JWT token is generated with user claims from database
3. Token is stored in protected session storage
4. All admin routes are protected with JWT validation
5. Token expires after configured duration

### Failsafe Mode (Database Unavailable)

1. System detects database is unavailable
2. Login accepts failsafe credentials (`admin` / `Admin@123`)
3. JWT token is generated with failsafe flag
4. Red warning banner appears in admin area
5. Full admin access is granted for troubleshooting

### How to Check Current Mode

Look for the warning banner at the top of the admin panel:
- **No banner:** Normal operation with database
- **Red banner:** Failsafe mode is active

## Disabling Failsafe Mode for Production

### Step 1: Update Configuration

Set `Enabled` to `false` in `appsettings.Production.json`:

```json
{
  "Authentication": {
    "FailsafeMode": {
      "Enabled": false
    }
  }
}
```

### Step 2: Set JWT Secret

**Option A - Environment Variables (Recommended):**
```bash
export Jwt__Secret="your-production-secret-key-minimum-32-chars"
```

**Option B - Azure Key Vault (Best for Production):**
```json
{
  "KeyVault": {
    "VaultUri": "https://your-keyvault.vault.azure.net/",
    "Secrets": {
      "JwtSecret": "jwt-secret"
    }
  }
}
```

### Step 3: Ensure Database is Configured

1. Verify connection string in `appsettings.Production.json`
2. Run database migrations: `dotnet ef database update`
3. Seed admin users using the database seeding process
4. Test login with actual user credentials

### Step 4: Verify

1. Deploy to production
2. Try logging in with failsafe credentials - should FAIL
3. Try logging in with actual user credentials - should SUCCEED
4. Verify no warning banner appears

## Testing Authentication

### Test Normal Login (Database Available)

1. Start the application: `dotnet run`
2. Navigate to `/admin/login`
3. Enter credentials from database (e.g., `admin` / `Admin@123`)
4. Should redirect to admin dashboard
5. No warning banner should appear

### Test Failsafe Login (Database Unavailable)

**Method 1 - Stop Database:**
1. Stop your SQL Server instance
2. Navigate to `/admin/login`
3. Enter failsafe credentials: `admin` / `Admin@123`
4. Should redirect to admin dashboard
5. Red warning banner should appear

**Method 2 - Wrong Connection String:**
1. Temporarily change connection string to invalid value
2. Restart application
3. Navigate to `/admin/login`
4. Enter failsafe credentials: `admin` / `Admin@123`
5. Should redirect to admin dashboard
6. Red warning banner should appear

### Test Logout

1. Click "Logout" in admin sidebar
2. Should redirect to login page
3. Session should be cleared
4. Accessing `/admin` should redirect to login

## Security Considerations

### Production Deployment Checklist

- [ ] Disable failsafe mode (`Authentication:FailsafeMode:Enabled` = `false`)
- [ ] Set strong JWT secret (minimum 32 characters, use Key Vault)
- [ ] Configure proper database connection string
- [ ] Seed database with admin users
- [ ] Test login with actual credentials
- [ ] Verify failsafe credentials are rejected
- [ ] Enable HTTPS only
- [ ] Configure appropriate token expiration
- [ ] Set up log monitoring for failed login attempts

### Why Failsafe Mode is Safe

1. **Visible Warning:** Red banner clearly indicates non-standard operation
2. **Logged:** All failsafe logins are logged with warnings
3. **Database Flag:** JWT token contains `FailsafeMode` claim
4. **Easy to Disable:** Single configuration setting
5. **No Persistence:** Failsafe doesn't create database records

## Troubleshooting

### "Invalid username or password" with failsafe credentials

**Cause:** Failsafe mode is disabled in configuration

**Solution:** 
1. Check `appsettings.json` → `Authentication:FailsafeMode:Enabled`
2. Set to `true` if you need emergency access
3. Restart application

### Warning banner won't go away

**Cause:** Still logged in with failsafe credentials

**Solution:**
1. Click "Logout"
2. Fix database/configuration issues
3. Log in with normal credentials
4. Banner should disappear

### JWT token keeps expiring

**Cause:** Token expiration set too short

**Solution:**
1. Check `appsettings.json` → `Jwt:ExpirationMinutes`
2. Increase value (default: 480 = 8 hours)
3. Users will need to re-login after change

### Cannot access admin area after disabling failsafe

**Cause:** No valid admin users in database

**Solution:**
1. Enable failsafe mode temporarily
2. Log in with failsafe credentials
3. Create proper admin user through UI
4. Log out and back in with new credentials
5. Disable failsafe mode

## Architecture Details

### JWT Token Claims

The JWT token includes these claims:
- `NameIdentifier`: User ID
- `Name`: Username
- `Email`: User email
- `DisplayName`: User display name
- `Role`: User role (e.g., "Administrator")
- `FailsafeMode`: "true" if logged in via failsafe (optional)

### Token Flow

1. **Login:** User submits credentials
2. **Validation:** Check database (or failsafe if DB unavailable)
3. **Token Generation:** Create JWT with user claims
4. **Storage:** Store in protected session storage
5. **Validation:** Each request validates JWT
6. **Refresh:** Token valid until expiration
7. **Logout:** Clear session storage

### Database Availability Check

The system checks database availability by attempting to query users. If this fails:
1. Database is marked as unavailable
2. Failsafe mode activates (if enabled)
3. System logs warning
4. Warning banner displays (after login)

## Files Modified/Created

- `Services/IJwtTokenService.cs` - Interface for JWT operations
- `Services/JwtTokenService.cs` - JWT token generation and validation
- `Services/AuthenticationService.cs` - Updated with failsafe logic
- `Services/CustomAuthenticationStateProvider.cs` - JWT validation
- `Components/Shared/FailsafeWarningBanner.razor` - Warning banner component
- `Components/Layout/Admin/AdminLayout.razor` - Added banner
- `Components/Pages/Admin/Login.razor` - Added failsafe hint
- `Program.cs` - JWT middleware configuration
- `appsettings.json` - JWT and failsafe settings
- `FAILSAFE_AUTH_README.md` - This documentation

## References

- [Oqtane CMS](https://github.com/oqtane/oqtane.framework) - Inspiration for JWT approach
- [JWT.io](https://jwt.io/) - JWT standard documentation
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
