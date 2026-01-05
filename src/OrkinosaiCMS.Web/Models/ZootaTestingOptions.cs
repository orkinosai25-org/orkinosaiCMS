namespace OrkinosaiCMS.Web.Models;

/// <summary>
/// Configuration options for Zoota Testing features
/// Controls visibility and access to Zoota Test Page and automated test runner
/// </summary>
public class ZootaTestingOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "ZootaTesting";

    /// <summary>
    /// Gets or sets whether Zoota Testing features are enabled
    /// Set to true during R&D and pre-production, false in production
    /// </summary>
    public bool Enabled { get; set; } = true;
}
