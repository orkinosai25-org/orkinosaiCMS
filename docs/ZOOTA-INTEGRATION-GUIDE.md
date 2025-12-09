# Zoota AI Assistant - Integration Guide for OrkinosaiCMS

## Overview

This guide explains how the Zoota AI Assistant has been integrated into OrkinosaiCMS. Zoota is a conversational AI chat agent powered by Azure OpenAI, with a Python Flask backend and Blazor frontend component.

## Architecture

```
┌─────────────────────────────────────────┐
│         OrkinosaiCMS Application        │
├─────────────────────────────────────────┤
│  ┌──────────────────┐                   │
│  │ Python Backend   │                   │
│  │ (Port 8000)      │                   │
│  │ - Flask + WSGI   │                   │
│  │ - Gunicorn       │                   │
│  │ - Azure OpenAI   │                   │
│  └────────┬─────────┘                   │
│           │                              │
│  ┌────────▼─────────┐                   │
│  │ .NET/Blazor App  │                   │
│  │ (Port 80/443)    │                   │
│  │ - ChatAgent.razor│                   │
│  │ - MainLayout     │                   │
│  └──────────────────┘                   │
└─────────────────────────────────────────┘
```

## Components Integrated

### 1. Python Backend (Flask Server)

**Location:** `src/OrkinosaiCMS.Web/PythonBackend/`

**Files:**
- `app.py` - Main Flask application with Azure OpenAI integration
- `wsgi.py` - WSGI entry point for Gunicorn (production)
- `requirements.txt` - Python dependencies
- `README.md` - Backend documentation

**Key Features:**
- ✅ Reads configuration from `appsettings.json`
- ✅ Azure OpenAI integration
- ✅ Fallback to mock responses when Azure is not configured
- ✅ RESTful API endpoints
- ✅ CORS support for frontend
- ✅ Comprehensive logging
- ✅ Health check endpoint

### 2. Blazor Chat Component

**Location:** `src/OrkinosaiCMS.Web/Components/Shared/`

**Files:**
- `ChatAgent.razor` - Main chat UI component (180 lines)
- `ChatAgent.razor.css` - Scoped CSS styling (440 lines)

**Key Features:**
- ✅ Floating chat button in bottom-right corner
- ✅ Expandable chat panel
- ✅ Welcome screen with Zoota introduction
- ✅ Message history display
- ✅ Typing indicator
- ✅ User/assistant message differentiation
- ✅ Azure/Fluent Design styling
- ✅ Responsive design (desktop & mobile)

### 3. Configuration

**Location:** `src/OrkinosaiCMS.Web/appsettings.json`

**Added Sections:**
```json
{
  "DatabaseEnabled": false,
  "AzureOpenAI": {
    "Endpoint": "https://your-resource-name.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "gpt-4o",
    "ApiVersion": "2024-08-01-preview",
    "MaxTokens": 800,
    "Temperature": 0.7
  },
  "PythonBackend": {
    "Enabled": true,
    "Port": 8000,
    "Host": "0.0.0.0",
    "Workers": 2,
    "Timeout": 120
  },
  "Zoota": {
    "Name": "Zoota AI Assistant",
    "Version": "2.0.0",
    "WelcomeMessage": "Hi! I'm Zoota...",
    "SystemPrompt": "You are Zoota...",
    "KnowledgeBase": {
      "CompanyName": "OrkinosaiCMS",
      "Services": [...],
      "Expertise": [...]
    }
  }
}
```

### 4. Startup Script

**Location:** `src/OrkinosaiCMS.Web/startup.sh`

**Purpose:** Orchestrates startup of Python backend and .NET application

**Process:**
1. Detects environment (Azure or local)
2. Validates `appsettings.json` exists
3. Installs Python dependencies
4. Starts Gunicorn server in daemon mode (background)
5. Verifies Python backend is responding
6. Starts .NET application

### 5. Layout Integration

**Modified Files:**

**`src/OrkinosaiCMS.Web/Components/Layout/MainLayout.razor`**
- Added `<ChatAgent />` component at the end

**`src/OrkinosaiCMS.Web/Components/_Imports.razor`**
- Added `@using OrkinosaiCMS.Web.Components.Shared`

## Configuration Setup

### For Local Development (Mock Responses)

