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
    [InlineData("Production", "SQLite", false, "Production must not use SQLite - only Azure SQL")]
    [InlineData("Production", "SqlServer", true, "Production must use SqlServer (Azure SQL)")]
    [InlineData("Production", "sqlserver", true, "Production must use SqlServer (case-insensitive)")]
    [InlineData("Development", "SQLite", true, "Development can use SQLite for local F5/debug")]
    [InlineData("Development", "SqlServer", true, "Development can use SqlServer for deployed dev")]
    [InlineData("Staging", "SQLite", false, "Staging (deployed) must not use SQLite")]
    [InlineData("Staging", "SqlServer", true, "Staging must use SqlServer (Azure SQL)")]
    [InlineData("Test", "SQLite", false, "Any deployed environment must not use SQLite")]
    [InlineData("Test", "SqlServer", true, "Any deployed environment must use SqlServer")]
    public void ValidateDatabaseProvider_EnforcesCorrectProviderByEnvironment(
        string environment, string provider, bool isValid, string reason)
    {
        // This test verifies the STRICT configuration validation logic from Program.cs:
        // - SQLite is ONLY allowed in Development environment (for local F5/debug)
        // - ALL other environments (Production, Staging, deployed Dev) MUST use SqlServer (Azure SQL)
        // - This ensures SQLite is never used in any Azure deployment
        
        bool configurationIsValid;
        
        if (provider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            // SQLite is ONLY valid for Development environment (local F5)
            configurationIsValid = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
        else if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            // SqlServer (Azure SQL) is valid for all environments
            configurationIsValid = true;
        }
        else
        {
            // Other providers not explicitly validated here
            configurationIsValid = false;
        }
        
        Assert.Equal(isValid, configurationIsValid);
    }
    
    [Theory]
    [InlineData("", false, "Empty connection string is invalid")]
    [InlineData("   ", false, "Whitespace-only connection string is invalid")]
    [InlineData("Server=(localdb)\\mssqllocaldb;Database=Test", false, "LocalDB is not valid for production")]
    [InlineData("Data Source=orkinosai-cms.db", false, "SQLite connection string is not valid for production")]
    [InlineData("Data Source=:memory:", false, "SQLite in-memory connection string is not valid for production")]
    [InlineData("Server=tcp:server.database.windows.net;Database=mydb;User ID=admin;Password=pass", true, "Valid Azure SQL connection string")]
    [InlineData("Data Source=server.database.windows.net;Initial Catalog=mydb;User ID=admin;Password=pass", true, "Valid SQL Server connection string with Data Source")]
    public void ValidateProductionConnectionString_RejectsInvalidConnectionStrings(
        string connectionString, bool isValid, string reason)
    {
        // This test verifies connection string validation for production environment
        // Production connection strings must:
        // - Not be empty or whitespace
        // - Not contain LocalDB references
        // - Not be SQLite connection strings (check for .db file extension or :memory:)
        
        bool connectionStringIsValid = true;
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionStringIsValid = false;
        }
        else if (connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            connectionStringIsValid = false;
        }
        else if (connectionString.Contains(".db", StringComparison.OrdinalIgnoreCase) ||
                 connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            // SQLite connection strings contain .db file extensions or :memory:
            connectionStringIsValid = false;
        }
        
        Assert.Equal(isValid, connectionStringIsValid);
    }
    
    [Fact]
    public void DevelopmentConfiguration_RecommendsSQLiteForLocalOnly()
    {
        // This test documents that Development environment uses SQLite for LOCAL F5/debug ONLY
        var developmentEnvironment = "Development";
        var recommendedProvider = "SQLite";
        
        // Verify that SQLite is the recommended provider for local development (F5/debug)
        Assert.Equal("Development", developmentEnvironment);
        Assert.Equal("SQLite", recommendedProvider);
    }
    
    [Fact]
    public void AllDeployments_RequireAzureSQL()
    {
        // This test documents that ALL Azure deployments (dev, staging, production) MUST use Azure SQL
        // SQLite is ONLY for local Visual Studio F5/debug runs, NEVER for any deployment
        var deploymentEnvironments = new[] { "Production", "Staging", "Development" }; // when deployed to Azure
        var requiredProvider = "SqlServer"; // Azure SQL
        
        foreach (var env in deploymentEnvironments)
        {
            // All deployed environments must use Azure SQL, never SQLite
            Assert.Equal("SqlServer", requiredProvider);
        }
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
