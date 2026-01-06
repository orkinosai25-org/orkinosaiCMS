# Modern Page Designer

## Overview

The Modern Page Designer brings SharePoint-style drag-and-drop page editing capabilities to OrkinosaiCMS. It allows users to visually design pages using a flexible layout system with sections, columns, and content blocks.

## Features

### Page Layout System
- **Sections**: Organize content into horizontal sections/rows
- **Column Layouts**: Support for full-width, two-column, and three-column layouts
- **Content Blocks**: Multiple block types for different content

### Content Block Types
1. **Text Block** - Rich text content with HTML support
2. **Image Block** - Images with URL and alt text
3. **Hero Block** - Large banner with title, subtitle, and background image
4. **Video Block** - Video embed support
5. **Gallery Block** - Image galleries
6. **Cards Block** - Card-based content
7. **HTML Block** - Custom HTML content

### Pre-built Templates
Quick start templates for common page layouts:
- **Hero**: Full-width hero banner with title and call-to-action
- **Text + Image**: Two-column layout with text and image side-by-side
- **Gallery**: Image gallery layout
- **Cards**: Three-column card layout

## Getting Started

### Accessing the Page Designer

1. Navigate to **Admin Panel** → **Pages Management**
2. Click the **🎨 Design** button next to any page
3. You'll be taken to the Page Designer interface

### Designer Interface

The Page Designer has three main areas:

#### 1. Block Toolbar (Left Sidebar)
- **Templates**: Quick-start templates for common layouts
- **Block Types**: Individual content blocks you can add
- **Layout Actions**: Add new sections to your page

#### 2. Canvas (Center)
- Visual representation of your page
- Sections with columns showing your content blocks
- Section controls to reorder or change layout type
- Block previews with edit and delete actions

#### 3. Properties Panel (Right Sidebar)
- Edit selected block properties
- Change content, images, text, etc.
- Save or cancel changes

## Using the Page Designer

### Starting a New Page Design

**Option 1: Use a Template**
1. Click one of the template buttons in the Block Toolbar
2. The template structure will be automatically created
3. Edit blocks to customize content

**Option 2: Build From Scratch**
1. Click **Add Section** to create a new section
2. Choose section layout (Full Width, Two Columns, Three Columns)
3. Add blocks to each column
4. Configure block content in the Properties Panel

### Working with Sections

**Add a Section**
- Click the **➕ Add Section** button in the Block Toolbar
- New section appears at the bottom of your page

**Change Section Layout**
- Use the dropdown in section controls to switch between:
  - Full Width (1 column)
  - Two Columns (50/50 split)
  - Three Columns (33/33/33 split)

**Reorder Sections**
- Use ↑ and ↓ buttons in section controls to move sections up or down

**Delete a Section**
- Click the 🗑️ button in section controls
- **Warning**: This will delete all blocks in the section

### Working with Blocks

**Add a Block**
1. Click a block type button in the Block Toolbar
2. Block is added to the first section's first column
3. Click the ✏️ edit button to configure

**Edit a Block**
1. Click the ✏️ (edit) icon on the block
2. Properties Panel opens on the right
3. Modify block content
4. Click **Save Changes**

**Delete a Block**
- Click the 🗑️ icon on any block to remove it

### Block Configuration

#### Text Block
- **Content**: HTML-enabled rich text editor
- Supports paragraphs, headings, lists, etc.

#### Image Block
- **Image URL**: Path or URL to image
- **Alt Text**: Accessibility description

#### Hero Block
- **Title**: Main headline
- **Subtitle**: Supporting text
- **Background Image URL**: Hero background image

#### HTML Block
- **HTML Content**: Custom HTML code
- Full HTML and inline CSS support

## Publishing Your Page

### Save Draft
- Click **💾 Save Draft** to save without publishing
- Page remains unpublished

### Publish
- Click **🚀 Publish** to make page live
- Page becomes visible to visitors

### Preview
- Click **👁️ Preview** to see page as visitors will
- Opens the published page URL

## Tips and Best Practices

### Layout Design
1. **Start with a template** for faster design
2. **Use consistent column layouts** for visual harmony
3. **Limit columns to 3** for better mobile experience
4. **Group related content** in the same section

### Content Blocks
1. **Text blocks**: Keep paragraphs concise
2. **Images**: Use consistent aspect ratios
3. **Hero blocks**: Use high-quality background images
4. **HTML blocks**: Test custom code thoroughly

### Performance
1. **Optimize images** before uploading
2. **Limit blocks per section** to 5-7 for best performance
3. **Use semantic HTML** in HTML blocks

## Keyboard Shortcuts

Currently, the Page Designer uses mouse/touch interactions. Keyboard shortcuts are planned for a future release.

## Responsive Design

The Page Designer creates layouts that are automatically responsive:
- **Desktop**: Full multi-column layouts
- **Tablet**: Sections may stack at medium breakpoints
- **Mobile**: All columns stack vertically

## Troubleshooting

### Block Not Saving
- Ensure all required fields are filled
- Check browser console for errors
- Try refreshing the page and re-editing

### Preview Not Showing Changes
- Make sure you clicked **Save Changes** on the block
- Click **💾 Save Draft** to persist to database
- Refresh the preview window

### Layout Looking Different in Preview
- Some CSS may affect block rendering
- Check custom HTML blocks for conflicting styles
- Ensure theme compatibility

## Future Enhancements

Planned features for future releases:
- **Interactive drag-and-drop** for blocks and sections
- **Rich text editor** with WYSIWYG toolbar
- **Image upload** directly from designer
- **Image cropping and resizing** tools
- **Block library** with saved custom blocks
- **Revision history** and version control
- **Mobile/responsive preview** within designer
- **Keyboard navigation** and shortcuts
- **Undo/redo** functionality
- **Copy/paste** blocks between pages
- **Custom block types** via plugins

## Technical Details

### Data Model
Pages using the Modern Designer store their layout in three entities:
- **PageLayout**: Container for page design
- **PageSection**: Horizontal sections with column configuration
- **PageBlock**: Individual content blocks with JSON data

### Block Content Format
Block content is stored as JSON:
```json
{
  "html": "<p>Text content</p>"
}
```

```json
{
  "title": "Hero Title",
  "subtitle": "Subtitle text",
  "imageUrl": "/images/hero.jpg"
}
```

### Service API
- `IPageLayoutService`: Manages layouts, sections, and blocks
- `CreateLayoutAsync()`: Create new page layout
- `ApplyTemplateAsync()`: Apply predefined template
- `CreateBlockAsync()`: Add new content block
- `UpdateBlockAsync()`: Modify block content

## Support

For issues or questions:
- Check the [OrkinosaiCMS Documentation](../README.md)
- Create an issue on [GitHub](https://github.com/orkinosai25-org/orkinosaiCMS/issues)
- Contact the development team

## See Also

- [Page Management](PAGES.md) - Managing pages in OrkinosaiCMS
- [Master Pages](MASTER_PAGES.md) - Understanding page templates
- [Modules](EXTENSIBILITY.md) - Adding custom functionality
- [Themes](THEMES.md) - Customizing visual appearance
