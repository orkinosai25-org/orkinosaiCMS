using OrkinosaiCMS.Core.Entities.Media;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service for managing media library files and folders
/// </summary>
public interface IMediaService
{
    // Folder operations
    /// <summary>
    /// Get all folders
    /// </summary>
    Task<IEnumerable<MediaFolder>> GetAllFoldersAsync();

    /// <summary>
    /// Get folder by ID
    /// </summary>
    Task<MediaFolder?> GetFolderByIdAsync(int id);

    /// <summary>
    /// Get root folders (folders without parent)
    /// </summary>
    Task<IEnumerable<MediaFolder>> GetRootFoldersAsync();

    /// <summary>
    /// Get child folders of a parent folder
    /// </summary>
    Task<IEnumerable<MediaFolder>> GetChildFoldersAsync(int parentFolderId);

    /// <summary>
    /// Create a new folder
    /// </summary>
    Task<MediaFolder> CreateFolderAsync(string name, string? description, int? parentFolderId);

    /// <summary>
    /// Update folder
    /// </summary>
    Task<MediaFolder> UpdateFolderAsync(MediaFolder folder);

    /// <summary>
    /// Delete folder (soft delete)
    /// </summary>
    Task DeleteFolderAsync(int id);

    // File operations
    /// <summary>
    /// Get all media files
    /// </summary>
    Task<IEnumerable<MediaFile>> GetAllFilesAsync();

    /// <summary>
    /// Get files in a specific folder
    /// </summary>
    Task<IEnumerable<MediaFile>> GetFilesByFolderAsync(int? folderId);

    /// <summary>
    /// Get media file by ID
    /// </summary>
    Task<MediaFile?> GetFileByIdAsync(int id);

    /// <summary>
    /// Search media files by name or tags
    /// </summary>
    Task<IEnumerable<MediaFile>> SearchFilesAsync(string searchTerm);

    /// <summary>
    /// Upload a new media file
    /// </summary>
    Task<MediaFile> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, int? folderId, string? title, string? description, string? tags);

    /// <summary>
    /// Update media file metadata
    /// </summary>
    Task<MediaFile> UpdateFileAsync(MediaFile file);

    /// <summary>
    /// Delete media file (soft delete and remove physical file)
    /// </summary>
    Task DeleteFileAsync(int id);

    /// <summary>
    /// Get total storage used in bytes
    /// </summary>
    Task<long> GetTotalStorageUsedAsync();

    /// <summary>
    /// Get file count
    /// </summary>
    Task<int> GetFileCountAsync();
}
