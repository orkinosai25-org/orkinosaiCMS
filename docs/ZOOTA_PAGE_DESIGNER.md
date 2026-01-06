# Zoota Page Designer Integration

## Overview

Zoota, the AI assistant in OrkinosaiCMS, can now help users design and build pages conversationally. Users can ask Zoota to create pages, add layouts, insert content blocks, add images, and generate content—all through natural language commands.

**🎨 NEW: AI Image Generation** - Zoota can now generate custom images using DALL-E and automatically add them to pages as banners!

## Available Commands

### AI Image Generation 🆕

**Generate custom banner images:**
- "Generate a banner with an ocean and an albatross gliding close to water"
- "Create a hero image showing a sunset over mountains"
- "Make a banner with a futuristic city skyline"
- "Generate a professional office background image"

**How it works:**
1. User describes the desired image
2. Zoota generates the image using DALL-E 3
3. Image is automatically saved to the media library
4. Image is added to the page as a hero/banner block

**API Endpoint:** `POST /api/zoota/cms/pages/{pageId}/generate-banner`

### Page Creation

**Create a new page:**
- "Create a new page called 'About Us'"
- "Make a page for our services"
- "Add a contact page"

**API Endpoint:** `POST /api/zoota/cms/pages`

### Layout Templates

**Apply predefined templates:**
- "Apply a hero template to the About page"
- "Use the gallery layout for my portfolio page"
- "Add a text and image layout"
- "Set up a cards layout"

**Available Templates:**
- `hero` - Full-width hero banner
- `gallery` - Image gallery layout
- `text-image` - Two-column text and image
- `cards` - Three-column card layout

**API Endpoint:** `POST /api/zoota/cms/pages/{pageId}/layout/template`

### Section Management

**Add sections:**
- "Add a new section to the page"
- "Create a two-column section"
- "Add a three-column layout section"

**Section Types:**
- `full-width` - Single column (100%)
- `two-column` - 50/50 split
- `three-column` - 33/33/33 split

**API Endpoint:** `POST /api/zoota/cms/pages/{pageId}/layout/sections`

### Content Blocks

**Add blocks:**
- "Add a text block"
- "Insert an image"
- "Create a hero banner"
- "Add a video"
- "Insert custom HTML"

**Block Types:**
- `text` - Rich text content
- `image` - Image with URL and alt text
- `hero` - Hero banner with title, subtitle, and background
- `video` - Video embed
- `gallery` - Image gallery
- `cards` - Card-based content
- `html` - Custom HTML

**API Endpoint:** `POST /api/zoota/cms/pages/{pageId}/layout/sections/{sectionId}/blocks`

### Content Generation

**Update block content:**
- "Change the hero title to 'Welcome to Our Site'"
- "Update the text to say..."
- "Set the image URL to..."

**API Endpoint:** `PUT /api/zoota/cms/pages/{pageId}/layout/blocks/{blockId}`

## Example Workflows

### Workflow 1: AI-Generated Banner Image 🆕

```
User: "Create a landing page"
Zoota: [Creates page] "Landing page created. Would you like to add content?"

User: "Generate a banner with an ocean and an albatross gliding close to water, we see from the side, image half sea half albatross, add to top of page as a banner"
Zoota: [Generates image with DALL-E, saves to media library, creates hero section with image] 
       "Banner generated and added! The image shows an albatross gliding close to ocean water, viewed from the side."

User: "Set the title to 'Welcome to Our World'"
Zoota: [Updates hero block title] "Title updated to 'Welcome to Our World'."
```

### Workflow 2: Create a Simple About Page

```
User: "Create an About Us page"
Zoota: [Creates page] "I've created the About Us page. Would you like me to add content?"

User: "Yes, add a hero banner"
Zoota: [Applies hero template] "Hero banner added. What should the title be?"

User: "Make it say 'About Our Company'"
Zoota: [Updates hero block] "Done! The hero banner now says 'About Our Company'."
```

### Workflow 2: Build a Services Page with Multiple Sections

```
User: "Create a services page with three sections"
Zoota: [Creates page and adds 3 sections] "Services page created with 3 sections."

User: "Add a text block to each section describing web design, SEO, and marketing"
Zoota: [Adds text blocks with generated content] "Added descriptions for all three services."

User: "Add images to each section"
Zoota: [Adds image blocks] "Image blocks added. Please provide image URLs or upload images."
```

### Workflow 3: Quick Portfolio Page

```
User: "Make a portfolio page with a gallery layout"
Zoota: [Creates page with gallery template] "Portfolio page created with gallery layout."

User: "Add 6 image placeholders"
Zoota: [Adds 6 image blocks] "6 images added to your gallery."
```

## API Reference

### Get Page Layout

```http
GET /api/zoota/cms/pages/{pageId}/layout
```

Returns the complete layout structure including sections and blocks.

### Apply Template

```http
POST /api/zoota/cms/pages/{pageId}/layout/template
Content-Type: application/json

{
  "templateName": "hero"
}
```

### Add Section

```http
POST /api/zoota/cms/pages/{pageId}/layout/sections
Content-Type: application/json

{
  "sectionType": "two-column"
}
```

### Add Block

```http
POST /api/zoota/cms/pages/{pageId}/layout/sections/{sectionId}/blocks
Content-Type: application/json

{
  "blockType": "text",
  "columnIndex": 0,
  "content": "{\"html\": \"<p>Your text here</p>\"}"
}
```

### Update Block

```http
PUT /api/zoota/cms/pages/{pageId}/layout/blocks/{blockId}
Content-Type: application/json

{
  "content": "{\"html\": \"<p>Updated text</p>\"}"
}
```