The default configuration uses placeholder values. Zoota will work with mock responses:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource-name.openai.azure.com/",
    "ApiKey": "your-api-key-here"
  }
}
```

### For Production (Azure OpenAI Integration)

Update `appsettings.json` with real Azure OpenAI credentials:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://YOUR-RESOURCE.openai.azure.com/",
    "ApiKey": "YOUR-ACTUAL-API-KEY",
    "DeploymentName": "gpt-4o",
    "ApiVersion": "2024-08-01-preview"
  }
}
```

**How to get credentials:**
1. Create Azure OpenAI resource in Azure Portal
2. Go to "Keys and Endpoint"
3. Copy Endpoint and Key
4. Create a deployment (e.g., gpt-4o)
5. Use deployment name in configuration

## Running Locally

### Prerequisites

- .NET 10.0 SDK
- Python 3.8 or higher
- pip (Python package manager)

### Option 1: Manual Startup

**Terminal 1 - Python Backend:**
```bash
cd src/OrkinosaiCMS.Web/PythonBackend
pip install -r requirements.txt
python app.py
```

**Terminal 2 - .NET Application:**
```bash
cd src/OrkinosaiCMS.Web
dotnet run
```

Access at: `https://localhost:5001`

### Option 2: Using Startup Script (Azure-style)

```bash
cd src/OrkinosaiCMS.Web
bash startup.sh
```

**Note:** This starts Python backend in daemon mode then starts .NET app.

## Testing

### Test Python Backend

```bash
# Health check
curl http://localhost:8000/health

# Expected response:
# {"status":"healthy","service":"Zoota AI Backend",...}

# Test chat
curl -X POST http://localhost:8000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello, Zoota!"}'

# Expected response:
# {"message":"Hello! I'm Zoota...","source":"mock"}
```

### Test Chat UI

1. Run the application
2. Navigate to any page
3. Look for floating chat button in bottom-right corner
4. Click button to open chat
5. Send a message
6. Verify response appears

## API Endpoints

### Python Backend

**Base URL:** `http://localhost:8000`

#### 1. Health Check
```
GET /health

Response:
{
  "status": "healthy",
  "service": "Zoota AI Backend",
  "azure_configured": false,
  "config_loaded": true
}
```

#### 2. Chat
```
POST /api/chat

Request:
{
  "message": "Tell me about OrkinosaiCMS",
  "history": []
}

Response:
{
  "message": "OrkinosaiCMS is a modular Content Management System...",
  "source": "mock"  // or "azure_openai"
}
```

#### 3. Configuration
```
GET /api/config

Response:
{
  "name": "Zoota AI Assistant",
  "version": "2.0.0",
  "welcomeMessage": "Hi! I'm Zoota...",
  "azureConfigured": false
}
```

## Customization

### Update Knowledge Base

Edit `appsettings.json`:

```json
{
  "Zoota": {
    "KnowledgeBase": {
      "CompanyName": "Your Company",
      "Website": "https://your-website.com",
      "Services": [
        "Service 1",
        "Service 2"
      ],
      "Expertise": [
        "Expertise 1",
        "Expertise 2"
      ]
    }
  }
}
```

### Update Welcome Message

Edit `appsettings.json`:

```json
{
  "Zoota": {
    "WelcomeMessage": "Your custom welcome message here!"
  }
}
```

### Update System Prompt (AI Behavior)

Edit `appsettings.json`:

```json
{
  "Zoota": {
    "SystemPrompt": "You are Zoota, a friendly AI assistant for [Your Company]. Your custom instructions here..."
  }
}
```

### Customize Chat UI Colors

Edit `src/OrkinosaiCMS.Web/Components/Shared/ChatAgent.razor.css`:

```css
/* Change primary color */
.chat-header {
    background: linear-gradient(135deg, #0078D4, #00BCF2);
}

/* Change button color */
.chat-button {
    background: linear-gradient(135deg, #0078D4, #00BCF2);
}
```

## Deployment

### Azure App Service

The startup script is designed for Azure App Service deployment:

1. **Set Startup Command** in Azure Portal:
   ```
   Configuration → General Settings → Startup Command:
   bash startup.sh
   ```

2. **Configure Application Settings** (optional):
   - Or update `appsettings.json` directly in repository

