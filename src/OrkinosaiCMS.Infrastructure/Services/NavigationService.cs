using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Navigation;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service implementation for navigation management operations
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IRepository<NavigationMenu> _menuRepository;
    private readonly IRepository<NavigationItem> _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public NavigationService(
        IRepository<NavigationMenu> menuRepository,
        IRepository<NavigationItem> itemRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _menuRepository = menuRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    #region Navigation Menu Operations

    public async Task<NavigationMenu?> GetMenuByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.NavigationMenus
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<NavigationMenu?> GetMenuByNameAsync(string name, int siteId, CancellationToken cancellationToken = default)
    {
        return await _menuRepository.FirstOrDefaultAsync(
            m => m.Name == name && m.SiteId == siteId, cancellationToken);
    }

    public async Task<IEnumerable<NavigationMenu>> GetMenusBySiteAsync(int siteId, CancellationToken cancellationToken = default)
    {
        return await _menuRepository.FindAsync(m => m.SiteId == siteId, cancellationToken);
    }

    public async Task<NavigationMenu> CreateMenuAsync(NavigationMenu menu, CancellationToken cancellationToken = default)
    {
        menu.CreatedOn = DateTime.UtcNow;
        await _menuRepository.AddAsync(menu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return menu;
    }

    public async Task<NavigationMenu> UpdateMenuAsync(NavigationMenu menu, CancellationToken cancellationToken = default)
    {
        menu.ModifiedOn = DateTime.UtcNow;
        _menuRepository.Update(menu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return menu;
    }

    public async Task DeleteMenuAsync(int id, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
        if (menu != null)
        {
            _menuRepository.Remove(menu);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Navigation Item Operations

    public async Task<NavigationItem?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.NavigationItems
            .Include(i => i.Children)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<NavigationItem>> GetItemsByMenuAsync(int menuId, CancellationToken cancellationToken = default)
    {
        return await _itemRepository.FindAsync(i => i.MenuId == menuId, cancellationToken);
    }

    public async Task<IEnumerable<NavigationItem>> GetRootItemsAsync(int menuId, CancellationToken cancellationToken = default)
    {
        return await _context.NavigationItems
            .Where(i => i.MenuId == menuId && i.ParentId == null)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NavigationItem>> GetChildItemsAsync(int parentId, CancellationToken cancellationToken = default)
    {
        return await _context.NavigationItems
            .Where(i => i.ParentId == parentId)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<NavigationItem> CreateItemAsync(NavigationItem item, CancellationToken cancellationToken = default)
    {
        item.CreatedOn = DateTime.UtcNow;
        await _itemRepository.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<NavigationItem> UpdateItemAsync(NavigationItem item, CancellationToken cancellationToken = default)
    {
        item.ModifiedOn = DateTime.UtcNow;
        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task DeleteItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            _itemRepository.Remove(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Hierarchy Operations

    public async Task ReorderItemAsync(int itemId, int newOrder, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Navigation item with ID {itemId} not found.");
        }

        item.Order = newOrder;
        item.ModifiedOn = DateTime.UtcNow;

        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveItemAsync(int itemId, int? newParentId, int newOrder, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item == null)
        {
            throw new ArgumentException($"Navigation item with ID {itemId} not found.");
        }

        // Validate that we're not creating a circular reference
        if (newParentId.HasValue)
        {
            var newParent = await _itemRepository.GetByIdAsync(newParentId.Value, cancellationToken);
            if (newParent == null)
            {
                throw new ArgumentException($"Parent navigation item with ID {newParentId} not found.");
            }

            // Check for circular reference
            if (await IsDescendantOf(newParentId.Value, itemId, cancellationToken))
            {
                throw new InvalidOperationException("Cannot move item to one of its descendants.");
            }
        }

        item.ParentId = newParentId;
        item.Order = newOrder;
        item.ModifiedOn = DateTime.UtcNow;

        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<NavigationItem>> GetItemHierarchyAsync(int menuId, CancellationToken cancellationToken = default)
    {
        var allItems = await _context.NavigationItems
            .Where(i => i.MenuId == menuId)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);

        return BuildHierarchy(allItems, null);
    }

    #endregion

    #region Rendering Operations

    public async Task<IEnumerable<NavigationItem>> GetVisibleItemsAsync(int menuId, string? userRoles = null, CancellationToken cancellationToken = default)
    {
        var items = await _context.NavigationItems
            .Where(i => i.MenuId == menuId && i.IsEnabled)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);

        return FilterByPermissions(items, userRoles);
    }

    public async Task<IEnumerable<NavigationItem>> GetVisibleHierarchyAsync(int menuId, string? userRoles = null, CancellationToken cancellationToken = default)
    {
        var allItems = await _context.NavigationItems
            .Where(i => i.MenuId == menuId && i.IsEnabled)
            .OrderBy(i => i.Order)
            .ToListAsync(cancellationToken);

        var filteredItems = FilterByPermissions(allItems, userRoles).ToList();
        return BuildHierarchy(filteredItems, null);
    }

    #endregion

    #region Private Helper Methods

    private async Task<bool> IsDescendantOf(int potentialDescendantId, int ancestorId, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(potentialDescendantId, cancellationToken);

        while (item?.ParentId != null)
        {
            if (item.ParentId == ancestorId)
            {
                return true;
            }
            item = await _itemRepository.GetByIdAsync(item.ParentId.Value, cancellationToken);
        }

        return false;
    }

    private List<NavigationItem> BuildHierarchy(List<NavigationItem> items, int? parentId)
    {
        var result = new List<NavigationItem>();

        var children = items.Where(i => i.ParentId == parentId).OrderBy(i => i.Order).ToList();

        foreach (var child in children)
        {
            child.Children = BuildHierarchy(items, child.Id);
            result.Add(child);
        }

        return result;
    }

    private IEnumerable<NavigationItem> FilterByPermissions(IEnumerable<NavigationItem> items, string? userRoles)
    {
        if (string.IsNullOrEmpty(userRoles))
        {
            return items.Where(i => string.IsNullOrEmpty(i.RequiredRoles));
        }

        var roles = userRoles.Split(',').Select(r => r.Trim()).ToHashSet();

        return items.Where(i =>
        {
            if (string.IsNullOrEmpty(i.RequiredRoles))
            {
                return true; // No role requirement, visible to all
            }

            var requiredRoles = i.RequiredRoles.Split(',').Select(r => r.Trim());
            return requiredRoles.Any(r => roles.Contains(r));
        });
    }

    #endregion
}
