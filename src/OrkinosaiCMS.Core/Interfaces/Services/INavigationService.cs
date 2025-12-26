using OrkinosaiCMS.Core.Entities.Navigation;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service interface for navigation management operations
/// </summary>
public interface INavigationService
{
    // Navigation Menu operations
    Task<NavigationMenu?> GetMenuByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<NavigationMenu?> GetMenuByNameAsync(string name, int siteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationMenu>> GetMenusBySiteAsync(int siteId, CancellationToken cancellationToken = default);
    Task<NavigationMenu> CreateMenuAsync(NavigationMenu menu, CancellationToken cancellationToken = default);
    Task<NavigationMenu> UpdateMenuAsync(NavigationMenu menu, CancellationToken cancellationToken = default);
    Task DeleteMenuAsync(int id, CancellationToken cancellationToken = default);

    // Navigation Item operations
    Task<NavigationItem?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationItem>> GetItemsByMenuAsync(int menuId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationItem>> GetRootItemsAsync(int menuId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationItem>> GetChildItemsAsync(int parentId, CancellationToken cancellationToken = default);
    Task<NavigationItem> CreateItemAsync(NavigationItem item, CancellationToken cancellationToken = default);
    Task<NavigationItem> UpdateItemAsync(NavigationItem item, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(int id, CancellationToken cancellationToken = default);

    // Hierarchy operations
    Task ReorderItemAsync(int itemId, int newOrder, CancellationToken cancellationToken = default);
    Task MoveItemAsync(int itemId, int? newParentId, int newOrder, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationItem>> GetItemHierarchyAsync(int menuId, CancellationToken cancellationToken = default);

    // Rendering operations (with permission filtering)
    Task<IEnumerable<NavigationItem>> GetVisibleItemsAsync(int menuId, string? userRoles = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<NavigationItem>> GetVisibleHierarchyAsync(int menuId, string? userRoles = null, CancellationToken cancellationToken = default);
}
