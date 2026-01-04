using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Identity;
using OrkinosaiCMS.Core.Entities.Media;
using OrkinosaiCMS.Core.Entities.Navigation;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Entities.Subscriptions;

namespace OrkinosaiCMS.Infrastructure.Data;

/// <summary>
/// Main database context for the OrkinosaiCMS with ASP.NET Core Identity integration
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Sites
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<MasterPage> MasterPages => Set<MasterPage>();

    // Modules
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<PageModule> PageModules => Set<PageModule>();

    // Themes
    public DbSet<Theme> Themes => Set<Theme>();

    // Navigation
    public DbSet<NavigationMenu> NavigationMenus => Set<NavigationMenu>();
    public DbSet<NavigationItem> NavigationItems => Set<NavigationItem>();

    // Users and Permissions (Legacy - will coexist with Identity)
    // Named differently to avoid conflicts with Identity tables
    public DbSet<User> CmsUsers => Set<User>();
    public DbSet<Role> CmsRoles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> CmsUserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Content
    public DbSet<Content> Contents => Set<Content>();

    // Subscriptions
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    // Media
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<MediaFolder> MediaFolders => Set<MediaFolder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure soft delete filters
        modelBuilder.Entity<Site>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Page>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MasterPage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Module>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PageModule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Theme>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Permission>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RolePermission>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Content>().HasQueryFilter(e => !e.IsDeleted);
        
        // Subscription entities soft delete filters
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Subscription>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Invoice>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PaymentMethod>().HasQueryFilter(e => !e.IsDeleted);
        
        // Navigation entities soft delete filters
        modelBuilder.Entity<NavigationMenu>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<NavigationItem>().HasQueryFilter(e => !e.IsDeleted);
        
        // Identity entities soft delete filter
        modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => !e.IsDeleted);
        
        // Media entities soft delete filters
        modelBuilder.Entity<MediaFile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MediaFolder>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set CreatedOn and ModifiedOn
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Core.Common.BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Core.Common.BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
            
            if (entry.State == EntityState.Modified)
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
