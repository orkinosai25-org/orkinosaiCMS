# Navigation Management System

OrkinosaiCMS provides a flexible, SharePoint-inspired navigation management system that allows administrators to create and manage hierarchical navigation menus for their websites.

## Features

### Navigation Management
- **Multiple Menus**: Create multiple navigation menus per site (e.g., Top Navigation, Footer, Quick Launch)
- **Hierarchical Structure**: Support for multi-level navigation (up to 3 levels deep by default, configurable)
- **Drag-and-Drop Interface**: Visual editor for managing navigation items (UI foundation in place)
- **Role-Based Visibility**: Control which navigation items appear for different user roles
- **Icon Support**: Add icons to navigation items using CSS classes (Font Awesome, Bootstrap Icons, etc.)
- **Custom Styling**: Apply CSS classes to individual items or entire menus
- **Enable/Disable**: Easily enable or disable menus and items without deletion

### Data Model

#### NavigationMenu Entity
Represents a container for navigation items with the following properties:
- **SiteId**: Site this menu belongs to
- **Name**: Unique identifier (e.g., "TopNavigation")
- **Title**: Display title for the menu
- **Description**: Optional description
- **Location**: Placement (Top, Left, Right, Footer, QuickLaunch)
- **IsEnabled**: Whether the menu is active
- **CssClass**: Optional CSS class for styling
- **MaxDepth**: Maximum hierarchical depth allowed

#### NavigationItem Entity
Represents an individual navigation item with:
- **MenuId**: Parent menu reference
- **ParentId**: Parent item for hierarchical structure (null for root items)
- **Label**: Display text
- **Url**: Target URL or path
- **PageId**: Optional reference to a CMS page
- **IconCssClass**: Icon CSS class (e.g., "fas fa-home")
- **Order**: Display order within parent/menu
- **IsEnabled**: Whether the item is visible
- **OpenInNewWindow**: Open link in new tab
- **CssClass**: Custom CSS classes
- **Description**: Tooltip/description text
- **RequiredRoles**: Comma-separated list of required roles
- **RequiredPermission**: Permission required to view
- **CustomAttributes**: JSON for additional attributes

## Usage

### Admin Interface

#### Access Navigation Management
1. Log in as an Administrator
2. Navigate to `/admin/navigation`
3. View all navigation menus for the site

#### Create a Navigation Menu
1. Click "Create Menu" button
2. Fill in the form:
   - **Menu Name**: Unique identifier (e.g., "TopNavigation")
   - **Display Title**: User-friendly name
   - **Description**: Optional description
   - **Location**: Select placement (Top, Left, Right, Footer, QuickLaunch)
   - **Maximum Depth**: Set hierarchical levels (1-5)
   - **CSS Class**: Optional styling class
   - **Enabled**: Check to enable the menu
3. Click "Save Menu"

#### Manage Navigation Items
1. From the navigation management page, click "Manage Items" on a menu
2. Click "Add Item" to create a new navigation item
3. Fill in the item form:
   - **Label**: Display text for the item
   - **URL**: Target URL or path
   - **Icon CSS Class**: Optional icon (e.g., "fas fa-home")
   - **Order**: Display order (lower numbers first)
   - **Description**: Optional tooltip text
   - **CSS Class**: Optional styling classes
   - **Required Roles**: Comma-separated roles (e.g., "Administrator,Editor")
   - **Enabled**: Check to make visible
   - **Open in New Window**: Check to open in new tab
4. Click "Save Item"

#### Create Sub-Items (Hierarchical Navigation)
1. From the navigation items page, click "➕" next to an existing item
2. This creates a child item under the selected parent
3. Fill in the child item details and save

#### Edit or Delete Items
- Click "✏️" to edit an item
- Click "🗑️" to delete an item
- Changes reflect immediately in the UI

### Front-End Rendering

#### Using NavigationRenderer Component
Add navigation to any Razor component or page:

```razor
@using OrkinosaiCMS.Web.Components.Navigation

<NavigationRenderer MenuName="TopNavigation" 
                   SiteId="1" 
                   CssClass="custom-nav" 
                   MaxDepth="3" />
```

Parameters:
- **MenuName**: Name of the menu to render (e.g., "TopNavigation")
- **SiteId**: Site ID (default: 1)
- **CssClass**: Additional CSS classes for the nav element
- **MaxDepth**: Maximum depth to render (default: 3)

#### Styling Navigation
The navigation renderer includes default styles for different locations:
- **top**: Horizontal navigation bar
- **left/right**: Vertical sidebar navigation
- **footer**: Horizontal footer navigation with wrapping

Override styles by targeting CSS classes:
```css
.navigation-menu.top {
    /* Top navigation styles */
}

.nav-item {
    /* Individual item styles */
}

.nav-submenu {
    /* Submenu styles */
}
```

## Service Layer

### INavigationService
The navigation service provides methods for CRUD operations:

#### Menu Operations
```csharp
Task<NavigationMenu?> GetMenuByIdAsync(int id);
Task<NavigationMenu?> GetMenuByNameAsync(string name, int siteId);
Task<IEnumerable<NavigationMenu>> GetMenusBySiteAsync(int siteId);
Task<NavigationMenu> CreateMenuAsync(NavigationMenu menu);
Task<NavigationMenu> UpdateMenuAsync(NavigationMenu menu);
Task DeleteMenuAsync(int id);
```

#### Item Operations
```csharp
Task<NavigationItem?> GetItemByIdAsync(int id);
Task<IEnumerable<NavigationItem>> GetItemsByMenuAsync(int menuId);
Task<IEnumerable<NavigationItem>> GetRootItemsAsync(int menuId);
Task<IEnumerable<NavigationItem>> GetChildItemsAsync(int parentId);
Task<NavigationItem> CreateItemAsync(NavigationItem item);
Task<NavigationItem> UpdateItemAsync(NavigationItem item);
Task DeleteItemAsync(int id);
```

