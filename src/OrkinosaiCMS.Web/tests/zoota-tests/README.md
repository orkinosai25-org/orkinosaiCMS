# Zoota Test Files (Deployment Copy)

This folder contains test files that are **included in the deployment package**.

## Automatic Deployment

These test files are configured to be copied to the server during deployment:
- Included in build output (`bin/Debug` or `bin/Release`)
- Included in publish output (deployment package)
- Available on the server without requiring GitHub access

## Configuration

The test files are included via the project file (`OrkinosaiCMS.Web.csproj`):
```xml
<ItemGroup>
  <Content Update="tests\zoota-tests\*.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

## Adding New Test Files

1. Add your `.json` test file to this folder
2. Build/publish the project
3. The file will automatically be included in the deployment

No additional configuration needed - all `.json` files in this folder are automatically included.

## Test File Format

See the parent repository's `tests/zoota-tests/README.md` for detailed information about test file format and usage.
