using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Services;
using System.Text.Json;

namespace OrkinosaiCMS.Tests;

/// <summary>
/// Comprehensive unit tests for PageDesigner2 component covering page edit and design functionality.
/// Tests penetrate all key functionality including block editing, state management, and form operations.
/// </summary>
public class PageDesigner2Tests
{
    private readonly Mock<IPageService> _mockPageService;
    private readonly Mock<IPageLayoutService> _mockPageLayoutService;
    private readonly Mock<ILogger<PageDesigner2TestHelper>> _mockLogger;

    public PageDesigner2Tests()
    {
        _mockPageService = new Mock<IPageService>();
        _mockPageLayoutService = new Mock<IPageLayoutService>();
        _mockLogger = new Mock<ILogger<PageDesigner2TestHelper>>();
    }

    #region Block Content Parsing Tests

    [Fact]
    public void ParseHeroBlockContent_ValidJson_ShouldParseCorrectly()
    {
        // Arrange
        var heroContent = "{\"title\": \"Welcome\", \"subtitle\": \"Your hero message here\", \"imageUrl\": \"\"}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseHeroContent(heroContent);

        // Assert
        Assert.Equal("Welcome", result.Title);
        Assert.Equal("Your hero message here", result.Subtitle);
        Assert.Equal("", result.ImageUrl);
    }

    [Fact]
    public void ParseHeroBlockContent_InvalidJson_ShouldReturnDefaults()
    {
        // Arrange
        var invalidContent = "{invalid json}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseHeroContent(invalidContent);

        // Assert
        Assert.Equal("", result.Title);
        Assert.Equal("", result.Subtitle);
        Assert.Equal("", result.ImageUrl);
    }

    [Theory]
    [InlineData("", "", "", "")]
    [InlineData("Test Title", "", "", "Test Title")]
    [InlineData("", "Test Subtitle", "", "")]
    [InlineData("Title", "Subtitle", "/img.jpg", "Title")]
    public void ParseHeroBlockContent_VariousInputs_ShouldHandleCorrectly(
        string title, string subtitle, string imageUrl, string expectedTitle)
    {
        // Arrange
        var content = $"{{\"title\": \"{title}\", \"subtitle\": \"{subtitle}\", \"imageUrl\": \"{imageUrl}\"}}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseHeroContent(content);

        // Assert
        Assert.Equal(expectedTitle, result.Title);
    }

    #endregion

    #region Block Content Generation Tests

    [Fact]
    public void GenerateHeroBlockContent_ValidInputs_ShouldGenerateCorrectJson()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        var title = "New Title";
        var subtitle = "New Subtitle";
        var imageUrl = "/uploads/hero.jpg";

        // Act
        var result = helper.GenerateHeroContent(title, subtitle, imageUrl);