#### Hierarchy Operations
```csharp
Task ReorderItemAsync(int itemId, int newOrder);
Task MoveItemAsync(int itemId, int? newParentId, int newOrder);
Task<IEnumerable<NavigationItem>> GetItemHierarchyAsync(int menuId);
```

#### Rendering Operations (with permission filtering)
```csharp
Task<IEnumerable<NavigationItem>> GetVisibleItemsAsync(int menuId, string? userRoles = null);
Task<IEnumerable<NavigationItem>> GetVisibleHierarchyAsync(int menuId, string? userRoles = null);
```

## Database Schema

### Tables
- **NavigationMenus**: Stores navigation menu definitions
- **NavigationItems**: Stores individual navigation items with hierarchical relationships

### Key Relationships
- NavigationMenu → Site (many-to-one)
- NavigationItem → NavigationMenu (many-to-one)
- NavigationItem → NavigationItem (self-referencing for hierarchy)
- NavigationItem → Page (optional one-to-one)

### Soft Delete
Both NavigationMenu and NavigationItem entities support soft delete through the BaseEntity `IsDeleted` property.

## Seeded Data

The system includes a default "TopNavigation" menu with sample items:
- Home (/cms-home)
- About (/cms-about)
- Features (/cms-features)
- Contact (/cms-contact)
- Admin (/admin) - visible to Administrators only

## Frontend Integration

### Dynamic Navigation (December 2025 Update)

The CMS now features **fully dynamic navigation** that pulls menu items directly from the database. Changes made in the admin panel reflect immediately on the frontend without requiring code changes or redeployment.

#### CMSNavigation Component

The main site navigation now uses the `CMSNavigation` component which automatically renders the TopNavigation menu with user authentication controls:

```razor
<CMSNavigation CurrentPage="home" />
```

This component:
- Automatically loads the "TopNavigation" menu from the database
- Displays navigation items based on user roles
- Shows login/logout controls
- Displays the current user's name when authenticated
- Is mobile-responsive by default

#### Legacy Pages Deprecated

The following legacy Blazor sample pages are no longer included in the navigation:
- `/counter` - Counter demo page (still accessible directly)
- `/weather` - Weather demo page (still accessible directly)

These pages remain in the codebase for reference but are not linked in the CMS navigation. To add them back, simply create navigation items pointing to these URLs in the Navigation Management admin panel.

#### How Dynamic Navigation Works

1. **Menu Definition**: Administrators define menus in `/admin/navigation`
2. **Database Storage**: Menu and item data is stored in NavigationMenus and NavigationItems tables
3. **Automatic Loading**: NavigationRenderer component queries the database on page load
4. **Role Filtering**: Items are filtered based on user roles and permissions
5. **Hierarchy Building**: The service builds the complete navigation tree
6. **Rendering**: The component renders the navigation with appropriate styling

#### No Code Changes Required

To modify navigation:
1. ✅ **Just use the admin panel** - No need to edit Razor files
2. ✅ **Immediate updates** - Changes appear instantly on the frontend
3. ✅ **No redeployment** - All navigation is database-driven
4. ✅ **Designer-friendly** - Non-developers can manage navigation

## Best Practices

### Navigation Design
1. **Keep it simple**: Limit to 3 levels of depth for better UX
2. **Use meaningful labels**: Clear, concise navigation text
3. **Logical grouping**: Group related pages under parent items
4. **Icon usage**: Use icons consistently across the menu
5. **Mobile considerations**: Test navigation on mobile devices

### Performance
1. **Caching**: The navigation service doesn't cache by default - implement caching in the application layer if needed
2. **Lazy loading**: Only load expanded menu items when needed
3. **Minimize hierarchy**: Deep hierarchies can impact render performance

### Security
1. **Role-based visibility**: Use RequiredRoles to control access
2. **Permission checks**: Leverage RequiredPermission for fine-grained control
3. **Validate URLs**: Ensure navigation URLs are safe and valid

## Extensibility

### Custom Navigation Renderers
Create custom navigation components by extending NavigationRenderer:
```razor
@inherits NavigationRenderer

@* Custom rendering logic *@
```

### Custom Attributes
Use the CustomAttributes field (JSON) to store additional metadata:
```csharp
item.CustomAttributes = JsonSerializer.Serialize(new {
    Badge = "New",
    Color = "#ff0000",
    CustomField = "value"
});
```

### Event Hooks
Future versions will include event hooks for:
- OnMenuCreated
- OnItemAdded
- OnNavigationRendered

## Future Enhancements

Planned features:
- [ ] Drag-and-drop reordering in the admin UI
- [ ] Navigation preview before publishing
- [ ] Import/export navigation configurations
- [ ] Navigation analytics and tracking
- [ ] A/B testing for navigation layouts
- [ ] Mega-menu support for large sites
- [ ] Breadcrumb component
- [ ] Sitemap generation from navigation

## Troubleshooting

### Navigation not appearing
1. Check if the menu is enabled
2. Verify the menu name matches the renderer
3. Ensure items are enabled
4. Check user roles if using role-based visibility

### Items out of order
- Update the Order property for items
- Use the ReorderItemAsync method
- Rebuild the navigation hierarchy

### Permission issues
- Verify RequiredRoles are comma-separated
- Check user authentication state
- Ensure role names match exactly

## Support

For issues or questions:
- Check the [GitHub Issues](https://github.com/orkinosai25-org/orkinosaiCMS/issues)
- Review the [Architecture Guide](ARCHITECTURE.md)
- Consult the [Extensibility Guide](EXTENSIBILITY.md)
