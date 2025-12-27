# Quick Start Guide for Designers

## Welcome to OrkinosaiCMS!

This guide will help designers and site builders get started with OrkinosaiCMS navigation and content management without needing to write code.

## Logging In

1. Navigate to `/admin/login`
2. Use the default credentials:
   - **Username**: `admin`
   - **Password**: `Admin@123`
   - ⚠️ **Important**: Change these credentials after first login in production!

## Admin Panel Overview

After logging in, you'll see the Admin Panel sidebar with these sections:

- 🏠 **Dashboard** - Overview and quick stats
- 📄 **Pages** - Manage CMS pages
- 🧭 **Navigation** - Manage site menus ⭐ *Start here!*
- 📝 **Content** - Content management
- 🖼️ **Media** - Upload and manage images/files
- 🎨 **Themes** - Site appearance settings
- 👥 **Users** - User management
- ⚙️ **Settings** - Site configuration

## Managing Navigation (Designer-Friendly!)

### Step 1: Access Navigation Management

Click **🧭 Navigation** in the admin sidebar.

### Step 2: View Your Menus

You'll see a list of navigation menus. By default, you'll have:
- **TopNavigation** - The main site menu

### Step 3: Add or Edit Menu Items

1. Click **📝 Manage Items** on the TopNavigation menu
2. You'll see the current navigation structure in a tree view
3. To add a new item:
   - Click **➕ Add Item** button
   - Fill in:
     - **Label**: Text shown in menu (e.g., "Our Services")
     - **URL**: Where it links (e.g., `/services` or `https://example.com`)
     - **Icon**: Optional icon class (e.g., `fas fa-briefcase`)
     - **Order**: Position in menu (0 = first)
   - Click **Save Item**

### Step 4: Create Sub-Menus (Drop-downs)

Want nested menus? Easy!

1. Find the parent menu item
2. Click the **➕** icon next to it
3. Fill in the child item details
4. Click **Save Item**

The child item will appear indented under its parent.

### Step 5: Reorder Items

Change the **Order** value when editing items:
- 0 = First position
- 1 = Second position
- And so on...

### Step 6: Hide Items Without Deleting

Want to temporarily hide an item?
1. Edit the item
2. Uncheck **Item is enabled**
3. Save

The item stays in your navigation but won't show on the site.

## Common Tasks for Designers

### Creating a Services Menu

```
Services (parent)
├── Web Design
├── Branding
└── Marketing
```

1. Create "Services" item with URL `/services`
2. Click ➕ next to "Services"
3. Add "Web Design" as child
4. Repeat for other services

### Adding Footer Navigation

1. Click **Create Menu** on the Navigation page
2. Set:
   - **Menu Name**: `FooterNavigation`
   - **Location**: `Footer`
   - **Maximum Depth**: `1` (no sub-menus)
3. Add footer links (Privacy, Terms, Contact, etc.)
4. Use this menu in your footer template with:
   ```razor
   <NavigationRenderer MenuName="FooterNavigation" SiteId="1" />
   ```

### Restricting Admin Links

Want an "Admin" link only visible to administrators?

1. Add navigation item with:
   - **Label**: `Admin`
   - **URL**: `/admin`
   - **Required Roles**: `Administrator`
2. This link will only show when an admin is logged in!

### Adding External Links

Link to external sites:
1. Use full URL: `https://example.com`
2. Check **Open in new window** if you want it to open in a new tab
3. Add icon: `fas fa-external-link-alt`

## Icon Guide

OrkinosaiCMS supports Font Awesome and Bootstrap Icons:

### Common Font Awesome Icons
- Home: `fas fa-home`
- About: `fas fa-info-circle`
- Contact: `fas fa-envelope`
- Phone: `fas fa-phone`
- Email: `fas fa-at`
- Location: `fas fa-map-marker-alt`
- Settings: `fas fa-cog`
- User: `fas fa-user`
- Search: `fas fa-search`

### Bootstrap Icons
- Home: `bi bi-house`
- About: `bi bi-info-circle`
- Contact: `bi bi-envelope`
- Phone: `bi bi-telephone`

Browse more icons at:
- Font Awesome: https://fontawesome.com/icons
- Bootstrap Icons: https://icons.getbootstrap.com/

## Testing Your Changes

1. Save your navigation changes in the admin panel
2. Open your site in a new tab (or refresh)
3. Your changes appear immediately! ✨

No code changes, no deployment needed!

## Best Practices for Designers

### Keep It Simple
- 5-7 top-level items maximum
- No more than 3 levels deep
- Use clear, concise labels

### Logical Organization
```
Good ✅                      Bad ❌
Home                        Home
About                       About Us | Our Story
  - Our Story              Products
  - Team                     - Category 1
Services                       - Sub 1
  - Web Design                   - Sub Sub 1
  - Branding                - Category 2
Contact                    Resources
```

### Mobile-First Thinking
- Navigation is responsive by default
- Test on mobile devices
- Keep labels short
- Use icons to save space

### Consistent Style
- Use the same icon style (all Font Awesome or all Bootstrap Icons)
- Follow a naming pattern (all Title Case or all lower case)
- Group related items under parent menus

## Page Management (Coming Soon)

The **Pages** section will let you:
- Create new pages visually
- Assign layouts and themes
- Add content modules
- Set page permissions
- Manage metadata

## Theme Management (Coming Soon)

The **Themes** section will let you:
- Switch site themes
- Customize colors
- Upload custom themes
- Preview theme changes

## Need Help?

### Common Questions

**Q: Where do my pages link to?**
A: Create pages in the **Pages** section, then link to them in Navigation using their path (e.g., `/my-new-page`)

**Q: Can I have multiple menus?**
A: Yes! Create as many menus as you need (Header, Footer, Sidebar, etc.)

**Q: Do I need to know coding?**
A: No! All navigation management is visual and designer-friendly.

**Q: Can I undo changes?**
A: Currently, changes are immediate. Best practice: Test in a staging environment first.

### Resources

- [Full Navigation Documentation](./NAVIGATION_MANAGEMENT.md)
- [Architecture Guide](./ARCHITECTURE.md)
- [GitHub Issues](https://github.com/orkinosai25-org/orkinosaiCMS/issues)

### Support

- Email: info@orkinosai.com
- GitHub: https://github.com/orkinosai25-org/orkinosaiCMS

---

**Happy building! 🚀**

*OrkinosaiCMS - Enterprise-ready CMS built on .NET 10 and Blazor*