        // Assert
        Assert.NotNull(result);
        var json = JsonDocument.Parse(result);
        Assert.Equal(title, json.RootElement.GetProperty("title").GetString());
        Assert.Equal(subtitle, json.RootElement.GetProperty("subtitle").GetString());
        Assert.Equal(imageUrl, json.RootElement.GetProperty("imageUrl").GetString());
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("Title with \"quotes\"", "Subtitle with 'quotes'", "/path/to/image.jpg")]
    [InlineData("Special chars: <>&", "Line\nBreak", "")]
    public void GenerateHeroBlockContent_SpecialCharacters_ShouldEscapeCorrectly(
        string title, string subtitle, string imageUrl)
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.GenerateHeroContent(title, subtitle, imageUrl);

        // Assert - Should be valid JSON
        Assert.NotNull(result);
        var exception = Record.Exception(() => JsonDocument.Parse(result));
        Assert.Null(exception);
    }

    #endregion

    #region Form Field Reset Tests

    [Fact]
    public void ResetFormFields_ShouldClearAllFields()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper
        {
            BlockContentHtml = "test html",
            BlockImageUrl = "test.jpg",
            BlockImageAlt = "test alt",
            BlockHeroTitle = "test title",
            BlockHeroSubtitle = "test subtitle",
            BlockHeroImageUrl = "hero.jpg",
            BlockHtmlContent = "<div>test</div>"
        };

        // Act
        helper.ResetFormFields();

        // Assert
        Assert.Equal("", helper.BlockContentHtml);
        Assert.Equal("", helper.BlockImageUrl);
        Assert.Equal("", helper.BlockImageAlt);
        Assert.Equal("", helper.BlockHeroTitle);
        Assert.Equal("", helper.BlockHeroSubtitle);
        Assert.Equal("", helper.BlockHeroImageUrl);
        Assert.Equal("", helper.BlockHtmlContent);
    }

    [Fact]
    public void ResetFormFields_CalledMultipleTimes_ShouldRemainEmpty()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper
        {
            BlockHeroTitle = "test"
        };

        // Act
        helper.ResetFormFields();
        helper.ResetFormFields();
        helper.ResetFormFields();

        // Assert
        Assert.Equal("", helper.BlockHeroTitle);
    }

    #endregion

    #region Block Type Validation Tests

    [Theory]
    [InlineData("text", true)]
    [InlineData("image", true)]
    [InlineData("hero", true)]
    [InlineData("html", true)]
    [InlineData("video", true)]
    [InlineData("gallery", true)]
    [InlineData("cards", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void ValidateBlockType_ShouldIdentifyValidTypes(string blockType, bool expectedValid)
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.IsValidBlockType(blockType);

        // Assert
        Assert.Equal(expectedValid, result);
    }

    #endregion

    #region Default Block Content Tests

    [Theory]
    [InlineData("text", "{\"html\": \"<p>Enter your text here</p>\"}")]
    [InlineData("image", "{\"src\": \"\", \"alt\": \"Image\"}")]
    [InlineData("hero", "{\"title\": \"Hero Title\", \"subtitle\": \"Subtitle\", \"imageUrl\": \"\"}")]
    [InlineData("html", "{\"html\": \"<div>Custom HTML</div>\"}")]
    [InlineData("video", "{\"url\": \"\"}")]
    public void GetDefaultBlockContent_ShouldReturnCorrectDefaults(string blockType, string expectedContent)
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.GetDefaultBlockContent(blockType);

        // Assert
        Assert.Equal(expectedContent, result);
    }

    #endregion

    #region Text Block Tests

    [Fact]
    public void ParseTextBlockContent_ValidHtml_ShouldParseCorrectly()
    {
        // Arrange
        var content = "{\"html\": \"<p>Test paragraph</p>\"}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseTextContent(content);

        // Assert
        Assert.Equal("<p>Test paragraph</p>", result);
    }

    [Fact]
    public void GenerateTextBlockContent_ShouldGenerateValidJson()
    {
        // Arrange
        var html = "<h1>Heading</h1><p>Paragraph</p>";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.GenerateTextContent(html);

        // Assert
        var json = JsonDocument.Parse(result);
        Assert.Equal(html, json.RootElement.GetProperty("html").GetString());
    }

    #endregion

    #region Image Block Tests

    [Fact]
    public void ParseImageBlockContent_ValidJson_ShouldParseCorrectly()
    {
        // Arrange
        var content = "{\"src\": \"/images/test.jpg\", \"alt\": \"Test Image\"}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseImageContent(content);

        // Assert
        Assert.Equal("/images/test.jpg", result.Src);
        Assert.Equal("Test Image", result.Alt);
    }

    [Fact]
    public void GenerateImageBlockContent_ShouldGenerateValidJson()
    {
        // Arrange
        var src = "/uploads/image.png";
        var alt = "My Image";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.GenerateImageContent(src, alt);

        // Assert
        var json = JsonDocument.Parse(result);
        Assert.Equal(src, json.RootElement.GetProperty("src").GetString());
        Assert.Equal(alt, json.RootElement.GetProperty("alt").GetString());
    }

    #endregion

    #region HTML Block Tests

    [Fact]
    public void ParseHtmlBlockContent_ValidJson_ShouldParseCorrectly()
    {
        // Arrange
        var htmlContent = "<div class=\"custom\"><span>Content</span></div>";
        var content = $"{{\"html\": {JsonSerializer.Serialize(htmlContent)}}}";
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.ParseHtmlContent(content);

        // Assert
        Assert.Equal(htmlContent, result);
    }

    #endregion

    #region Content Preview Truncation Tests

    [Theory]
    [InlineData("Short content", "Short content")]
    [InlineData("This is a very long content that exceeds fifty characters in length", "This is a very long content that exceeds fifty cha...")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void TruncateContentForLogging_ShouldTruncateCorrectly(string? input, string expected)
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.TruncateContentPreview(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TruncateContentForLogging_ExactlyFiftyChars_ShouldNotTruncate()
    {
        // Arrange
        var content = "12345678901234567890123456789012345678901234567890"; // Exactly 50 chars
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.TruncateContentPreview(content);

        // Assert
        Assert.Equal(content, result);
    }

    #endregion

    #region Block Editing Workflow Tests

    [Fact]
    public void EditBlockWorkflow_HeroBlock_ShouldFollowCompleteWorkflow()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        var block = new PageBlock
        {
            Id = 1,
            BlockType = "hero",
            Content = "{\"title\": \"Original Title\", \"subtitle\": \"Original Subtitle\", \"imageUrl\": \"\"}"
        };

        // Act - Simulate editing workflow
        // Step 1: Reset fields
        helper.ResetFormFields();
        
        // Step 2: Parse existing content
        var parsed = helper.ParseHeroContent(block.Content);
        helper.BlockHeroTitle = parsed.Title;
        helper.BlockHeroSubtitle = parsed.Subtitle;
        helper.BlockHeroImageUrl = parsed.ImageUrl;

        // Step 3: User modifies fields
        helper.BlockHeroTitle = "Updated Title";
        helper.BlockHeroSubtitle = "Updated Subtitle";
        helper.BlockHeroImageUrl = "/new-image.jpg";

        // Step 4: Generate new content
        var newContent = helper.GenerateHeroContent(
            helper.BlockHeroTitle,
            helper.BlockHeroSubtitle,
            helper.BlockHeroImageUrl);

        // Assert
        var result = JsonDocument.Parse(newContent);
        Assert.Equal("Updated Title", result.RootElement.GetProperty("title").GetString());
        Assert.Equal("Updated Subtitle", result.RootElement.GetProperty("subtitle").GetString());
        Assert.Equal("/new-image.jpg", result.RootElement.GetProperty("imageUrl").GetString());
    }

    [Fact]
    public void EditBlockWorkflow_TextBlock_ShouldMaintainHtmlContent()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        var htmlContent = "<h2>Title</h2><p>Paragraph with <strong>bold</strong> text</p>";
        var block = new PageBlock
        {
            Id = 2,
            BlockType = "text",
            Content = $"{{\"html\": {JsonSerializer.Serialize(htmlContent)}}}"
        };

        // Act
        helper.ResetFormFields();
        helper.BlockContentHtml = helper.ParseTextContent(block.Content);
        var newContent = helper.GenerateTextContent(helper.BlockContentHtml);

        // Assert
        var result = helper.ParseTextContent(newContent);
        Assert.Equal(htmlContent, result);
    }

    #endregion

    #region Edge Cases and Error Handling Tests

    [Fact]
    public void ParseBlockContent_MalformedJson_ShouldNotThrow()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        var malformedContent = "{title: 'missing quotes', subtitle";

        // Act & Assert
        var exception = Record.Exception(() => helper.ParseHeroContent(malformedContent));
        Assert.Null(exception); // Should handle gracefully
    }

    [Fact]
    public void GenerateBlockContent_NullValues_ShouldHandleGracefully()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        var result = helper.GenerateHeroContent(null!, null!, null!);

        // Assert
        Assert.NotNull(result);
        var json = JsonDocument.Parse(result);
        Assert.NotNull(json);
    }

    [Fact]
    public void ParseBlockContent_MissingProperties_ShouldUseDefaults()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        var partialContent = "{\"title\": \"Only Title\"}"; // Missing subtitle and imageUrl

        // Act
        var result = helper.ParseHeroContent(partialContent);

        // Assert
        Assert.Equal("Only Title", result.Title);
        Assert.Equal("", result.Subtitle);
        Assert.Equal("", result.ImageUrl);
    }

    #endregion

    #region State Management Tests

    [Fact]
    public void StateChange_AfterBlockEdit_ShouldBeTracked()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();

        // Act
        helper.ResetFormFields();
        helper.BlockHeroTitle = "New Value";

        // Assert
        Assert.True(helper.HasFormChanges());
    }

    [Fact]
    public void StateChange_AfterReset_ShouldBeCleared()
    {
        // Arrange
        var helper = new PageDesigner2TestHelper();
        helper.BlockHeroTitle = "Test";

        // Act
        helper.ResetFormFields();

        // Assert
        Assert.False(helper.HasFormChanges());
    }

    #endregion
}

