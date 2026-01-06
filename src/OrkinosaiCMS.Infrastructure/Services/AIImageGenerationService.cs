using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrkinosaiCMS.Core.Entities.Media;
using OrkinosaiCMS.Core.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// AI Image Generation Service using Azure OpenAI DALL-E
/// </summary>
public class AIImageGenerationService : IAIImageGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly IMediaService _mediaService;
    private readonly ILogger<AIImageGenerationService> _logger;
    private readonly HttpClient _httpClient;

    public AIImageGenerationService(
        IConfiguration configuration,
        IMediaService mediaService,
        ILogger<AIImageGenerationService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _mediaService = mediaService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GenerateImageAsync(string prompt, string size = "1792x1024", string quality = "standard", string style = "vivid")
    {
        var endpoint = _configuration["AzureOpenAI:Endpoint"];
        var apiKey = _configuration["AzureOpenAI:ApiKey"];
        var apiVersion = _configuration["AzureOpenAI:ApiVersion"] ?? "2024-02-01";

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Azure OpenAI configuration is missing. Please configure Endpoint and ApiKey in appsettings.json");
        }

        // Use DALL-E 3 endpoint
        var url = $"{endpoint.TrimEnd('/')}/openai/deployments/dall-e-3/images/generations?api-version={apiVersion}";

        var requestBody = new
        {
            prompt = prompt,
            size = size,
            quality = quality,
            style = style,
            n = 1
        };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);
            
            var imageUrl = jsonDoc.RootElement
                .GetProperty("data")[0]
                .GetProperty("url")
                .GetString();

            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new InvalidOperationException("Failed to generate image: No URL returned");
            }

            _logger.LogInformation("Successfully generated image from prompt: {Prompt}", prompt);
            return imageUrl;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error generating image from prompt: {Prompt}", prompt);
            throw new InvalidOperationException($"Failed to generate image: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image from prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async Task<MediaFile> GenerateAndSaveImageAsync(string prompt, string fileName, int? folderId = null, string size = "1792x1024")
    {
        try
        {
            // Generate the image
            var imageUrl = await GenerateImageAsync(prompt, size);

            // Download the image
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
            var imageStream = new MemoryStream(imageBytes);

            // Sanitize filename
            var sanitizedFileName = SanitizeFileName(fileName);
            if (!sanitizedFileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                sanitizedFileName += ".png";
            }

            // Save to media library
            var mediaFile = await _mediaService.UploadFileAsync(
                imageStream,
                sanitizedFileName,
                "image/png",
                imageBytes.Length,
                folderId,
                fileName,
                $"AI-generated image: {prompt}",
                "ai-generated,dall-e,banner"
            );

            _logger.LogInformation("Successfully saved generated image to media library: {FileName}", sanitizedFileName);
            return mediaFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating and saving image: {Prompt}", prompt);
            throw;
        }
    }

    private string SanitizeFileName(string fileName)
    {
        // Remove invalid characters
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        
        // Ensure it's not empty
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = $"generated_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        return sanitized;
    }
}
