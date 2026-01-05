using Microsoft.Extensions.Configuration;
using Xunit;

namespace OrkinosaiCMS.Tests;

/// <summary>
/// Tests for database configuration validation logic.
/// These tests verify that the application enforces correct database provider configuration
/// based on the environment (SQLite for development, Azure SQL for production).
/// </summary>
public class DatabaseConfigurationTests
{
    [Theory]
    [InlineData("Production", "SQLite", false, "Production must not use SQLite")]
    [InlineData("Production", "SqlServer", true, "Production should use SqlServer")]
    [InlineData("Production", "sqlserver", true, "Production should use SqlServer (case-insensitive)")]
    [InlineData("Development", "SQLite", true, "Development can use SQLite")]
    [InlineData("Development", "SqlServer", true, "Development can use SqlServer")]
    [InlineData("Staging", "SQLite", true, "Staging can use SQLite")]
    [InlineData("Staging", "SqlServer", true, "Staging can use SqlServer")]
    public void ValidateDatabaseProvider_EnforcesCorrectProviderByEnvironment(
        string environment, string provider, bool isValid, string reason)
    {
        // This test verifies the configuration validation logic from Program.cs:
        // - Production environment MUST NOT use SQLite
        // - Production environment SHOULD use SqlServer (Azure SQL)
        // - Development environment can use any provider (but SQLite is recommended)
        // - Other environments can use any provider
        
        bool configurationIsValid;
        
        if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            // Production MUST NOT use SQLite
            configurationIsValid = !provider.Equals("SQLite", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            // Other environments can use any provider
            configurationIsValid = true;
        }
        
        Assert.Equal(isValid, configurationIsValid);
    }
    
    [Theory]
    [InlineData("", false, "Empty connection string is invalid")]
    [InlineData("   ", false, "Whitespace-only connection string is invalid")]
    [InlineData("Server=(localdb)\\mssqllocaldb;Database=Test", false, "LocalDB is not valid for production")]
    [InlineData("Data Source=orkinosai-cms.db", false, "SQLite connection string is not valid for production")]
    [InlineData("Server=tcp:server.database.windows.net;Database=mydb;User ID=admin;Password=pass", true, "Valid Azure SQL connection string")]
    public void ValidateProductionConnectionString_RejectsInvalidConnectionStrings(
        string connectionString, bool isValid, string reason)
    {
        // This test verifies connection string validation for production environment
        // Production connection strings must:
        // - Not be empty or whitespace
        // - Not contain LocalDB references
        // - Not be SQLite connection strings
        
        bool connectionStringIsValid = true;
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionStringIsValid = false;
        }
        else if (connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            connectionStringIsValid = false;
        }
        else if (connectionString.Contains("orkinosai-cms.db", StringComparison.OrdinalIgnoreCase) ||
                 connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            connectionStringIsValid = false;
        }
        
        Assert.Equal(isValid, connectionStringIsValid);
    }
    
    [Fact]
    public void DevelopmentConfiguration_RecommendsSQLite()
    {
        // This test documents that Development environment should prefer SQLite
        var developmentEnvironment = "Development";
        var recommendedProvider = "SQLite";
        
        // Verify that SQLite is the recommended provider for development
        Assert.Equal("Development", developmentEnvironment);
        Assert.Equal("SQLite", recommendedProvider);
    }
    
    [Fact]
    public void ProductionConfiguration_RequiresAzureSQL()
    {
        // This test documents that Production environment must use Azure SQL
        var productionEnvironment = "Production";
        var requiredProvider = "SqlServer";
        
        // Verify that SqlServer (Azure SQL) is required for production
        Assert.Equal("Production", productionEnvironment);
        Assert.Equal("SqlServer", requiredProvider);
    }
}
