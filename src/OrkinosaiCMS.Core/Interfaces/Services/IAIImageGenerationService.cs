namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service for AI-powered image generation using DALL-E
/// </summary>
public interface IAIImageGenerationService
{
    /// <summary>
    /// Generate an image from a text prompt using DALL-E
    /// </summary>
    /// <param name="prompt">Text description of the image to generate</param>
    /// <param name="size">Image size (e.g., "1024x1024", "1792x1024", "1024x1792")</param>
    /// <param name="quality">Image quality ("standard" or "hd")</param>
    /// <param name="style">Image style ("natural" or "vivid")</param>
    /// <returns>URL of the generated image</returns>
    Task<string> GenerateImageAsync(string prompt, string size = "1792x1024", string quality = "standard", string style = "vivid");

    /// <summary>
    /// Generate an image and save it to the media library
    /// </summary>
    /// <param name="prompt">Text description of the image to generate</param>
    /// <param name="fileName">File name for the saved image</param>
    /// <param name="folderId">Optional folder ID to save the image in</param>
    /// <param name="size">Image size</param>
    /// <returns>The saved MediaFile entity</returns>
    Task<Core.Entities.Media.MediaFile> GenerateAndSaveImageAsync(string prompt, string fileName, int? folderId = null, string size = "1792x1024");
}