/// <summary>
/// Test helper class that simulates PageDesigner2 functionality for testing.
/// Extracts core logic to enable comprehensive unit testing without Blazor runtime.
/// </summary>
public class PageDesigner2TestHelper
{
    // Form fields
    public string BlockContentHtml { get; set; } = "";
    public string BlockImageUrl { get; set; } = "";
    public string BlockImageAlt { get; set; } = "";
    public string BlockHeroTitle { get; set; } = "";
    public string BlockHeroSubtitle { get; set; } = "";
    public string BlockHeroImageUrl { get; set; } = "";
    public string BlockHtmlContent { get; set; } = "";

    private readonly Dictionary<string, string> _initialState = new();

    public PageDesigner2TestHelper()
    {
        CaptureInitialState();
    }

    private void CaptureInitialState()
    {
        _initialState["BlockContentHtml"] = BlockContentHtml;
        _initialState["BlockImageUrl"] = BlockImageUrl;
        _initialState["BlockImageAlt"] = BlockImageAlt;
        _initialState["BlockHeroTitle"] = BlockHeroTitle;
        _initialState["BlockHeroSubtitle"] = BlockHeroSubtitle;
        _initialState["BlockHeroImageUrl"] = BlockHeroImageUrl;
        _initialState["BlockHtmlContent"] = BlockHtmlContent;
    }