3. **Deploy:**
   - GitHub Actions automatically handles deployment
   - Python backend starts on port 8000
   - .NET app starts on port 80/443

### Docker

To run in Docker, create `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Install Python
RUN apt-get update && apt-get install -y python3 python3-pip

WORKDIR /app
COPY . .

# Install Python dependencies
WORKDIR /app/PythonBackend
RUN pip3 install -r requirements.txt

WORKDIR /app

# Make startup script executable
RUN chmod +x startup.sh

EXPOSE 80
EXPOSE 8000

ENTRYPOINT ["bash", "startup.sh"]
```

## Troubleshooting

### Chat Button Not Appearing

**Check:**
1. `ChatAgent` component is added to `MainLayout.razor`
2. `@using OrkinosaiCMS.Web.Components.Shared` is in `_Imports.razor`
3. Build succeeded without errors
4. Browser console shows no errors (F12)

### Python Backend Not Starting

**Check:**
1. Python 3.8+ is installed: `python3 --version`
2. Dependencies installed: `pip install -r requirements.txt`
3. `appsettings.json` exists in correct location
4. Check logs: `tail -f /home/LogFiles/python_error.log` (Azure)

### Chat Shows "Backend Unavailable"

**Check:**
1. Python backend is running: `curl http://localhost:8000/health`
2. Port 8000 is not blocked
3. Python backend started before .NET app
4. Check Python logs for errors

### Azure OpenAI Errors

**Check:**
1. Credentials are correct in `appsettings.json`
2. Azure OpenAI resource is active
3. Deployment name matches exactly
4. API key hasn't expired
5. **Fallback:** Zoota automatically falls back to mock responses on Azure errors

## Security Notes

### API Keys

- **Never commit real API keys** to Git
- Use placeholder values in repository
- Set real values in Azure Portal Application Settings
- Or use Azure Key Vault references

### CORS

Python backend has CORS enabled for frontend integration. In production, restrict CORS origins:

Edit `app.py`:
```python
CORS(app, origins=["https://your-domain.com"])
```

## Files Modified

### New Files (10):
- `src/OrkinosaiCMS.Web/PythonBackend/app.py`
- `src/OrkinosaiCMS.Web/PythonBackend/wsgi.py`
- `src/OrkinosaiCMS.Web/PythonBackend/requirements.txt`
- `src/OrkinosaiCMS.Web/PythonBackend/README.md`
- `src/OrkinosaiCMS.Web/Components/Shared/ChatAgent.razor`
- `src/OrkinosaiCMS.Web/Components/Shared/ChatAgent.razor.css`
- `src/OrkinosaiCMS.Web/startup.sh`
- `docs/ZOOTA-IMPLEMENTATION-SUMMARY.md`
- `docs/PYTHON-BACKEND-DEPLOYMENT.md`
- `docs/ZOOTA-INTEGRATION-GUIDE.md` (this file)

### Modified Files (3):
- `src/OrkinosaiCMS.Web/appsettings.json` (added Zoota configuration)
- `src/OrkinosaiCMS.Web/Components/Layout/MainLayout.razor` (added ChatAgent component)
- `src/OrkinosaiCMS.Web/Components/_Imports.razor` (added Shared namespace)

## Support Resources

- **Backend Documentation:** `src/OrkinosaiCMS.Web/PythonBackend/README.md`
- **Implementation Summary:** `docs/ZOOTA-IMPLEMENTATION-SUMMARY.md`
- **Deployment Guide:** `docs/PYTHON-BACKEND-DEPLOYMENT.md`
- **Integration Guide:** `docs/ZOOTA-INTEGRATION-GUIDE.md` (this file)

## Summary

✅ **Zoota AI Assistant is now fully integrated into OrkinosaiCMS**

- Python Flask backend provides Azure OpenAI integration
- Blazor ChatAgent component provides chat UI
- Works out-of-the-box with mock responses
- Easy to configure with Azure OpenAI
- Fully documented and production-ready

**Next Steps:**
1. Test locally: `dotnet run`
2. Configure Azure OpenAI credentials (optional)
3. Deploy to Azure App Service
4. Enjoy your AI-powered chat assistant!

---

**Version:** 1.0.0  
**Date:** December 8, 2025  
**Source:** Copied from orkinosai25-org/orkinosai-website
