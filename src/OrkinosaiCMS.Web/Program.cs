using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;
using OrkinosaiCMS.Infrastructure.Repositories;
using OrkinosaiCMS.Infrastructure.Services;
using OrkinosaiCMS.Web.Components;
using OrkinosaiCMS.Web.Services;
using OrkinosaiCMS.Web.Middleware;
using OrkinosaiCMS.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for services that need it (e.g., ChatAgent)
builder.Services.AddHttpClient();

// Configure ZootaTesting options
builder.Services.Configure<ZootaTestingOptions>(
    builder.Configuration.GetSection(ZootaTestingOptions.SectionName));

// Add Controllers for API endpoints
builder.Services.AddControllers();

// Configure JWT Authentication
// Auto-provision JWT secret if not configured (for dev/test/failsafe mode)
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    jwtSecret = "OrkinosaiCMS-Dev-Secret-Key-DO-NOT-USE-IN-PRODUCTION-" + Guid.Parse("12345678-1234-1234-1234-123456789012").ToString();
    // Log warning during startup (will be logged once application starts)
    Console.WriteLine("WARNING: JWT Secret not configured. Using auto-generated key for development. DO NOT USE IN PRODUCTION!");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OrkinosaiCMS";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OrkinosaiCMS";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Register Authentication Services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Oqtane Authentication Service (separate from main auth)
builder.Services.AddScoped<IOqtaneAuthService, OqtaneAuthService>();

// Configure Database with Environment-Specific Validation
var environment = builder.Environment.EnvironmentName;
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Validate database configuration based on environment
var configLogger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("DatabaseConfig");

// SQLite is ONLY allowed for local Development environment (Visual Studio F5/debug)
// ALL Azure deployments (including dev deployments) MUST use Azure SQL
if (databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
{
    // SQLite detected - only allow in Development environment
    if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
        // This is acceptable for local F5/debug runs
        configLogger.LogInformation("✓ Database configuration validated: Local Development using SQLite");
    }
    else
    {
        // SQLite is NEVER allowed in any deployed environment (Production, Staging, or deployed Dev)
        configLogger.LogCritical("CONFIGURATION ERROR: {Environment} environment is configured to use SQLite. " +
            "SQLite is ONLY allowed for local Visual Studio F5/debug runs. " +
            "ALL Azure deployments (dev, staging, production) MUST use Azure SQL Database.", environment);
        throw new InvalidOperationException(
            $"{environment} environment cannot use SQLite. SQLite is only for local development. " +
            "Please configure Azure SQL Database via Azure App Service Configuration.");
    }
}
else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    // SqlServer provider - validate connection string for deployed environments
    if (!environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
        // Production, Staging, or other deployed environments
        if (string.IsNullOrWhiteSpace(connectionString) || 
            connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            configLogger.LogCritical("CONFIGURATION ERROR: {Environment} environment has invalid connection string. " +
                "LocalDB is not allowed in deployed environments. " +
                "Azure SQL connection string must be configured via Azure App Service Configuration.", environment);
            throw new InvalidOperationException(
                $"{environment} environment requires a valid Azure SQL connection string. " +
                "Please configure ConnectionStrings__DefaultConnection in Azure App Service Configuration.");
        }
        
        configLogger.LogInformation("✓ Database configuration validated: {Environment} using Azure SQL Database", environment);
    }
    else
    {
        // Development environment using SqlServer (not typical for local F5, but allowed)
        configLogger.LogWarning("Development environment is configured to use SqlServer. " +
            "For local development (F5/debug), SQLite is recommended for easier setup. " +
            "If deploying to Azure dev environment, this configuration is correct.");
    }
}
else
{
    // Other providers not supported
    configLogger.LogWarning("Unsupported database provider: {Provider}. Only SQLite (local dev only) and SqlServer (Azure SQL) are supported.",
        databaseProvider);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
    {
        var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=orkinosai-cms.db";
        options.UseSqlite(sqliteConnectionString, sqliteOptions =>
        {
            sqliteOptions.MigrationsAssembly("OrkinosaiCMS.Infrastructure");
        });
    }
    else
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.MigrationsAssembly("OrkinosaiCMS.Infrastructure");
        });
    }
});

// Register Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IPageLayoutService, PageLayoutService>();
builder.Services.AddScoped<IMasterPageService, MasterPageService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
builder.Services.AddScoped<IAIImageGenerationService, AIImageGenerationService>();
builder.Services.AddHttpClient(); // Required for AIImageGenerationService
builder.Services.AddScoped<IMediaService>(sp =>
{
    var fileRepo = sp.GetRequiredService<IRepository<OrkinosaiCMS.Core.Entities.Media.MediaFile>>();
    var folderRepo = sp.GetRequiredService<IRepository<OrkinosaiCMS.Core.Entities.Media.MediaFolder>>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    var webHostEnv = sp.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<MediaService>>();
    return new MediaService(fileRepo, folderRepo, unitOfWork, webHostEnv.WebRootPath, logger);
});


var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Starting database initialization...");
        await SeedData.InitializeAsync(services);
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "CRITICAL ERROR: Database initialization failed. The application cannot start without a properly initialized database. Please check your database configuration in appsettings.json and ensure the database is accessible. The application will now exit.");
        
        // Exit the application instead of continuing with a broken database
        throw new InvalidOperationException("Database initialization failed. The application cannot continue. See logs for details.", ex);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Use development exception middleware in development mode for detailed error information
    app.UseMiddleware<DevelopmentExceptionMiddleware>();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
