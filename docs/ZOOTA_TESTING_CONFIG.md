# Zoota Testing Configuration

## Overview

The `ZootaTesting` configuration section controls the visibility and accessibility of the Zoota Test Page and automated JSON test runner in OrkinosaiCMS. This feature is designed for R&D, prototyping, and pre-production testing, and should be disabled in production environments.

## Test Files Location

Test files are stored in the `tests/zoota-tests` folder in the repository. The test runner automatically loads all `.json` files from this folder and displays them for selection.

### Pre-included Test Files

The repository includes sample test files:
- `sample-test.json` - Basic test example with page creation and content verification
- `smoke-test.json` - Comprehensive smoke test with multiple operations

You can add your own test files to this folder to expand your test suite.

## Configuration

### appsettings.json

Add the following section to your `appsettings.json`:

```json
{
  "ZootaTesting": {
    "Enabled": true,
    "_note": "Set to true during R&D and pre-production to enable testing features. Set to false in production to hide all test functionality."
  }
}
```

### Configuration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Enabled` | `bool` | `true` | Controls whether Zoota Test Page and test runner are accessible |

## Behavior

### When Enabled = true

- **Admin Navigation**: A "🧪 Zoota Test" menu item appears in the admin sidebar
- **Test Page Access**: Users can navigate to `/admin/zoota-test` to access the test runner
- **Test Execution**: Administrators can upload and execute JSON-based test suites
- **Test Results**: Full test execution results and reporting are available

### When Enabled = false

- **Admin Navigation**: The "Zoota Test" menu item is hidden from the admin sidebar
- **Test Page Access**: Navigating to `/admin/zoota-test` shows a disabled message
- **Security**: Test functionality is not exposed to production users
- **Clean UI**: Production admins see a streamlined admin panel without test features

## Usage Scenarios

### Development & R&D (Enabled = true)

Use this setting during:
- Local development
- Feature prototyping
- Integration testing
- Staging environments
- Pre-production validation

### Production (Enabled = false)

Disable this feature when:
- Deploying to production
- Releasing to customers
- Going live with the public site
- Operating in a production environment

## Zoota Test Page Features

When enabled, the Zoota Test Page provides:

### 1. Test File Selection
- Browse and select test files from the `tests/zoota-tests` folder
- Multiple file selection with checkboxes
- "Select All" option to quickly select all available tests
- Clear selection button
- View test descriptions loaded from JSON files

### 2. JSON Test File Upload
- Upload custom `.json` files containing test suites
- Alternative to using pre-defined test files
- Preview uploaded test configuration
- Validate test structure before execution

### 3. Automated Test Execution
- Execute multiple test files in sequence
- Run selected tests with a single click
- Real-time progress updates
- Detailed logging for each step
- Shows which file each test step came from

### 4. Test Results Reporting
- Pass/Fail status for each test
- Summary statistics (total, passed, failed)
- Detailed error messages and diagnostics
- Visual result indicators
- Source file tracking for each result

### 5. Supported Test Actions

The test runner currently supports these actions:

| Action | Description | Parameters |
|--------|-------------|------------|
| `createPage` | Create a new CMS page | `name`: Page title |
| `addContent` | Add content to a page | `pageName`: Target page, `content`: Content body |
| `uploadImage` | Upload an image to a page | `pageName`: Target page, `imagePath`: Image file path |
| `verifyContent` | Verify content exists on a page | `pageName`: Target page |

## Example Test JSON

Create a `tests.json` file with the following structure:

```json
{
  "testSuite": "Stage 1 CMS Smoke Tests",
  "steps": [
    {
      "action": "createPage",
      "params": { "name": "DemoTestPage" },
      "expect": { "created": true }
    },
    {
      "action": "addContent",
      "params": { 
        "pageName": "DemoTestPage", 
        "content": "Welcome to the CMS!" 
      },
      "expect": { "contentExists": true }
    },
    {
      "action": "verifyContent",
      "params": { "pageName": "DemoTestPage" },
      "expect": { "contentExists": true }
    }
  ]
}
```

## Environment Variables

You can override the configuration using environment variables:

```bash
# Azure App Service / Container
ZootaTesting__Enabled=false

# Docker
-e ZootaTesting__Enabled=false

# Local development (.env)
ZootaTesting__Enabled=true
```

## Security Considerations

### Production Deployment

⚠️ **IMPORTANT**: Always set `ZootaTesting:Enabled` to `false` before deploying to production.

Test features should never be exposed in production environments because:
- They provide administrative capabilities that could be exploited
- They may execute operations that could modify production data
- They are not designed for production-level security hardening
- They may expose internal system details

### Access Control

Even when enabled, the Zoota Test Page:
- Requires admin authentication (when auth is enabled)
- Uses the same security context as other admin features
- Should only be accessible to trusted administrators

## Troubleshooting

### Test Page Not Visible

**Symptom**: Can't see the "Zoota Test" menu item in admin navigation

**Solution**: Check that `ZootaTesting:Enabled` is set to `true` in your `appsettings.json`

### Test Page Shows "Disabled" Message

**Symptom**: Can access `/admin/zoota-test` but see a disabled message

**Solution**: The configuration is correctly set to `false`. Set it to `true` to enable test features.

### Configuration Not Taking Effect

**Symptom**: Changed `appsettings.json` but behavior hasn't changed

**Solution**: 
1. Restart the application
2. Check for environment variable overrides
3. Verify you're editing the correct `appsettings.json` file
4. Clear browser cache and reload

## Related Documentation

- [Zoota AI Assistant User Guide](ZOOTA_USER_GUIDE.md) - Using the Zoota chat assistant
- [Application Settings Guide](appsettings.md) - Complete configuration reference
- [Deployment Checklist](DEPLOYMENT_CHECKLIST.md) - Production deployment procedures
- [Setup Guide](SETUP.md) - Initial setup and configuration

## API Reference

### ZootaTestingOptions Class

Located at: `OrkinosaiCMS.Web.Models.ZootaTestingOptions`

```csharp
public class ZootaTestingOptions
{
    public const string SectionName = "ZootaTesting";
    public bool Enabled { get; set; } = true;
}
```

### Dependency Injection

The configuration is registered in `Program.cs`:

```csharp
builder.Services.Configure<ZootaTestingOptions>(
    builder.Configuration.GetSection(ZootaTestingOptions.SectionName));
```

### Usage in Components

Inject the options into your Razor components:

```razor
@inject IOptions<ZootaTestingOptions> ZootaTestingOptions

@if (ZootaTestingOptions.Value.Enabled)
{
    <!-- Test features visible -->
}
```

## Best Practices

1. **Default to Enabled in Development**: Keep `Enabled: true` during development for easier testing
2. **Disable Before Production**: Always set `Enabled: false` before production deployment
3. **Use Environment Variables**: Override settings per environment without modifying config files
4. **Document Your Tests**: Maintain a library of test JSON files for regression testing
5. **Version Control**: Keep test JSON files in source control for team collaboration
6. **CI/CD Integration**: Use automated tests in your deployment pipeline

## Future Enhancements

Planned improvements to the Zoota Testing feature:

- [ ] Additional test actions (user management, module installation)
- [ ] Test scheduling and automation
- [ ] Integration with CI/CD pipelines
- [ ] Test coverage reporting
- [ ] Performance benchmarking
- [ ] Database snapshot/restore for testing
- [ ] Multi-environment test execution

---

For questions or issues related to Zoota Testing, please refer to the [main documentation](../README.md) or open an issue on GitHub.
