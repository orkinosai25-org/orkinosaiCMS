# Oqtane-Based Login Mechanism

## Overview

This document describes the Oqtane-based login mechanism implemented in OrkinosaiCMS. This authentication system is **the primary and only authentication method** for OrkinosaiCMS, providing a unified login experience inspired by Oqtane CMS architecture.

> **Important**: As of the latest update, this is the **only authentication system** in OrkinosaiCMS. The previous separate login systems have been removed, and all login routes (`/login` and `/admin/login`) now redirect to `/oqtane-login`.

## Purpose

This implementation demonstrates:
- **Unified Authentication Flow**: Single authentication system for all users
- **Oqtane Architecture Reference**: Authentication logic inspired by Oqtane CMS framework
- **Role-Based Access Control**: Automatic role assignment based on username patterns
- **Integrated Experience**: Uses JWT tokens and integrates with `AuthorizeView` components

## Features

### 1. Primary Login Flow
- Main login page at `/oqtane-login`
- All other login routes (`/login`, `/admin/login`) redirect here automatically
- Fully integrated authentication system via JWT tokens
- Uses `OqtaneAuthService` which updates `CustomAuthenticationStateProvider`
- Admin menu and settings visible when logged in as administrator

### 2. Role-Based Authentication
- **Administrator Role**: Assigned when username contains "admin" (case-insensitive)
- **User Role**: Assigned to all other usernames
- Roles integrate with Blazor's `<AuthorizeView>` components
- Admin users see "⚙️ Admin Panel" link in navigation

### 3. Hello World Confirmation
- Upon successful login, displays a dedicated "Hello World" page at `/oqtane-hello`
- Shows authenticated user information including role
- Provides clear visual confirmation of successful authentication
- Admin users see "Admin Panel" button on this page

### 4. Navigation Integration
- **Single "🚀 Oqtane Login" link** in the main navigation header when not authenticated
- After login, shows "Welcome, {username}" message
- Admin users see "⚙️ Admin Panel" link
- "Logout" link available when authenticated
- Distinctive purple gradient styling for Oqtane login button

### 5. Session Management
- JWT-based authentication tokens
- Integrated with `CustomAuthenticationStateProvider`
- Session persists across page navigation
- Proper logout clears authentication state

## File Structure

All Oqtane login components are organized in clearly separated locations:

```
src/OrkinosaiCMS.Web/
├── Services/
│   └── OqtaneAuthService.cs          # Oqtane authentication service
├── Components/
│   ├── Pages/
│   │   └── OqtaneAuth/               # Oqtane authentication pages
│   │       ├── OqtaneLogin.razor     # Login page
│   │       └── OqtaneHelloWorld.razor # Success/Hello World page
│   └── Shared/
│       └── CMSNavigation.razor        # Updated with Oqtane login link
└── Program.cs                         # Service registration
```

## Usage

### Accessing the Oqtane Login

1. Navigate to the home page (`/cms-home` or `/`)
2. Click the "🚀 Oqtane Login" link in the navigation header
3. You'll be redirected to `/oqtane-login`

### Demo Credentials

The Oqtane authentication system accepts:
- **Password**: `oqtane123` (required for all users)
- **Admin Access**: Use username containing "admin" (e.g., `admin`, `testadmin`, `myadmin`)
- **Regular User**: Any username without "admin" (e.g., `john`, `testuser`)

This is a demo implementation. In a production scenario, credentials would be validated against a proper user database.

### After Successful Login

Upon successful authentication:
- **Admin Users** (username contains "admin"): Redirected directly to `/admin` - the admin dashboard
  - No intermediate "Hello World" page
  - Immediate access to admin panel features
  - Single sign-in experience
- **Regular Users**: Redirected to `/oqtane-hello` - the Hello World page
  - Animated success checkmark
  - Welcome message with "Hello World!"
  - User session details (username, email, user ID, login time)
  - System information about the authentication mechanism
  - Attribution to Oqtane CMS

### Logout

- **Admin Users**: Use the "Logout" button in the admin sidebar to log out
- **Regular Users**: On the Hello World page, click the "Logout" button

Both logout options will:
- Clear your Oqtane session
- Return to the Oqtane login page

## Technical Details

### OqtaneAuthService

The `OqtaneAuthService` provides:
- `AuthenticateAsync()`: Validates credentials, determines role, creates JWT token, and updates authentication state
- `GetCurrentOqtaneUserAsync()`: Retrieves current user session
- `LogoutAsync()`: Clears the user session and authentication state
- `IsAuthenticated`: Property indicating authentication status

The service integrates with `CustomAuthenticationStateProvider` to ensure authentication works across the entire application.

### OqtaneUserSession

Session object containing:
- `UserId`: Generated from username hash
- `Username`: User's login name
- `DisplayName`: Formatted display name
- `Email`: Generated email address
- `Role`: "Administrator" or "User" based on username
- `AuthenticatedAt`: Timestamp of authentication

### Integration with Main System

The Oqtane authentication is **fully integrated** with the main system:
- Uses `CustomAuthenticationStateProvider` for authentication state management
- Creates JWT tokens via `IJwtTokenService`
- Sessions work with Blazor's `<AuthorizeView>` components
- Admin roles grant access to admin-only features
- Single logout clears all authentication state

## Attribution

