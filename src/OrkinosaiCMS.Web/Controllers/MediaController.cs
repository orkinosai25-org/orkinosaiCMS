using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrkinosaiCMS.Core.Entities.Media;
using OrkinosaiCMS.Core.Interfaces.Services;

namespace OrkinosaiCMS.Web.Controllers;

/// <summary>
/// API Controller for media library management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    #region Folder Endpoints

    /// <summary>
    /// Get all folders
    /// </summary>
    [HttpGet("folders")]
    [ProducesResponseType(typeof(IEnumerable<MediaFolder>), 200)]
    public async Task<IActionResult> GetAllFolders()
    {
        try
        {
            var folders = await _mediaService.GetAllFoldersAsync();
            return Ok(folders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all folders");
            return StatusCode(500, new { message = "Error retrieving folders" });
        }
    }

    /// <summary>
    /// Get root folders
    /// </summary>
    [HttpGet("folders/root")]
    [ProducesResponseType(typeof(IEnumerable<MediaFolder>), 200)]
    public async Task<IActionResult> GetRootFolders()
    {
        try
        {
            var folders = await _mediaService.GetRootFoldersAsync();
            return Ok(folders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root folders");
            return StatusCode(500, new { message = "Error retrieving root folders" });
        }
    }

    /// <summary>
    /// Get folder by ID
    /// </summary>
    [HttpGet("folders/{id}")]
    [ProducesResponseType(typeof(MediaFolder), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFolderById(int id)
    {
        try
        {
            var folder = await _mediaService.GetFolderByIdAsync(id);
            if (folder == null)
                return NotFound(new { message = $"Folder with ID {id} not found" });

            return Ok(folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting folder by ID {FolderId}", id);
            return StatusCode(500, new { message = "Error retrieving folder" });
        }
    }

    /// <summary>
    /// Get child folders
    /// </summary>
    [HttpGet("folders/{id}/children")]
    [ProducesResponseType(typeof(IEnumerable<MediaFolder>), 200)]
    public async Task<IActionResult> GetChildFolders(int id)
    {
        try
        {
            var folders = await _mediaService.GetChildFoldersAsync(id);
            return Ok(folders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting child folders for folder {FolderId}", id);
            return StatusCode(500, new { message = "Error retrieving child folders" });
        }
    }

    /// <summary>
    /// Create a new folder
    /// </summary>
    [HttpPost("folders")]
    [ProducesResponseType(typeof(MediaFolder), 201)]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        try
        {
            var folder = await _mediaService.CreateFolderAsync(request.Name, request.Description, request.ParentFolderId);
            return CreatedAtAction(nameof(GetFolderById), new { id = folder.Id }, folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating folder");
            return StatusCode(500, new { message = "Error creating folder" });
        }
    }

    /// <summary>
    /// Update a folder
    /// </summary>
    [HttpPut("folders/{id}")]
    [ProducesResponseType(typeof(MediaFolder), 200)]
    public async Task<IActionResult> UpdateFolder(int id, [FromBody] MediaFolder folder)
    {
        try
        {
            if (id != folder.Id)
                return BadRequest(new { message = "ID mismatch" });

            var updatedFolder = await _mediaService.UpdateFolderAsync(folder);
            return Ok(updatedFolder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating folder {FolderId}", id);
            return StatusCode(500, new { message = "Error updating folder" });
        }
    }

    /// <summary>
    /// Delete a folder
    /// </summary>
    [HttpDelete("folders/{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteFolder(int id)
    {
        try
        {
            await _mediaService.DeleteFolderAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting folder {FolderId}", id);
            return StatusCode(500, new { message = "Error deleting folder" });
        }
    }

    #endregion

    #region File Endpoints

    /// <summary>
    /// Get all media files
    /// </summary>
    [HttpGet("files")]
    [ProducesResponseType(typeof(IEnumerable<MediaFile>), 200)]
    public async Task<IActionResult> GetAllFiles()
    {
        try
        {
            var files = await _mediaService.GetAllFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all files");
            return StatusCode(500, new { message = "Error retrieving files" });
        }
    }

    /// <summary>
    /// Get files by folder
    /// </summary>
    [HttpGet("files/folder/{folderId?}")]
    [ProducesResponseType(typeof(IEnumerable<MediaFile>), 200)]
    public async Task<IActionResult> GetFilesByFolder(int? folderId)
    {
        try
        {
            var files = await _mediaService.GetFilesByFolderAsync(folderId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting files by folder {FolderId}", folderId);
            return StatusCode(500, new { message = "Error retrieving files" });
        }
    }

    /// <summary>
    /// Get file by ID
    /// </summary>
    [HttpGet("files/{id}")]
    [ProducesResponseType(typeof(MediaFile), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFileById(int id)
    {
        try
        {
            var file = await _mediaService.GetFileByIdAsync(id);
            if (file == null)
                return NotFound(new { message = $"File with ID {id} not found" });

            return Ok(file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file by ID {FileId}", id);
            return StatusCode(500, new { message = "Error retrieving file" });
        }
    }

    /// <summary>
    /// Search media files
    /// </summary>
    [HttpGet("files/search")]
    [ProducesResponseType(typeof(IEnumerable<MediaFile>), 200)]
    public async Task<IActionResult> SearchFiles([FromQuery] string searchTerm)
    {
        try
        {
            var files = await _mediaService.SearchFilesAsync(searchTerm);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with term {SearchTerm}", searchTerm);
            return StatusCode(500, new { message = "Error searching files" });
        }
    }

    /// <summary>
    /// Upload a new file
    /// </summary>
    [HttpPost("files/upload")]
    [ProducesResponseType(typeof(MediaFile), 201)]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
    {
        try
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "No file provided" });

            // Validate file size (50MB max)
            const long maxFileSize = 50 * 1024 * 1024;
            if (request.File.Length > maxFileSize)
                return BadRequest(new { message = "File size exceeds 50MB limit" });

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = $"File type {extension} is not allowed" });

            using (var stream = request.File.OpenReadStream())
            {
                var file = await _mediaService.UploadFileAsync(
                    stream,
                    request.File.FileName,
                    request.File.ContentType,
                    request.File.Length,
                    request.FolderId,
                    request.Title,
                    request.Description,
                    request.Tags
                );

                return CreatedAtAction(nameof(GetFileById), new { id = file.Id }, file);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new { message = $"Error uploading file: {ex.Message}" });
        }
    }

    /// <summary>
    /// Update file metadata
    /// </summary>
    [HttpPut("files/{id}")]
    [ProducesResponseType(typeof(MediaFile), 200)]
    public async Task<IActionResult> UpdateFile(int id, [FromBody] MediaFile file)
    {
        try
        {
            if (id != file.Id)
                return BadRequest(new { message = "ID mismatch" });

            var updatedFile = await _mediaService.UpdateFileAsync(file);
            return Ok(updatedFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating file {FileId}", id);
            return StatusCode(500, new { message = "Error updating file" });
        }
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    [HttpDelete("files/{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteFile(int id)
    {
        try
        {
            await _mediaService.DeleteFileAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileId}", id);
            return StatusCode(500, new { message = "Error deleting file" });
        }
    }

    /// <summary>
    /// Get storage statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(StorageStats), 200)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var totalStorage = await _mediaService.GetTotalStorageUsedAsync();
            var fileCount = await _mediaService.GetFileCountAsync();

            return Ok(new StorageStats
            {
                TotalStorageBytes = totalStorage,
                TotalStorageMB = totalStorage / (1024.0 * 1024.0),
                FileCount = fileCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage stats");
            return StatusCode(500, new { message = "Error retrieving storage statistics" });
        }
    }

    #endregion
}

#region Request Models

public class CreateFolderRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentFolderId { get; set; }
}

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public int? FolderId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
}

public class StorageStats
{
    public long TotalStorageBytes { get; set; }
    public double TotalStorageMB { get; set; }
    public int FileCount { get; set; }
}

#endregion
