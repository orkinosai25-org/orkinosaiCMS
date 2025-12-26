# Error/Exception Harness Implementation for Development

## Overview

OrkinosaiCMS now includes a comprehensive error and exception harness system inspired by best practices from modern CMS platforms (mosaic/mosaic-saas-new repositories). This system provides full detailed error information during development while maintaining user-friendly error pages in production.

## Features

### 1. Development Exception Middleware
**File:** `src/OrkinosaiCMS.Web/Middleware/DevelopmentExceptionMiddleware.cs`

A beautiful, detailed error page that displays:
- Full exception details (type, message)
- Complete stack traces with syntax highlighting
- Request information (path, method, headers)
- Inner exceptions with full context
- Exception data dictionary
- Collapsible sections for better readability
- Dark theme optimized for developer eyes

### 2. Blazor Error Boundary Component
**File:** `src/OrkinosaiCMS.Web/Components/Shared/DevelopmentErrorBoundary.razor`

A reusable Blazor component that:
- Catches errors in Blazor components
- Shows detailed error information in development
- Displays friendly error message in production
- Provides "Recover" and "Reload" actions
- Supports nested inner exceptions

### 3. Enhanced Login Error Display
**File:** `src/OrkinosaiCMS.Web/Components/Pages/Admin/Login.razor`

The login page now:
- Shows basic error messages to all users
- Displays detailed debugging information in development mode
- Includes stack traces for exceptions
- Provides troubleshooting hints
- Logs all authentication attempts with full context

### 4. Verbose Authentication Logging
**File:** `src/OrkinosaiCMS.Web/Services/AuthenticationService.cs`

Enhanced logging includes:
- Login attempt tracking
- Database availability checks
- Password verification results
- User state validation
- Role assignment verification
- Exception details with re-throw for development

## Configuration

### appsettings.json

```json
{
  "ErrorHandling": {
    "ShowDetailedErrors": false,
    "IncludeStackTrace": false
  }
}
```

### appsettings.Development.json

```json
{
  "ErrorHandling": {
    "ShowDetailedErrors": true,
    "IncludeStackTrace": true
  }
}
```

## How It Works

### Development Mode
When `ASPNETCORE_ENVIRONMENT=Development` or `ErrorHandling:ShowDetailedErrors=true`:

1. **HTTP Middleware Errors:**
   - Middleware catches all unhandled exceptions
   - Generates beautiful HTML error page with full details
   - Shows exception type, message, stack trace
   - Displays request information and headers
   - Lists all inner exceptions recursively

2. **Blazor Component Errors:**
   - Error boundary catches component exceptions
   - Displays detailed error information inline
   - Shows exception chain and stack traces
   - Provides recovery options

3. **Login Errors:**
   - Basic error: "Invalid username or password"
   - Detailed section shows:
     - Username attempted
     - Timestamp
     - Possible causes
     - Demo and failsafe credentials
   - Exception details with stack trace
   - Inner exception information

### Production Mode
When `ASPNETCORE_ENVIRONMENT=Production` and `ErrorHandling:ShowDetailedErrors=false`:

1. **HTTP Middleware Errors:**
   - Redirects to `/Error` page
   - Shows user-friendly message
   - No sensitive information exposed

2. **Blazor Component Errors:**
   - Shows friendly "Something went wrong" message
   - Provides reload button
   - No technical details visible

3. **Login Errors:**
   - Only shows: "Invalid username or password"
   - No debugging information
   - Clean, professional presentation

## Usage Examples

### Wrap Blazor Components with Error Boundary

```razor
@page "/my-page"

<DevelopmentErrorBoundary>
    <MyComponent />
</DevelopmentErrorBoundary>
```

### Testing the Error Harness

#### Test Login Errors
1. Start the application in development mode
2. Navigate to `/admin/login`
3. Enter invalid credentials
4. You should see detailed error information including:
   - Error message
   - Development details section
   - Possible causes
   - Demo credentials hint

#### Test Component Errors
Create a test component that throws an exception:

```razor
@page "/test-error"

<DevelopmentErrorBoundary>
    <button @onclick="ThrowError">Trigger Error</button>
</DevelopmentErrorBoundary>

@code {
    private void ThrowError()
    {
        throw new InvalidOperationException("Test exception for error harness");
    }
}
```

#### Test Middleware Errors
Add a test endpoint that throws:

```csharp
app.MapGet("/test-middleware-error", () =>
{
    throw new Exception("Test middleware error");
});
```