This authentication mechanism is inspired by [Oqtane CMS Framework](https://github.com/oqtane/oqtane.framework), an open-source modular application framework for Blazor.

### Oqtane CMS

Oqtane is a modular application framework built on .NET and Blazor that provides:
- Modular architecture for building web applications
- Extensibility through a plugin-based system
- Multi-tenant capabilities
- Rich content management features

This implementation borrows architectural concepts from Oqtane, including:
- Separation of concerns in authentication
- Service-based authentication patterns
- Clean component structure

**License**: Oqtane Framework is licensed under the MIT License
**Project URL**: https://github.com/oqtane/oqtane.framework
**Documentation**: https://docs.oqtane.org

## Customization

### Changing Authentication Logic

To modify the authentication logic, edit `OqtaneAuthService.cs`:

```csharp
public Task<bool> AuthenticateAsync(string username, string password)
{
    // Add your custom authentication logic here
    // Example: validate against a database, API, or directory service
}
```

### Customizing the Hello World Page

Edit `OqtaneHelloWorld.razor` to:
- Change the displayed message
- Add additional user information
- Customize styling and animations

### Modifying Credentials

The demo password is defined in `OqtaneAuthService.cs`:

```csharp
// Change this line to use a different demo password
if (password == "oqtane123")
```

## Security Considerations

⚠️ **Important**: This is a demonstration implementation for learning purposes.

For production use, you should:
1. **Implement proper credential validation** against a secure user database
2. **Hash and salt passwords** - never store or compare plain text passwords
3. **Use HTTPS** for all authentication traffic
4. **Implement rate limiting** to prevent brute force attacks
5. **Add CSRF protection** for authentication forms
6. **Use secure JWT token storage** with appropriate expiration times
7. **Implement account lockout** after failed login attempts
8. **Add multi-factor authentication** for enhanced security
9. **Validate and sanitize all user inputs**
10. **Implement proper role-based access control validation**

## Authentication System

**Oqtane Authentication is now the only authentication system in OrkinosaiCMS.**

| Feature | Details |
|---------|---------|
| Login URL | `/oqtane-login` (all other login routes redirect here) |
| Service | `OqtaneAuthService` |
| Session Storage | JWT tokens via `CustomAuthenticationStateProvider` |
| Demo Password | `oqtane123` |
| Success Page | Hello World page at `/oqtane-hello` |
| User Authentication | Demo-only (username pattern matching) |
| Navigation Link | "🚀 Oqtane Login" |
| Role Assignment | Pattern-based (username contains "admin" = Administrator, otherwise User) |
| Integration | Fully integrated with entire system |

> **Migration Note**: The previous separate authentication systems have been removed. All authentication now flows through the Oqtane system for consistency and simplicity.

## Testing

### Manual Testing

1. **Test Admin Login Flow**:
   ```
   1. Visit /oqtane-login
   2. Enter username: admin (or any username containing "admin")
   3. Enter password: oqtane123
   4. Click "Sign In with Oqtane"
   5. Verify redirect directly to /admin (Admin Dashboard)
   6. Verify no intermediate "Hello World" page
   7. Verify admin panel is accessible
   ```

2. **Test Regular User Login Flow**:
   ```
   1. Visit /oqtane-login
   2. Enter username: testuser (any username without "admin")
   3. Enter password: oqtane123
   4. Click "Sign In with Oqtane"
   5. Verify redirect to /oqtane-hello
   6. Verify "Hello World!" message displays
   ```

3. **Test Authentication Required**:
   ```
   1. Visit /oqtane-hello directly (without logging in)
   2. Verify "Authentication Required" message shows
   3. Click "Go to Oqtane Login"
   4. Verify redirect to /oqtane-login
   ```

4. **Test Admin Logout**:
   ```
   1. Log in as admin
   2. In admin panel sidebar, click "Logout"
   3. Verify redirect to /oqtane-login
   4. Try visiting /admin again
   5. Verify redirect to /oqtane-login (auth required)
   ```

5. **Test Navigation Link**:
   ```
   1. Visit home page
   2. Verify "🚀 Oqtane Login" link appears in navigation (when not logged in)
   3. Click the link
   4. Verify redirect to /oqtane-login
   ```

### Invalid Credentials Testing

Try logging in with wrong password:
- Username: testuser
- Password: wrongpassword
- Expected: Error message "Invalid username or password"

## Future Enhancements

Potential improvements for this module:
1. Integrate with actual Oqtane user database
2. Add persistent session storage (database or distributed cache)
3. Implement JWT tokens for Oqtane sessions
4. Add "Remember Me" functionality
5. Support for OAuth providers (Google, Microsoft, etc.)
6. Add user profile management page
7. Implement role-based access control
8. Add password reset functionality

## Troubleshooting

### "Authentication Required" message on Hello World page

**Cause**: You're not logged in via Oqtane authentication

**Solution**: Go to `/oqtane-login` and log in with valid credentials

### Login button doesn't work

**Cause**: Form validation failed or service not registered

**Solution**: 
1. Check console for errors
2. Verify `OqtaneAuthService` is registered in `Program.cs`
3. Ensure username is at least 3 characters
4. Ensure password is at least 6 characters

### Navigation link not appearing

**Cause**: Already authenticated

**Solution**: The login link only appears when you're not authenticated. Log out to see the login link.

## Contributing

When extending this authentication system:
1. Ensure authentication remains integrated with the main system
2. Maintain clear attribution to Oqtane
3. Follow existing code patterns and styling
4. Update this documentation with changes
5. Test thoroughly before committing

## Support

For issues related to this module:
- Check the troubleshooting section above
- Review the code comments in service and component files
- Consult Oqtane documentation for architecture patterns: https://docs.oqtane.org

For Oqtane CMS questions:
- Visit: https://github.com/oqtane/oqtane.framework
- Documentation: https://docs.oqtane.org

---

**Last Updated**: December 31, 2024
**Module Version**: 1.0.0
**OrkinosaiCMS Version**: 1.0.0
