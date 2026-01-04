# Migrate Media Library to Azure Blob Storage

## Summary
Migrate the Media Library from local file system storage to Azure Blob Storage for improved scalability, reliability, and multi-server support.

## Background
The current Media Library implementation uses local file system storage in `wwwroot/uploads/` which works well for single-server deployments. However, for production environments with multiple servers, load balancing, or cloud deployments, Azure Blob Storage provides better scalability and reliability.

## Current Implementation
- **Storage Location**: `wwwroot/uploads/` (local file system)
- **Thumbnails**: `wwwroot/uploads/thumbnails/`
- **File Naming**: GUID-based unique filenames
- **Service**: `MediaService` in `OrkinosaiCMS.Infrastructure/Services/MediaService.cs`
- **Interface**: Stream-based API maintaining clean architecture

## Proposed Changes

### 1. Configuration (appsettings.json)
Add Azure Blob Storage configuration section:

```json
{
  "MediaStorage": {
    "Provider": "AzureBlob",  // or "FileSystem" for backward compatibility
    "AzureBlobStorage": {
      "ConnectionString": "",  // From Key Vault in production
      "ContainerName": "media-library",
      "ThumbnailContainerName": "media-thumbnails",
      "UseManagedIdentity": true,  // Recommended for production
      "AccountName": "orkinosaicms"
    },
    "FileSystem": {
      "UploadPath": "wwwroot/uploads",
      "ThumbnailPath": "wwwroot/uploads/thumbnails"
    }
  }
}
```

### 2. Create Storage Abstraction
Create `IMediaStorageProvider` interface:

```csharp
public interface IMediaStorageProvider
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType);
    Task<string> UploadThumbnailAsync(Stream stream, string fileName);
    Task<Stream> DownloadFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
}
```

### 3. Implement Storage Providers

**FileSystemStorageProvider** (existing behavior):
- Store files in `wwwroot/uploads/`
- Direct file system access
- For single-server deployments

**AzureBlobStorageProvider** (new):
- Use `Azure.Storage.Blobs` SDK
- Support for Managed Identity authentication
- Connection string fallback for development
- Container-based organization
- SAS token generation for secure file access

### 4. Update MediaService
Inject `IMediaStorageProvider` instead of direct file system access:

```csharp
public class MediaService : IMediaService
{
    private readonly IMediaStorageProvider _storageProvider;
    // ... other dependencies
    
    public MediaService(
        IMediaStorageProvider storageProvider,
        // ... other parameters
    )
    {
        _storageProvider = storageProvider;
        // ...
    }
}
```

### 5. Service Registration (Program.cs)
Configure storage provider based on appsettings:

```csharp
var storageProvider = builder.Configuration["MediaStorage:Provider"];
if (storageProvider == "AzureBlob")
{
    builder.Services.AddScoped<IMediaStorageProvider, AzureBlobStorageProvider>();
}
else
{
    builder.Services.AddScoped<IMediaStorageProvider, FileSystemStorageProvider>();
}
```

### 6. Migration Strategy
For existing deployments with files in local storage:

- Create migration script to upload existing files to Azure Blob Storage
- Update database records with new blob URLs
- Provide rollback capability
- Support gradual migration (new files to blob, old files remain local temporarily)

## Benefits

### Scalability
- ✅ Support for multiple web servers
- ✅ No shared file system required
- ✅ Automatic load balancing across Azure regions

### Reliability
- ✅ Geo-redundant storage (RAGRS)
- ✅ 99.99% availability SLA
- ✅ Automatic backup and disaster recovery

### Performance
- ✅ CDN integration for faster content delivery
- ✅ Tiered storage (hot/cool/archive)
- ✅ Parallel upload/download

### Security
- ✅ Encryption at rest (Microsoft-managed keys)
- ✅ TLS 1.2 encryption in transit
- ✅ SAS tokens for time-limited access
- ✅ Managed Identity (no credentials in code)
- ✅ Private endpoints for network isolation

