using OrkinosaiCMS.Core.Entities.Media;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service for managing media library files and folders
/// </summary>
public class MediaService : IMediaService
{
    private readonly IRepository<MediaFile> _fileRepository;
    private readonly IRepository<MediaFolder> _folderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _webRootPath;
    private const string MediaDirectory = "uploads";
    private const string ThumbnailDirectory = "uploads/thumbnails";
    private const int ThumbnailMaxWidth = 300;
    private const int ThumbnailMaxHeight = 300;

    public MediaService(
        IRepository<MediaFile> fileRepository,
        IRepository<MediaFolder> folderRepository,
        IUnitOfWork unitOfWork,
        string webRootPath)
    {
        _fileRepository = fileRepository;
        _folderRepository = folderRepository;
        _unitOfWork = unitOfWork;
        _webRootPath = webRootPath;
    }

    #region Folder Operations

    public async Task<IEnumerable<MediaFolder>> GetAllFoldersAsync()
    {
        return await _folderRepository.GetAllAsync();
    }

    public async Task<MediaFolder?> GetFolderByIdAsync(int id)
    {
        return await _folderRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MediaFolder>> GetRootFoldersAsync()
    {
        return await _folderRepository.FindAsync(f => f.ParentFolderId == null);
    }

    public async Task<IEnumerable<MediaFolder>> GetChildFoldersAsync(int parentFolderId)
    {
        return await _folderRepository.FindAsync(f => f.ParentFolderId == parentFolderId);
    }

    public async Task<MediaFolder> CreateFolderAsync(string name, string? description, int? parentFolderId)
    {
        var path = "/";
        if (parentFolderId.HasValue)
        {
            var parentFolder = await _folderRepository.GetByIdAsync(parentFolderId.Value);
            if (parentFolder != null)
            {
                path = $"{parentFolder.Path}/{name}";
            }
        }
        else
        {
            path = $"/{name}";
        }

        var folder = new MediaFolder
        {
            Name = name,
            Description = description,
            ParentFolderId = parentFolderId,
            Path = path,
            CreatedOn = DateTime.UtcNow
        };

        await _folderRepository.AddAsync(folder);
        await _unitOfWork.SaveChangesAsync();
        return folder;
    }

    public async Task<MediaFolder> UpdateFolderAsync(MediaFolder folder)
    {
        folder.ModifiedOn = DateTime.UtcNow;
        _folderRepository.Update(folder);
        await _unitOfWork.SaveChangesAsync();
        return folder;
    }

    public async Task DeleteFolderAsync(int id)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder != null)
        {
            _folderRepository.Remove(folder);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    #endregion

    #region File Operations

    public async Task<IEnumerable<MediaFile>> GetAllFilesAsync()
    {
        return await _fileRepository.GetAllAsync();
    }

    public async Task<IEnumerable<MediaFile>> GetFilesByFolderAsync(int? folderId)
    {
        return await _fileRepository.FindAsync(f => f.FolderId == folderId);
    }

    public async Task<MediaFile?> GetFileByIdAsync(int id)
    {
        return await _fileRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<MediaFile>> SearchFilesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllFilesAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();
        return await _fileRepository.FindAsync(f =>
            f.FileName.ToLower().Contains(lowerSearchTerm) ||
            f.Title.ToLower().Contains(lowerSearchTerm) ||
            (f.Description != null && f.Description.ToLower().Contains(lowerSearchTerm)) ||
            (f.Tags != null && f.Tags.ToLower().Contains(lowerSearchTerm))
        );
    }

    public async Task<MediaFile> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, int? folderId, string? title, string? description, string? tags)
    {
        if (fileStream == null || fileSize == 0)
        {
            throw new ArgumentException("File stream is empty or null");
        }

        // Ensure upload directories exist
        var uploadsPath = Path.Combine(_webRootPath, MediaDirectory);
        var thumbnailsPath = Path.Combine(_webRootPath, ThumbnailDirectory);
        Directory.CreateDirectory(uploadsPath);
        Directory.CreateDirectory(thumbnailsPath);

        // Generate unique filename
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);
        var relativeFilePath = $"/{MediaDirectory}/{uniqueFileName}";

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Create media file entity
        var mediaFile = new MediaFile
        {
            FileName = fileName,
            Title = title ?? Path.GetFileNameWithoutExtension(fileName),
            Description = description,
            Extension = extension,
            ContentType = contentType,
            SizeInBytes = fileSize,
            FilePath = relativeFilePath,
            Url = relativeFilePath,
            FolderId = folderId,
            Tags = tags,
            CreatedOn = DateTime.UtcNow
        };

        // Generate thumbnail for images
        if (IsImage(contentType))
        {
            try
            {
                var thumbnailFileName = $"thumb_{uniqueFileName}";
                var thumbnailPath = Path.Combine(thumbnailsPath, thumbnailFileName);
                var relativeThumbnailPath = $"/{ThumbnailDirectory}/{thumbnailFileName}";

                using (var image = await Image.LoadAsync(filePath))
                {
                    // Set image dimensions
                    mediaFile.Width = image.Width;
                    mediaFile.Height = image.Height;

                    // Create thumbnail
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(ThumbnailMaxWidth, ThumbnailMaxHeight),
                        Mode = ResizeMode.Max
                    }));

                    await image.SaveAsync(thumbnailPath);
                    mediaFile.ThumbnailUrl = relativeThumbnailPath;
                }
            }
            catch
            {
                // If thumbnail generation fails, continue without it
                mediaFile.ThumbnailUrl = relativeFilePath;
            }
        }

        await _fileRepository.AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();
        return mediaFile;
    }

    public async Task<MediaFile> UpdateFileAsync(MediaFile file)
    {
        file.ModifiedOn = DateTime.UtcNow;
        _fileRepository.Update(file);
        await _unitOfWork.SaveChangesAsync();
        return file;
    }

    public async Task DeleteFileAsync(int id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file != null)
        {
            // Delete physical files
            try
            {
                var filePath = Path.Combine(_webRootPath, file.FilePath.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (!string.IsNullOrEmpty(file.ThumbnailUrl))
                {
                    var thumbnailPath = Path.Combine(_webRootPath, file.ThumbnailUrl.TrimStart('/'));
                    if (File.Exists(thumbnailPath))
                    {
                        File.Delete(thumbnailPath);
                    }
                }
            }
            catch
            {
                // Continue with soft delete even if physical file deletion fails
            }

            _fileRepository.Remove(file);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<long> GetTotalStorageUsedAsync()
    {
        var files = await _fileRepository.GetAllAsync();
        return files.Sum(f => f.SizeInBytes);
    }

    public async Task<int> GetFileCountAsync()
    {
        var files = await _fileRepository.GetAllAsync();
        return files.Count();
    }

    #endregion

    #region Helper Methods

    private bool IsImage(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
