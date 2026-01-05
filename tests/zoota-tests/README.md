# Zoota Test Files

This folder contains JSON test files for the Zoota Test Runner.

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
