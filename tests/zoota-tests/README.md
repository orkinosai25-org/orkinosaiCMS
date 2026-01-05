# Zoota Test Files

This folder contains JSON test files for the Zoota Test Runner.

## Deployment

**Important:** The test files from `src/OrkinosaiCMS.Web/tests/zoota-tests/` are automatically included in the deployment package. These files will be available on the server without requiring GitHub access.

When you deploy the application, the test files are copied to the output directory and will be accessible by the Zoota Test Runner.

## File Format

Each test file should be a JSON file with the following structure:

```json
{
  "testSuite": "Test Suite Name",
  "steps": [
    {
      "action": "actionName",
      "params": { },
      "expect": { }
    }
  ]
}
```

## Supported Actions

- `createPage` - Create a new CMS page
- `addContent` - Add content to a page
- `uploadImage` - Upload an image to a page
- `verifyContent` - Verify content exists on a page

## Example Files

- `sample-test.json` - Basic test example
- `smoke-test.json` - Comprehensive smoke test

## Usage

1. Navigate to `/admin/zoota-test` in the admin panel
2. Select one or more test files from the list
3. Click "Run Selected Tests" to execute

## Adding New Tests

To add new test files:
1. Create a new `.json` file in `src/OrkinosaiCMS.Web/tests/zoota-tests/`
2. Follow the JSON structure above
3. The file will automatically appear in the test runner on next deployment