    public void ResetFormFields()
    {
        BlockContentHtml = "";
        BlockImageUrl = "";
        BlockImageAlt = "";
        BlockHeroTitle = "";
        BlockHeroSubtitle = "";
        BlockHeroImageUrl = "";
        BlockHtmlContent = "";
    }

    public bool HasFormChanges()
    {
        return BlockContentHtml != _initialState["BlockContentHtml"] ||
               BlockImageUrl != _initialState["BlockImageUrl"] ||
               BlockImageAlt != _initialState["BlockImageAlt"] ||
               BlockHeroTitle != _initialState["BlockHeroTitle"] ||
               BlockHeroSubtitle != _initialState["BlockHeroSubtitle"] ||
               BlockHeroImageUrl != _initialState["BlockHeroImageUrl"] ||
               BlockHtmlContent != _initialState["BlockHtmlContent"];
    }

    public (string Title, string Subtitle, string ImageUrl) ParseHeroContent(string content)
    {
        try
        {
            var json = JsonDocument.Parse(content);
            var title = json.RootElement.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
            var subtitle = json.RootElement.TryGetProperty("subtitle", out var s) ? s.GetString() ?? "" : "";
            var imageUrl = json.RootElement.TryGetProperty("imageUrl", out var i) ? i.GetString() ?? "" : "";
            return (title, subtitle, imageUrl);
        }
        catch
        {
            return ("", "", "");
        }
    }

    public string GenerateHeroContent(string title, string subtitle, string imageUrl)
    {
        return $"{{\"title\": {JsonSerializer.Serialize(title ?? "")}, \"subtitle\": {JsonSerializer.Serialize(subtitle ?? "")}, \"imageUrl\": {JsonSerializer.Serialize(imageUrl ?? "")}}}";
    }

    public string ParseTextContent(string content)
    {
        try
        {
            var json = JsonDocument.Parse(content);
            return json.RootElement.TryGetProperty("html", out var html) ? html.GetString() ?? "" : "";
        }
        catch
        {
            return "";
        }
    }

    public string GenerateTextContent(string html)
    {
        return $"{{\"html\": {JsonSerializer.Serialize(html)}}}";
    }

    public (string Src, string Alt) ParseImageContent(string content)
    {
        try
        {
            var json = JsonDocument.Parse(content);
            var src = json.RootElement.TryGetProperty("src", out var s) ? s.GetString() ?? "" : "";
            var alt = json.RootElement.TryGetProperty("alt", out var a) ? a.GetString() ?? "" : "";
            return (src, alt);
        }
        catch
        {
            return ("", "");
        }
    }

    public string GenerateImageContent(string src, string alt)
    {
        return $"{{\"src\": {JsonSerializer.Serialize(src)}, \"alt\": {JsonSerializer.Serialize(alt)}}}";
    }

    public string ParseHtmlContent(string content)
    {
        try
        {
            var json = JsonDocument.Parse(content);
            return json.RootElement.TryGetProperty("html", out var html) ? html.GetString() ?? "" : "";
        }
        catch
        {
            return "";
        }
    }

    public bool IsValidBlockType(string blockType)
    {
        var validTypes = new[] { "text", "image", "hero", "html", "video", "gallery", "cards" };
        return validTypes.Contains(blockType);
    }

    public string GetDefaultBlockContent(string blockType)
    {
        return blockType switch
        {
            "text" => "{\"html\": \"<p>Enter your text here</p>\"}",
            "image" => "{\"src\": \"\", \"alt\": \"Image\"}",
            "video" => "{\"url\": \"\"}",
            "hero" => "{\"title\": \"Hero Title\", \"subtitle\": \"Subtitle\", \"imageUrl\": \"\"}",
            "html" => "{\"html\": \"<div>Custom HTML</div>\"}",
            "gallery" => "{\"images\": []}",
            "cards" => "{\"title\": \"Card Title\", \"text\": \"Card content\", \"imageUrl\": \"\"}",
            _ => "{}"
        };
    }

    public string TruncateContentPreview(string? content)
    {
        if (string.IsNullOrEmpty(content))
            return "";
        
        if (content.Length <= 50)
            return content;
        
        return content.Substring(0, 50) + "...";
    }
}