### Cost Optimization
- ✅ Pay only for storage used
- ✅ Lifecycle management (auto-delete old thumbnails)
- ✅ Tiered storage for rarely accessed files

## Implementation Checklist

### Phase 1: Infrastructure
- [ ] Add `Azure.Storage.Blobs` NuGet package
- [ ] Create `IMediaStorageProvider` interface
- [ ] Implement `FileSystemStorageProvider` (refactor existing code)
- [ ] Implement `AzureBlobStorageProvider`
- [ ] Add configuration section to appsettings.json
- [ ] Update service registration in Program.cs

### Phase 2: Service Updates
- [ ] Update `MediaService` to use `IMediaStorageProvider`
- [ ] Update URL generation for blob storage
- [ ] Handle blob-specific errors (connection, authentication)
- [ ] Add retry logic for transient failures
- [ ] Update logging for blob operations

### Phase 3: Testing
- [ ] Unit tests for storage providers
- [ ] Integration tests with Azure Storage Emulator/Azurite
- [ ] Test file upload/download/delete
- [ ] Test thumbnail generation with blob storage
- [ ] Test provider switching (FileSystem ↔ AzureBlob)
- [ ] Performance testing (large files, concurrent uploads)

### Phase 4: Migration & Deployment
- [ ] Create migration script for existing files
- [ ] Document migration process
- [ ] Test rollback procedure
- [ ] Update deployment documentation
- [ ] Add Azure resources to infrastructure-as-code (Terraform/ARM)

### Phase 5: Documentation
- [ ] Update README with blob storage setup
- [ ] Create configuration guide
- [ ] Document managed identity setup for Azure
- [ ] Add troubleshooting guide
- [ ] Update API documentation

## Dependencies
- `Azure.Storage.Blobs` (v12.x)
- `Azure.Identity` (for Managed Identity support)

## Security Considerations
- ⚠️ Never commit connection strings to source control
- ⚠️ Use Azure Key Vault for connection strings in production
- ⚠️ Enable Managed Identity for Azure-hosted applications
- ⚠️ Configure CORS policies on blob storage
- ⚠️ Set appropriate blob access levels (private by default)
- ⚠️ Implement SAS token expiration (e.g., 1 hour)

## Breaking Changes
None - backward compatible if FileSystem provider is set as default.

Existing deployments can continue using file system storage until migrated.

## Alternatives Considered
1. **AWS S3**: Good alternative but Azure Blob is preferred for Azure-hosted deployments
2. **Keep FileSystem only**: Not suitable for scaled/multi-server deployments
3. **Hybrid approach**: Support both providers (recommended - implemented)

## Timeline Estimate
- Phase 1-2: 2-3 days
- Phase 3: 1-2 days
- Phase 4-5: 1 day
- **Total**: ~1 week

## Related Documentation
- [Azure Blob Storage Documentation](AZURE_BLOB_STORAGE.md)
- [Media Library Implementation](../src/OrkinosaiCMS.Web/Components/Pages/Admin/Media.razor)
- [Azure Storage Best Practices](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction)

## Acceptance Criteria
- [ ] Files can be uploaded to Azure Blob Storage
- [ ] Thumbnails are generated and stored in separate container
- [ ] URLs are updated to point to blob storage
- [ ] File deletion removes blobs from storage
- [ ] Provider can be switched via configuration
- [ ] Existing file system storage still works
- [ ] Migration script successfully moves files to blob
- [ ] Performance is equal or better than file system
- [ ] All existing tests pass
- [ ] New tests added for blob storage provider
- [ ] Documentation is complete

## Notes
- Consider implementing this for v2.0 release
- May want to batch this with other storage-related improvements (quotas, CDN)
- Useful for multi-tenant SaaS scenarios mentioned in docs

## How to Use This Document
This document can be used as a GitHub issue template. To create an issue from this:

1. Go to the GitHub repository
2. Click "Issues" → "New Issue"
3. Copy the entire content of this file
4. Paste it into the issue description
5. Add appropriate labels: `enhancement`, `storage`, `azure`, `media-library`
6. Assign to appropriate milestone (e.g., v2.0)
