# Zoota AI Assistant - User Guide

## Overview

Zoota is an AI-powered chat assistant integrated into OrkinosaiCMS to help administrators manage their content management system through natural conversation. Zoota appears in the bottom-right corner of the admin panel and provides intelligent assistance for common CMS tasks.

![Zoota AI Assistant](../src/OrkinosaiCMS.Web/wwwroot/assets/zoota-logo/zoota-logo-concept1.svg)

## Features

### ✨ Core Capabilities

1. **Page Management**
   - Create new pages with custom titles and paths
   - Update existing page content and metadata
   - Delete pages when no longer needed
   - List and search through all pages

2. **Visual Page Designer** 🎨 NEW!
   - Design pages conversationally with layouts and blocks
   - Apply templates (hero, gallery, text+image, cards)
   - Add content blocks (text, images, videos, HTML)
   - Generate content for blocks
   - Manage sections and columns
   - See [Zoota Page Designer Guide](ZOOTA_PAGE_DESIGNER.md)

3. **Content Management**
   - Create and organize content items
   - Update content body and metadata
   - Delete outdated content
   - Browse content library

4. **Navigation & Search**
   - Search across CMS content
   - Navigate to specific admin sections
   - Find pages, content, and media quickly

5. **AI-Powered Suggestions**
   - Get recommendations for content structure
   - Receive best practices for CMS usage
   - Troubleshoot common issues
   - Learn about CMS features

### 🌍 Language Support

Zoota automatically detects whether you're communicating in:
- **English** (default)
- **Turkish** (Türkçe)

Simply type in your preferred language, and Zoota will respond accordingly.

## Getting Started

### Accessing Zoota

1. **Login to Admin Panel**
   - Navigate to `/admin` in your browser
   - Sign in with your administrator credentials

2. **Locate Zoota**
   - Look for the blue circular button in the bottom-right corner
   - The button displays the Zoota logo with a pulsing animation

3. **Open Chat Panel**
   - Click the Zoota button to open the chat interface
   - The chat panel will slide up from the bottom-right

### Using the Chat Interface

#### Welcome Screen
When you first open Zoota, you'll see:
- A friendly greeting message
- Suggested quick actions you can try
- Information about Zoota's capabilities

#### Quick Actions
Click any suggestion button to:
- "Tell me about OrkinosAI" - Learn about the company
- "What services do you offer?" - Explore available services
- "How can AI help my business?" - Discover AI benefits

#### Typing Messages
1. **Enter Your Message**
   - Type in the text input at the bottom of the chat
   - Press `Enter` to send
   - Press `Shift + Enter` for a new line

2. **Wait for Response**
   - Zoota will show a "typing" indicator
   - Responses typically arrive within 2-5 seconds
   - For Azure OpenAI responses, it may take longer

#### Closing the Chat
- Click the ✕ button in the top-right of the chat panel
- Or click the Zoota button again to toggle the chat

## Example Conversations

### Creating a Page

**You:** "Create a new page called 'About Us'"

**Zoota:** "I can help you create a new 'About Us' page! To do this through the admin panel:
1. Navigate to the Pages section in the left sidebar
2. Click the 'Create New Page' button
3. Enter 'About Us' as the title
4. The system will auto-generate a URL slug
5. Add your content and publish when ready

Would you like me to explain more about page settings?"

### Finding Content

**You:** "Show me all published pages"

**Zoota:** "You can view all published pages by:
1. Going to the Pages section in the admin menu
2. Using the filter to show only published pages
3. Or you can access the page list at `/admin/pages`

I can also help you understand page management features if you need."

### Getting Help

**You:** "How do I update the site theme?"

**Zoota:** "To update your site theme:
1. Click on 'Themes' in the admin sidebar
2. Browse the available themes
3. Click 'Activate' on your preferred theme
4. The change will apply immediately to the public site

You can also customize theme colors and settings through the theme editor."

## Configuration

### Backend Configuration

Zoota uses two modes of operation:

#### 1. Azure OpenAI Mode (Production)
Configure in `appsettings.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "gpt-4o",
    "ApiVersion": "2024-08-01-preview",
    "MaxTokens": 800,
    "Temperature": 0.7
  }
}
```

**Features:**
- Advanced natural language understanding
- Context-aware responses
- Personalized suggestions
- Learns from conversation history

#### 2. Mock Mode (Development)
When Azure OpenAI is not configured, Zoota uses intelligent mock responses:

**Features:**
- Pre-programmed responses based on keywords
- Multi-language support (English/Turkish)
- No external dependencies
- Instant responses

### Zoota Settings

Customize Zoota behavior in `appsettings.json`:

```json
{
  "Zoota": {
    "Name": "Zoota AI Assistant",
    "Version": "2.0.0",
    "WelcomeMessage": "Hi! I'm Zoota 👋 Your AI assistant...",
    "SystemPrompt": "You are Zoota, an AI-powered...",
    "KnowledgeBase": {
      "CompanyName": "OrkinosaiCMS",
      "Website": "https://github.com/orkinosai25-org/orkinosaiCMS",
      "Services": [
        "Modular CMS Architecture",
        "Blazor Server Components",
        ".NET 10 Framework"
      ]
    }
  }
}
```

## Technical Architecture

### Components