Navigate to `/test-middleware-error` to see the detailed error page.

## Logging Integration

All errors are automatically logged with full context:

```csharp
_logger.LogError(exception, 
    "Unhandled exception occurred. Path: {Path}, Method: {Method}, QueryString: {QueryString}",
    context.Request.Path,
    context.Request.Method,
    context.Request.QueryString);
```

Authentication service provides verbose logging:
- Login attempt started
- Database availability check
- Password verification result
- User state validation
- Role assignment
- Success/failure with reasons

## Security Considerations

### ✅ Safe Practices
- Detailed errors only in development mode
- Configuration-based control
- User-friendly messages in production
- Sensitive data not logged
- Stack traces excluded in production

### ⚠️ Important Notes
1. **Never enable detailed errors in production** - It exposes sensitive information
2. **Review logs carefully** - Ensure no credentials are logged
3. **Test both modes** - Verify production mode shows friendly errors
4. **Monitor for PII** - Check that personal information isn't exposed

## Troubleshooting

### Detailed Errors Not Showing in Development

**Check:**
1. Environment variable: `ASPNETCORE_ENVIRONMENT=Development`
2. Configuration: `ErrorHandling:ShowDetailedErrors=true`
3. Middleware registration in `Program.cs`

### Production Still Shows Detailed Errors

**Fix:**
1. Set `ASPNETCORE_ENVIRONMENT=Production`
2. Update `appsettings.Production.json`:
   ```json
   {
     "ErrorHandling": {
       "ShowDetailedErrors": false,
       "IncludeStackTrace": false
     }
   }
   ```
3. Restart the application

### Login Errors Not Detailed Enough

**Enable more logging:**
```json
{
  "Logging": {
    "LogLevel": {
      "OrkinosaiCMS.Web.Services.AuthenticationService": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

## Files Modified/Created

### New Files
- `src/OrkinosaiCMS.Web/Middleware/DevelopmentExceptionMiddleware.cs`
- `src/OrkinosaiCMS.Web/Components/Shared/DevelopmentErrorBoundary.razor`
- `docs/ERROR_HARNESS.md` (this file)

### Modified Files
- `src/OrkinosaiCMS.Web/Program.cs` - Added middleware registration
- `src/OrkinosaiCMS.Web/Components/Pages/Admin/Login.razor` - Enhanced error display
- `src/OrkinosaiCMS.Web/Services/AuthenticationService.cs` - Added verbose logging
- `src/OrkinosaiCMS.Web/appsettings.Development.json` - Enabled detailed errors

## Benefits

1. **Faster Debugging** - See full error details instantly
2. **Better DX** - Beautiful, readable error pages
3. **Complete Context** - Request info, headers, inner exceptions
4. **Production Safe** - Automatically disabled in production
5. **Flexible** - Configuration-based control
6. **Comprehensive** - Covers middleware, Blazor, and authentication

## Comparison with Standard ASP.NET Error Handling

| Feature | Standard ASP.NET | OrkinosaiCMS Error Harness |
|---------|------------------|----------------------------|
| Exception type | ✅ Basic | ✅ Full with namespace |
| Stack trace | ✅ Plain text | ✅ Formatted with highlighting |
| Inner exceptions | ⚠️ Limited | ✅ Full chain with details |
| Request info | ⚠️ Basic | ✅ Complete with headers |
| Visual design | ❌ Plain HTML | ✅ Beautiful dark theme |
| Blazor errors | ⚠️ Generic | ✅ Component-specific |
| Login errors | ❌ None | ✅ Full authentication context |
| Production mode | ✅ Yes | ✅ User-friendly fallback |

## Inspired By

This error harness implementation is inspired by:
- Mosaic CMS error handling patterns
- Mosaic-SaaS-New development tools
- Modern developer experience practices
- ASP.NET Core diagnostic middleware
- Blazor error boundary component

## Future Enhancements

Potential improvements:
- [ ] Error search and filtering
- [ ] Copy error details to clipboard
- [ ] Send error report via email
- [ ] Error statistics dashboard
- [ ] Integration with error tracking services (Sentry, Raygun)
- [ ] Performance profiling integration
- [ ] Database query logging in error context

## Support

For issues or questions about the error harness:
1. Check this documentation
2. Review the implementation files
3. Enable debug logging
4. Check application logs in `App_Data/Logs/`
5. Open an issue on GitHub with error details

---

**Remember:** The error harness is a development tool. Always test in production mode before deploying!