### Delete Block

```http
DELETE /api/zoota/cms/pages/{pageId}/layout/blocks/{blockId}
```

### Delete Section

```http
DELETE /api/zoota/cms/pages/{pageId}/layout/sections/{sectionId}
```

### Generate AI Banner 🆕

```http
POST /api/zoota/cms/pages/{pageId}/generate-banner
Content-Type: application/json

{
  "prompt": "An ocean with an albatross gliding close to water, side view, half sea half albatross",
  "title": "Welcome",
  "subtitle": "Your journey begins here",
  "size": "1792x1024"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Banner generated and added to page 'Landing Page'",
  "data": {
    "mediaFileId": 42,
    "imageUrl": "/uploads/banner_landing_page_20260105233000.png",
    "fileName": "banner_landing_page_20260105233000.png",
    "prompt": "An ocean with an albatross gliding..."
  }
}
```

**How it works:**
1. Generates image using Azure OpenAI DALL-E 3
2. Downloads and saves image to media library
3. Creates or updates hero section at top of page
4. Sets hero block with generated image as background

**Supported Sizes:**
- `1024x1024` - Square (1:1)
- `1792x1024` - Landscape (16:9) - Default
- `1024x1792` - Portrait (9:16)

## Content Generation

Zoota can generate content for blocks based on context:

### AI Image Generation 🆕
- "Generate a banner showing a sunset over mountains"
- "Create a hero image with a futuristic city"
- "Make a background image of a serene forest"
- "Generate an ocean scene with wildlife"

**Powered by DALL-E 3:**
- High-quality AI-generated images
- Automatic saving to media library
- Direct integration with hero/banner blocks
- Supports custom prompts and descriptions

### Text Blocks
- "Write a paragraph about our company history"
- "Generate a mission statement"
- "Create a welcome message"

### Hero Banners
- "Make a hero banner with title 'Welcome' and subtitle 'Your Success Partner'"
- "Create an engaging hero for our homepage"

### Cards
- "Create 3 service cards for web design, SEO, and marketing"
- "Generate team member cards with placeholder content"

## Integration with Existing Zoota Features

The Page Designer integration works seamlessly with Zoota's existing capabilities:

1. **Natural Language Understanding**: Zoota interprets user intent from conversational commands
2. **Context Awareness**: Zoota remembers the current page being edited
3. **Error Handling**: Zoota provides helpful feedback if operations fail
4. **Step-by-Step Guidance**: Zoota can guide users through complex page creation

## Best Practices

1. **Start with Templates**: Begin with a template, then customize
2. **Work Section by Section**: Build pages incrementally
3. **Preview Often**: Ask Zoota to preview changes
4. **Use Natural Language**: Commands don't need to be technical
5. **Iterate**: Make changes and refinements conversationally

## Example Commands Reference

### Page Operations
- "Create a page called [name]"
- "Delete the [page name] page"
- "Publish the [page name]"
- "Show me all pages"

### Layout Operations
- "Apply [template] layout"
- "Add a [section-type] section"
- "Remove the last section"
- "Change section to [section-type]"

### Block Operations
- "Add a [block-type] block"
- "Insert [block-type] in column [number]"
- "Delete the [block-type] block"
- "Update the [block-type] content"

### Content Operations
- "Write content for [block]"
- "Generate [content-type]"
- "Set the title to [text]"
- "Change the image URL to [url]"

## Technical Details

### Authentication
All Zoota Page Designer APIs require Administrator role authentication.

### Block Content Format
Block content is stored as JSON strings:

**Text Block:**
```json
{
  "html": "<p>Your HTML content</p>"
}
```

**Image Block:**
```json
{
  "src": "/images/photo.jpg",
  "alt": "Photo description"
}
```

**Hero Block:**
```json
{
  "title": "Hero Title",
  "subtitle": "Subtitle text",
  "imageUrl": "/images/hero-bg.jpg"
}
```

### Response Format
All API responses follow this structure:

**Success:**
```json
{
  "success": true,
  "message": "Operation completed",
  "data": { ... }
}
```

**Error:**
```json
{
  "success": false,
  "message": "Error description"
}
```

## Future Enhancements

Planned features for enhanced Zoota page design capabilities:

- **AI Content Generation**: GPT-powered content writing
- **Image Generation**: AI-generated images for blocks
- **SEO Optimization**: Automatic meta tags and descriptions
- **Accessibility Checks**: Ensure pages meet accessibility standards
- **Design Suggestions**: AI recommendations for layouts and styles
- **Bulk Operations**: Edit multiple blocks at once
- **Page Templates Library**: Save and reuse custom templates
- **Drag-and-Drop Preview**: Visual preview of pages in chat
- **Version Control**: Undo/redo page changes
- **Multi-language Support**: Create pages in multiple languages

## Troubleshooting

### Common Issues

**"Page not found"**
- Ensure the page exists before applying layouts
- Use the correct page ID

**"Section not found"**
- Verify the section ID is correct
- Check if the section was deleted

**"Block not found"**
- Confirm the block ID exists
- Ensure the block wasn't already deleted

**"Template failed to apply"**
- Check template name spelling
- Verify page has no conflicting layouts

### Getting Help

If Zoota encounters issues:
1. Zoota will explain the error
2. Suggest corrective actions
3. Offer to retry the operation
4. Provide links to documentation

## See Also

- [Page Designer Documentation](PAGE_DESIGNER.md)
- [Zoota User Guide](ZOOTA_USER_GUIDE.md)
- [API Documentation](API.md)
- [CMS Architecture](ARCHITECTURE.md)
