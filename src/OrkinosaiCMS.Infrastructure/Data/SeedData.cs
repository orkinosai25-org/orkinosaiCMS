using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrkinosaiCMS.Core.Entities.Navigation;
using OrkinosaiCMS.Core.Entities.Sites;
using System.Text.Json;

namespace OrkinosaiCMS.Infrastructure.Data;

/// <summary>
/// Seeds initial data for OrkinosaiCMS
/// </summary>
public static class SeedData
{
    // Demo admin password for initial seeding
    // WARNING: This is for demo/development purposes only. Change in production.
    private const string DEMO_ADMIN_PASSWORD = "Admin@123";

    /// <summary>
    /// Initialize database with seed data
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedData");

        try
        {
            // Check if we're using SQLite
            var isSqlite = context.Database.IsSqlite();
            
            if (isSqlite)
            {
                // For SQLite, check if database exists and create if needed
                // AVOID calling EnsureDeletedAsync() as it can fail if database is locked by another process
                logger.LogInformation("SQLite detected. Checking database schema...");
                
                // Check if tables exist by attempting to query a core table
                bool databaseExists = false;
                try
                {
                    // Try to query the Sites table - if it exists, database is already initialized
                    await context.Sites.AnyAsync();
                    databaseExists = true;
                    logger.LogInformation("Database already exists with valid schema.");
                }
                catch (Microsoft.Data.Sqlite.SqliteException)
                {
                    // Table doesn't exist, need to create the database
                    databaseExists = false;
                }
                
                if (!databaseExists)
                {
                    logger.LogInformation("Database does not exist or is incomplete. Creating schema...");
                    await context.Database.EnsureCreatedAsync();
                    logger.LogInformation("Database schema created successfully");
                }
            }
            else
            {
                // For SQL Server, use migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var hasAppliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).Any();
                
                if (hasAppliedMigrations || pendingMigrations.Any())
                {
                    // Database has migration history or pending migrations - use MigrateAsync
                    logger.LogInformation("Applying database migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully");
                }
                else
                {
                    // Fresh database without migration history - use EnsureCreatedAsync
                    logger.LogInformation("Creating database schema...");
                    await context.Database.EnsureCreatedAsync();
                    logger.LogInformation("Database schema created successfully");
                }
            }

            // Check if data already exists
            if (await context.Sites.AnyAsync())
            {
                logger.LogInformation("Database already seeded. Ensuring admin user is configured correctly...");
                // Database has been seeded, but we still need to ensure admin user exists
                // and is properly configured (this handles password resets and user fixes)
                await EnsureAdminUserAsync(context, logger);
                return;
            }

            logger.LogInformation("Starting database seeding...");
            await SeedThemesAsync(context);
            await SeedSiteAsync(context);
            await SeedMasterPagesAsync(context);
            await SeedModulesAsync(context);
            await SeedPagesAsync(context);
            await SeedNavigationAsync(context);
            await SeedPermissionsAndRolesAsync(context);
            await SeedAdminUserAsync(context);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    private static async Task SeedThemesAsync(ApplicationDbContext context)
    {
        var themes = new List<Theme>
        {
            new Theme
            {
                Name = "Orkinosai Professional",
                Description = "Modern, clean professional theme with blue and green color scheme",
                AssetsPath = "/css/themes/orkinosai-theme.css",
                Category = "Modern",
                LayoutType = "TopNavigation",
                PrimaryColor = "#0066cc",
                SecondaryColor = "#00a86b",
                AccentColor = "#ff6b35",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#0066cc",
                    SecondaryColor = "#00a86b",
                    AccentColor = "#ff6b35",
                    FontFamily = "Segoe UI, sans-serif"
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Theme
            {
                Name = "SharePoint Portal",
                Description = "SharePoint-inspired portal theme with left navigation and modern UI",
                AssetsPath = "/css/themes/sharepoint-portal-theme.css",
                Category = "SharePoint",
                LayoutType = "LeftNavigation",
                PrimaryColor = "#0078d4",
                SecondaryColor = "#005a9e",
                AccentColor = "#8764b8",
                ThumbnailUrl = "/images/themes/sharepoint-portal.png",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#0078d4",
                    SecondaryColor = "#005a9e",
                    AccentColor = "#8764b8",
                    FontFamily = "Segoe UI, sans-serif",
                    ShowQuickLaunch = true,
                    ShowSuiteBar = true
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Theme
            {
                Name = "Top Navigation",
                Description = "Clean modern theme with top horizontal navigation",
                AssetsPath = "/css/themes/top-navigation-theme.css",
                Category = "Modern",
                LayoutType = "TopNavigation",
                PrimaryColor = "#2563eb",
                SecondaryColor = "#059669",
                AccentColor = "#f59e0b",
                ThumbnailUrl = "/images/themes/top-navigation.png",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#2563eb",
                    SecondaryColor = "#059669",
                    AccentColor = "#f59e0b",
                    FontFamily = "Inter, sans-serif"
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Theme
            {
                Name = "Dashboard",
                Description = "Modern dashboard theme perfect for admin interfaces and data visualization",
                AssetsPath = "/css/themes/dashboard-theme.css",
                Category = "Dashboard",
                LayoutType = "LeftNavigation",
                PrimaryColor = "#6366f1",
                SecondaryColor = "#10b981",
                AccentColor = "#f59e0b",
                ThumbnailUrl = "/images/themes/dashboard.png",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#6366f1",
                    SecondaryColor = "#10b981",
                    AccentColor = "#f59e0b",
                    FontFamily = "Inter, sans-serif",
                    ShowSidebar = true,
                    DarkMode = false
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Theme
            {
                Name = "Minimal",
                Description = "Clean and simple design with focus on content and readability",
                AssetsPath = "/css/themes/minimal-theme.css",
                Category = "Minimal",
                LayoutType = "TopNavigation",
                PrimaryColor = "#000000",
                SecondaryColor = "#666666",
                AccentColor = "#0066cc",
                ThumbnailUrl = "/images/themes/minimal.png",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#000000",
                    SecondaryColor = "#666666",
                    AccentColor = "#0066cc",
                    FontFamily = "Helvetica Neue, Arial, sans-serif"
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Theme
            {
                Name = "Marketing Landing",
                Description = "Bold, conversion-focused theme perfect for marketing and landing pages",
                AssetsPath = "/css/themes/marketing-theme.css",
                Category = "Marketing",
                LayoutType = "TopNavigation",
                PrimaryColor = "#7c3aed",
                SecondaryColor = "#ec4899",
                AccentColor = "#06b6d4",
                ThumbnailUrl = "/images/themes/marketing.png",
                DefaultSettings = JsonSerializer.Serialize(new
                {
                    PrimaryColor = "#7c3aed",
                    SecondaryColor = "#ec4899",
                    AccentColor = "#06b6d4",
                    FontFamily = "Inter, sans-serif",
                    ShowHero = true,
                    ShowCTA = true
                }),
                IsEnabled = true,
                IsSystem = true,
                IsMobileResponsive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.Themes.AddRange(themes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMasterPagesAsync(ApplicationDbContext context)
    {
        var site = await context.Sites.FirstAsync();
        
        var masterPages = new List<MasterPage>
        {
            new MasterPage
            {
                SiteId = site.Id,
                Name = "Standard Layout",
                Description = "Standard page layout with header, navigation, main content, sidebar, and footer",
                ComponentPath = "/Components/MasterPages/StandardMasterPage.razor",
                ContentZones = JsonSerializer.Serialize(new[] 
                { 
                    "Header", 
                    "Navigation", 
                    "Main", 
                    "Sidebar", 
                    "Footer" 
                }),
                IsDefault = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new MasterPage
            {
                SiteId = site.Id,
                Name = "Full Width Layout",
                Description = "Full-width landing page layout with hero section and multi-column footer",
                ComponentPath = "/Components/MasterPages/FullWidthMasterPage.razor",
                ContentZones = JsonSerializer.Serialize(new[] 
                { 
                    "Header", 
                    "Navigation", 
                    "Hero", 
                    "Main", 
                    "FooterColumn1", 
                    "FooterColumn2", 
                    "FooterColumn3", 
                    "FooterColumn4" 
                }),
                IsDefault = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.MasterPages.AddRange(masterPages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedModulesAsync(ApplicationDbContext context)
    {
        var modules = new List<Module>
        {
            new Module
            {
                Name = "HtmlContent",
                Title = "HTML Content Module",
                Description = "Display rich HTML content",
                Version = "1.0.0",
                AssemblyName = "OrkinosaiCMS.Modules.Content",
                ComponentType = "OrkinosaiCMS.Modules.Content.HtmlContentModule",
                Category = "Content",
                IsEnabled = true,
                IsSystem = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Name = "Hero",
                Title = "Hero Section",
                Description = "Eye-catching hero section with title, subtitle, and call-to-action",
                Version = "1.0.0",
                AssemblyName = "OrkinosaiCMS.Modules.Hero",
                ComponentType = "OrkinosaiCMS.Modules.Hero.HeroModule",
                Category = "Marketing",
                IsEnabled = true,
                IsSystem = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Name = "Features",
                Title = "Features Grid",
                Description = "Display features in a responsive grid with icons",
                Version = "1.0.0",
                AssemblyName = "OrkinosaiCMS.Modules.Features",
                ComponentType = "OrkinosaiCMS.Modules.Features.FeaturesModule",
                Category = "Marketing",
                IsEnabled = true,
                IsSystem = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Module
            {
                Name = "ContactForm",
                Title = "Contact Form",
                Description = "Contact form with validation",
                Version = "1.0.0",
                AssemblyName = "OrkinosaiCMS.Modules.ContactForm",
                ComponentType = "OrkinosaiCMS.Modules.ContactForm.ContactFormModule",
                Category = "Forms",
                IsEnabled = true,
                IsSystem = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.Modules.AddRange(modules);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSiteAsync(ApplicationDbContext context)
    {
        var theme = await context.Themes.FirstAsync();

        var site = new Site
        {
            Name = "OrkinosaiCMS Demo Site",
            Url = "https://localhost:5001",
            Description = "A modern, modular Content Management System built on .NET 10 and Blazor",
            ThemeId = theme.Id,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Sites.Add(site);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPagesAsync(ApplicationDbContext context)
    {
        var site = await context.Sites.FirstAsync();
        var standardMaster = await context.MasterPages
            .FirstAsync(m => m.Name == "Standard Layout");
        var fullWidthMaster = await context.MasterPages
            .FirstAsync(m => m.Name == "Full Width Layout");

        var pages = new List<Page>
        {
            new Page
            {
                SiteId = site.Id,
                MasterPageId = fullWidthMaster.Id,
                Title = "Home - OrkinosaiCMS",
                Path = "/cms-home",
                MetaDescription = "Welcome to OrkinosaiCMS - A modern, modular Content Management System built on .NET 10 and Blazor",
                MetaKeywords = "CMS, .NET, Blazor, Content Management, OrkinosaiCMS",
                IsPublished = true,
                ShowInNavigation = true,
                Order = 1,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Page
            {
                SiteId = site.Id,
                MasterPageId = standardMaster.Id,
                Title = "About - OrkinosaiCMS",
                Path = "/cms-about",
                MetaDescription = "Learn about OrkinosaiCMS architecture, vision, and technology stack",
                MetaKeywords = "About, Architecture, Vision, Technology",
                IsPublished = true,
                ShowInNavigation = true,
                Order = 2,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Page
            {
                SiteId = site.Id,
                MasterPageId = standardMaster.Id,
                Title = "Contact - OrkinosaiCMS",
                Path = "/cms-contact",
                MetaDescription = "Get in touch with the OrkinosaiCMS team",
                MetaKeywords = "Contact, Support, Get in Touch",
                IsPublished = true,
                ShowInNavigation = true,
                Order = 3,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.Pages.AddRange(pages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAndRolesAsync(ApplicationDbContext context)
    {
        // Seed Permissions
        var permissions = new List<Permission>
        {
            new Permission { Name = "View", Description = "View content", CreatedOn = DateTime.UtcNow, CreatedBy = "System" },
            new Permission { Name = "Edit", Description = "Edit content", CreatedOn = DateTime.UtcNow, CreatedBy = "System" },
            new Permission { Name = "Delete", Description = "Delete content", CreatedOn = DateTime.UtcNow, CreatedBy = "System" },
            new Permission { Name = "Manage", Description = "Manage site settings", CreatedOn = DateTime.UtcNow, CreatedBy = "System" },
            new Permission { Name = "Publish", Description = "Publish content", CreatedOn = DateTime.UtcNow, CreatedBy = "System" },
            new Permission { Name = "Design", Description = "Modify site design", CreatedOn = DateTime.UtcNow, CreatedBy = "System" }
        };

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();

        // Seed Roles
        var roles = new List<Role>
        {
            new Role 
            { 
                Name = "Administrator", 
                Description = "Full control over the site", 
                IsSystem = true,
                CreatedOn = DateTime.UtcNow, 
                CreatedBy = "System" 
            },
            new Role 
            { 
                Name = "Designer", 
                Description = "Design and layout management", 
                IsSystem = true,
                CreatedOn = DateTime.UtcNow, 
                CreatedBy = "System" 
            },
            new Role 
            { 
                Name = "Editor", 
                Description = "Create, edit, and publish content", 
                IsSystem = true,
                CreatedOn = DateTime.UtcNow, 
                CreatedBy = "System" 
            },
            new Role 
            { 
                Name = "Contributor", 
                Description = "Create and edit own content", 
                IsSystem = true,
                CreatedOn = DateTime.UtcNow, 
                CreatedBy = "System" 
            },
            new Role 
            { 
                Name = "Reader", 
                Description = "View published content", 
                IsSystem = true,
                CreatedOn = DateTime.UtcNow, 
                CreatedBy = "System" 
            }
        };

        context.CmsRoles.AddRange(roles);
        await context.SaveChangesAsync();

        // Assign permissions to roles
        var adminRole = await context.CmsRoles.FirstAsync(r => r.Name == "Administrator");
        var allPermissions = await context.Permissions.ToListAsync();
        
        foreach (var permission in allPermissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Ensure admin user exists and is properly configured
    /// This method is called on every startup to fix any issues with the admin user
    /// </summary>
    private static async Task EnsureAdminUserAsync(ApplicationDbContext context, ILogger logger)
    {
        // Ensure Administrator role exists
        var adminRole = await context.CmsRoles.FirstOrDefaultAsync(r => r.Name == "Administrator");
        if (adminRole == null)
        {
            logger.LogWarning("Administrator role not found. Database needs full seeding.");
            // If no admin role, database needs full seeding - return
            return;
        }

        // Check if admin user already exists
        var adminUser = await context.CmsUsers.FirstOrDefaultAsync(u => u.Username == "admin");
        
        if (adminUser == null)
        {
            logger.LogWarning("Admin user not found. Creating new admin user with username 'admin' and password 'Admin@123'");
            // Admin user doesn't exist - create it
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(DEMO_ADMIN_PASSWORD);
            
            var newAdminUser = new User
            {
                Username = "admin",
                Email = "admin@orkinosaicms.local",
                DisplayName = "Administrator",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.CmsUsers.Add(newAdminUser);
            await context.SaveChangesAsync();
            
            // Assign Administrator role to the user
            var newUserRole = new UserRole
            {
                UserId = newAdminUser.Id,
                RoleId = adminRole.Id,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.CmsUserRoles.Add(newUserRole);
            await context.SaveChangesAsync();
            logger.LogInformation("Admin user created successfully");
            return;
        }

        // Admin user exists - verify and fix if needed
        bool needsUpdate = false;
        
        // Ensure user is active
        if (!adminUser.IsActive)
        {
            logger.LogWarning("Admin user is inactive. Activating...");
            adminUser.IsActive = true;
            needsUpdate = true;
        }
        
        // Ensure user is not deleted
        if (adminUser.IsDeleted)
        {
            logger.LogWarning("Admin user is marked as deleted. Restoring...");
            adminUser.IsDeleted = false;
            adminUser.DeletedOn = null;
            needsUpdate = true;
        }
        
        // Verify password is correct by attempting to verify it
        // If verification fails, reset the password to the demo admin password
        bool passwordNeedsReset = false;
        
        try
        {
            // Check if password hash is null, empty, or verification fails
            if (string.IsNullOrEmpty(adminUser.PasswordHash) || 
                !BCrypt.Net.BCrypt.Verify(DEMO_ADMIN_PASSWORD, adminUser.PasswordHash))
            {
                passwordNeedsReset = true;
            }
        }
        catch (Exception ex)
        {
            // If BCrypt.Verify throws (malformed hash), reset password
            logger.LogWarning(ex, "Failed to verify admin password hash. Password will be reset.");
            passwordNeedsReset = true;
        }
        
        if (passwordNeedsReset)
        {
            logger.LogWarning("Admin password verification failed or password hash is invalid. Resetting password to 'Admin@123'");
            // Password is incorrect, missing, or corrupt - reset it
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DEMO_ADMIN_PASSWORD);
            adminUser.ModifiedOn = DateTime.UtcNow;
            adminUser.ModifiedBy = "System";
            needsUpdate = true;
        }
        
        if (needsUpdate)
        {
            context.CmsUsers.Update(adminUser);
            await context.SaveChangesAsync();
            logger.LogInformation("Admin user updated successfully");
        }
        else
        {
            logger.LogInformation("Admin user verified - no updates needed");
        }
        
        // Ensure admin role is assigned
        var hasAdminRole = await context.CmsUserRoles
            .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
            
        if (!hasAdminRole)
        {
            logger.LogWarning("Admin user does not have Administrator role. Assigning...");
            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.CmsUserRoles.Add(userRole);
            await context.SaveChangesAsync();
            logger.LogInformation("Administrator role assigned to admin user");
        }
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        // Ensure Administrator role exists
        var adminRole = await context.CmsRoles.FirstOrDefaultAsync(r => r.Name == "Administrator");
        if (adminRole == null)
        {
            throw new InvalidOperationException("Administrator role not found. Ensure SeedPermissionsAndRolesAsync() is called before SeedAdminUserAsync().");
        }

        // Check if admin user already exists
        var adminUser = await context.CmsUsers.FirstOrDefaultAsync(u => u.Username == "admin");
        
        if (adminUser != null)
        {
            // Admin user exists - verify and fix if needed
            bool needsUpdate = false;
            
            // Ensure user is active
            if (!adminUser.IsActive)
            {
                adminUser.IsActive = true;
                needsUpdate = true;
            }
            
            // Ensure user is not deleted
            if (adminUser.IsDeleted)
            {
                adminUser.IsDeleted = false;
                adminUser.DeletedOn = null;
                needsUpdate = true;
            }
            
            // Verify password is correct by attempting to verify it
            // If verification fails, reset the password to the demo admin password
            bool passwordNeedsReset = false;
            
            try
            {
                // Check if password hash is null, empty, or verification fails
                if (string.IsNullOrEmpty(adminUser.PasswordHash) || 
                    !BCrypt.Net.BCrypt.Verify(DEMO_ADMIN_PASSWORD, adminUser.PasswordHash))
                {
                    passwordNeedsReset = true;
                }
            }
            catch
            {
                // If BCrypt.Verify throws (malformed hash), reset password
                passwordNeedsReset = true;
            }
            
            if (passwordNeedsReset)
            {
                // Password is incorrect, missing, or corrupt - reset it
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DEMO_ADMIN_PASSWORD);
                adminUser.ModifiedOn = DateTime.UtcNow;
                adminUser.ModifiedBy = "System";
                needsUpdate = true;
            }
            
            if (needsUpdate)
            {
                context.CmsUsers.Update(adminUser);
            }
            
            // Ensure admin role is assigned
            var hasAdminRole = await context.CmsUserRoles
                .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
                
            if (!hasAdminRole)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                context.CmsUserRoles.Add(userRole);
            }
            
            return; // Admin user exists and has been verified/fixed
        }

        // Admin user doesn't exist - create it
        // Password: DEMO_ADMIN_PASSWORD constant
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(DEMO_ADMIN_PASSWORD);
        
        var newAdminUser = new User
        {
            Username = "admin",
            Email = "admin@orkinosaicms.local",
            DisplayName = "Administrator",
            PasswordHash = hashedPassword,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.CmsUsers.Add(newAdminUser);
        
        // Assign Administrator role to the user
        var newUserRole = new UserRole
        {
            User = newAdminUser,
            RoleId = adminRole.Id,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.CmsUserRoles.Add(newUserRole);
        // SaveChanges will be called by the parent InitializeAsync method
    }

    private static async Task SeedNavigationAsync(ApplicationDbContext context)
    {
        // Create main navigation menu
        var mainMenu = new NavigationMenu
        {
            SiteId = 1,
            Name = "TopNavigation",
            Title = "Main Navigation",
            Description = "Primary top navigation menu",
            Location = "Top",
            IsEnabled = true,
            MaxDepth = 3,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.NavigationMenus.Add(mainMenu);
        await context.SaveChangesAsync(); // Save to get menu ID

        // Create navigation items
        var navItems = new List<NavigationItem>
        {
            new NavigationItem
            {
                MenuId = mainMenu.Id,
                Label = "Home",
                Url = "/cms-home",
                IconCssClass = "fas fa-home",
                Order = 0,
                IsEnabled = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new NavigationItem
            {
                MenuId = mainMenu.Id,
                Label = "About",
                Url = "/cms-about",
                IconCssClass = "fas fa-info-circle",
                Order = 1,
                IsEnabled = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new NavigationItem
            {
                MenuId = mainMenu.Id,
                Label = "Features",
                Url = "/cms-features",
                IconCssClass = "fas fa-star",
                Order = 2,
                IsEnabled = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new NavigationItem
            {
                MenuId = mainMenu.Id,
                Label = "Contact",
                Url = "/cms-contact",
                IconCssClass = "fas fa-envelope",
                Order = 3,
                IsEnabled = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new NavigationItem
            {
                MenuId = mainMenu.Id,
                Label = "Admin",
                Url = "/admin",
                IconCssClass = "fas fa-cog",
                Order = 4,
                IsEnabled = true,
                RequiredRoles = "Administrator",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        context.NavigationItems.AddRange(navItems);
    }
}