1. **Frontend: ChatAgent.razor**
   - Blazor Server component
   - Real-time interactive chat UI
   - Azure/Fluent design system styling
   - Mobile-responsive layout

2. **Backend: Python Flask Service**
   - Location: `src/OrkinosaiCMS.Web/PythonBackend/`
   - Runs on port 8000
   - Azure OpenAI integration
   - Mock response fallback
   - Multi-language support

3. **API: ZootaCmsController.cs**
   - RESTful endpoints for CMS operations
   - Admin-only authorization
   - CRUD operations for pages, content, users

4. **JavaScript: zoota-chat-agent.js**
   - Textarea auto-resize
   - Keyboard shortcuts
   - Auto-scroll functionality
   - DOM interaction helpers

### Communication Flow

```
User Message
    ↓
ChatAgent.razor (Blazor)
    ↓
HTTP POST to localhost:8000/api/chat
    ↓
Python Backend (app.py)
    ↓
[Azure OpenAI] OR [Mock Responses]
    ↓
Response JSON
    ↓
ChatAgent.razor (Display)
```

## Deployment

### Local Development

1. **Start Python Backend**
   ```bash
   cd src/OrkinosaiCMS.Web/PythonBackend
   pip install -r requirements.txt
   python app.py
   ```

2. **Start .NET Application**
   ```bash
   cd src/OrkinosaiCMS.Web
   dotnet run
   ```

3. **Access Admin Panel**
   - Navigate to `https://localhost:5001/admin`
   - Zoota will be visible in bottom-right corner

### Azure App Service

The application includes a `startup.sh` script that:
1. Installs Python dependencies
2. Starts Python backend on port 8000 (daemon mode)
3. Starts .NET application on default port

Both services run on the same Azure App Service instance.

### Health Checks

Test if Zoota backend is running:

```bash
curl http://localhost:8000/health
```

Expected response:
```json
{
  "status": "healthy",
  "service": "Zoota AI Backend",
  "version": "2.0.0",
  "azure_configured": true,
  "config_loaded": true
}
```

## Troubleshooting

### Zoota Button Not Visible

**Check:**
1. Are you on an admin page? Zoota only appears in admin panel
2. Is AdminLayout.razor loading correctly?
3. Check browser console for JavaScript errors

**Fix:**
- Clear browser cache
- Ensure `zoota-chat-agent.js` is loaded
- Verify CSS files are not blocking the button

### Chat Not Responding

**Check:**
1. Is Python backend running?
   ```bash
   curl http://localhost:8000/health
   ```

2. Check Python logs:
   - Development: Console output
   - Azure: `/home/LogFiles/python_error.log`

**Fix:**
- Restart Python backend: `python app.py`
- Check `appsettings.json` configuration
- Verify port 8000 is not blocked

### Azure OpenAI Errors

**Check:**
1. Is your API key valid?
2. Is the endpoint URL correct?
3. Do you have quota remaining?

**Fix:**
- Verify credentials in `appsettings.json`
- Check Azure OpenAI service status
- Review Python backend logs for detailed errors
- Zoota will automatically fall back to mock responses

### Language Detection Issues

**Problem:** Zoota responds in wrong language

**Fix:**
- Use clear, unambiguous language
- Include Turkish-specific characters (ç, ğ, ı, ö, ş, ü) for Turkish
- Add language-specific words to ensure proper detection

## Security

### Admin-Only Access

- Zoota is **only accessible** to users with Administrator role
- API endpoints require `[Authorize(Roles = "Administrator")]`
- Unauthorized users cannot see or interact with Zoota

### Data Privacy

- Conversations are **not stored persistently**
- Message history is maintained only during chat session
- No personal data is sent to Azure OpenAI (only chat messages)

### API Security

- All CMS operations require authentication
- CORS configured for same-origin requests
- API keys stored securely in `appsettings.json`

## Customization

### Changing Appearance

Edit `ChatAgent.razor.css` to customize:
- Colors and gradients
- Button size and position
- Chat panel dimensions
- Animation effects

### Adding Custom Responses

Edit `PythonBackend/app.py` → `get_mock_response()`:

```python
def get_mock_response(user_message):
    message_lower = user_message.lower()
    
    if 'custom_keyword' in message_lower:
        return "Your custom response here"
    
    # ... rest of function
```

### Extending API Endpoints

Add new endpoints to `ZootaCmsController.cs`:

```csharp
[HttpGet("custom-endpoint")]
public async Task<IActionResult> CustomAction()
{
    // Your logic here
    return Ok(new { success = true, data = result });
}
```

## Support & Resources

- **Documentation**: `/docs/`
- **GitHub Issues**: [Report bugs or request features](https://github.com/orkinosai25-org/orkinosaiCMS/issues)
- **Python Backend README**: `src/OrkinosaiCMS.Web/PythonBackend/README.md`
- **API Documentation**: Coming soon

## Version History

### Version 2.0.0 (Current)
- Multi-language support (English/Turkish)
- Azure OpenAI integration
- Mock response fallback
- Enhanced UI with animations
- Mobile-responsive design
- Keyboard shortcuts
- Auto-scroll functionality

### Version 1.0.0
- Initial release
- Basic chat interface
- Simple Q&A functionality

---

**Built with ❤️ for OrkinosaiCMS**

*Zoota - Your friendly AI assistant, always ready to help!* 🦜
