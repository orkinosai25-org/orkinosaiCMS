# OrkinosaiCMS

A modern, modular Content Management System built on .NET 10 and Blazor, inspired by Oqtane CMS and SharePoint.

## 🚀 Features

- **Modular Architecture**: Plugin-based system for unlimited extensibility
- **SharePoint-Inspired**: Familiar concepts like Master Pages, Web Parts (Modules), and permission levels
- **Modern Stack**: Built on .NET 10, Blazor, and Entity Framework Core
- **Clean Architecture**: Clear separation between Core, Infrastructure, and UI layers
- **Flexible Permissions**: Fine-grained, SharePoint-style permission system
- **Master Pages**: Reusable layouts with content zones
- **Theme Support**: Customizable visual themes
- **Zoota AI Assistant**: 🤖 AI-powered chat assistant for admin tasks (see [User Guide](docs/ZOOTA_USER_GUIDE.md))
- **SaaS-Ready**: Architecture designed for multi-tenancy (coming soon)

## 📋 Requirements

- .NET 10 SDK
- SQL Server 2019+ / LocalDB (Windows) / Azure SQL (for development and production)
- Visual Studio 2022 (17.12+) or Visual Studio 2026 (recommended)

## 🎯 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/orkinosai25-org/orkinosaiCMS.git
cd orkinosaiCMS
```

### 2. Restore Dependencies

```bash
dotnet restore OrkinosaiCMS.sln
```

### 3. Apply Database Migrations

Database is pre-configured with LocalDB for Windows. Apply the initial migration:

```bash
# Install EF Core tools (first time only)
dotnet tool install --global dotnet-ef --version 10.0.0

# Apply migrations
cd src/OrkinosaiCMS.Infrastructure
dotnet ef database update --startup-project ../OrkinosaiCMS.Web
```

For non-Windows or production setup, see [Database Guide](docs/DATABASE.md) and [Setup Guide](docs/SETUP.md).

### 5. Run the Application

```bash
cd ../OrkinosaiCMS.Web
dotnet run
```

Navigate to `https://localhost:5001`

**Note**: The database will be automatically seeded with sample data including:
- Professional theme
- Two master page layouts (Standard and Full Width)
- Four modules (Hero, Features, ContactForm, HtmlContent)
- Three sample pages (Home, About, Contact)
- Default roles and permissions
- **Default admin user** with credentials: `admin` / `Admin@123`

### 6. Access the Admin Panel

Navigate to `https://localhost:5001/oqtane-login` and sign in with:
- **Username:** `admin` (or any username containing "admin")
- **Password:** `oqtane123`

You will be immediately redirected to the admin panel at `/admin` with full access to CMS management features.

> **Note**: The system now uses Oqtane-based authentication exclusively. Visiting `/admin/login` or `/login` will automatically redirect to `/oqtane-login`. After successful login, admin users are taken directly to the admin panel with no intermediate pages.

Visit `/cms-home` to see the demo website!

## 📚 Documentation

### Core Documentation
- **[Architecture Guide](docs/ARCHITECTURE.md)** - Understand the system design and architecture
- **[Setup Guide](docs/SETUP.md)** - Detailed setup and configuration instructions
- **[Database Guide](docs/DATABASE.md)** - Database architecture and data access patterns
- **[Extensibility Guide](docs/EXTENSIBILITY.md)** - Creating custom modules, themes, and extensions
- **[Logging Guide](docs/LOGGING.md)** - Serilog logging configuration, troubleshooting, and best practices

### SaaS & Multi-Tenancy
- **[SaaS Compatibility Guide](docs/SAAS_COMPATIBILITY.md)** - Transform OrkinosaiCMS into a multi-tenant SaaS platform
- **[SaaS Features Overview](docs/SaaS_FEATURES.md)** - Detailed feature comparison and roadmap
- **[Onboarding Guide](docs/ONBOARDING.md)** - Complete user journey from sign-up to launch
- **[Pricing Plans](docs/pricing.md)** - Complete pricing guide with plan comparisons
- **[Site Management API](docs/SITE_MANAGEMENT_API.md)** - API documentation for creating and managing sites

### Payment & Storage Integration
- **[Stripe Setup Guide](docs/STRIPE_SETUP.md)** - Complete guide for configuring Stripe payment integration
- **[Stripe Quick Start](docs/STRIPE_QUICK_START.md)** - Quick reference for Stripe integration
- **[Azure Blob Storage](docs/AZURE_BLOB_STORAGE.md)** - Media storage, security, and usage guide
- **[Application Settings](docs/appsettings.md)** - Configuration guide for appsettings.json and Azure Key Vault

### Migration & Deployment
- **[Migration Guide](docs/MIGRATION.md)** - Migrating from Oqtane v10
- **[Content Migration Design](docs/CONTENT_MIGRATION_DESIGN.md)** - Design decisions and migration strategy
- **[Azure Deployment](docs/AZURE_DEPLOYMENT.md)** - Deploy to Azure Web Apps with Azure SQL
- **[Deployment Checklist](docs/DEPLOYMENT_CHECKLIST.md)** - Complete deployment procedures

### GitHub Copilot & Troubleshooting
- **[Quick Fix Guide](docs/QUICK_FIX_GUIDE.md)** ⚡ - Immediate solutions for common Copilot agent issues
- **[Copilot Agent Guide](docs/github-copilot-agent-guide.md)** - Complete guide for working with Copilot agents
- **[Troubleshooting Guide](docs/copilot-agent-troubleshooting.md)** - Detailed analysis of agent workflow failures
- **[Utility Scripts](scripts/)** - Helper scripts for common tasks (e.g., `fix-base-branch.sh`)

### AI & Automation
- **[Zoota AI Assistant User Guide](docs/ZOOTA_USER_GUIDE.md)** - Complete guide for using the AI chat assistant
- **[Zoota Testing Configuration](docs/ZOOTA_TESTING_CONFIG.md)** - Configure and use the Zoota Test Page for automated testing
- **[AI Assistant Roadmap](docs/AI_ASSISTANT_ROADMAP.md)** - 12-month AI integration plan

### Roadmap & Planning
- **[Implementation Summary](docs/WEBSITE_IMPLEMENTATION_SUMMARY.md)** - Project overview and status

## 🏗️ Project Structure

```
orkinosaiCMS/
├── src/
│   ├── OrkinosaiCMS.Core/              # Domain entities and interfaces
│   ├── OrkinosaiCMS.Infrastructure/    # Data access and services
│   ├── OrkinosaiCMS.Modules.Abstractions/  # Module base classes
│   ├── OrkinosaiCMS.Shared/            # Shared DTOs
│   ├── OrkinosaiCMS.Web/               # Blazor Web App
│   └── Modules/
│       └── OrkinosaiCMS.Modules.Content/  # Sample content module
├── docs/                                # Documentation
└── tests/                               # Unit and integration tests
```

## 🎨 Creating a Module

Creating a module is simple:

```csharp
[Module("MyModule", "My Custom Module", Category = "Custom")]
public class MyModule : ModuleBase
{
    public override string ModuleName => "MyModule";
    public override string Title => "My Custom Module";
    public override string Description => "A custom module example";
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // Your initialization logic
    }
}
```

See the [Extensibility Guide](docs/EXTENSIBILITY.md) for more details.

## 🔐 Permission System

OrkinosaiCMS implements a SharePoint-inspired permission system:

- **Fine-grained Permissions**: View, Edit, Delete, Manage, etc.
- **Roles**: Administrator, Designer, Editor, Contributor, Reader
- **Permission Levels**: Similar to SharePoint permission levels
- **Page-Level Security**: Control access to individual pages

## 🎯 Architecture Highlights

### Clean Architecture Layers

1. **Core**: Domain entities and business logic interfaces
2. **Infrastructure**: Data access, external services, implementations
3. **Modules**: Pluggable components with attribute-based discovery
4. **Web**: Blazor UI and presentation layer

### Key Design Patterns

- Repository Pattern for data access
- Dependency Injection throughout
- Soft Delete for data preservation
- Automatic audit fields (CreatedOn, ModifiedOn, etc.)
- Master Page pattern for layouts

## 🌟 Comparison with Oqtane

| Feature | OrkinosaiCMS | Oqtane |
|---------|-------------|--------|
| Architecture | Clean Architecture | Modular Monolith |
| Page Model | SharePoint-inspired Master Pages | Templates |
| Modules | Attribute-based discovery | Interface-based |
| Permissions | Fine-grained SharePoint-style | Role-based |
| Target Version | .NET 10 | .NET 8/9 |

See [Migration Guide](docs/MIGRATION.md) for detailed comparison.

## 🔧 SaaS Compatibility & Configuration

OrkinosaiCMS is designed to be the master CMS repository with full support for both single-tenant and multi-tenant SaaS deployments.

### Environment Configuration

A comprehensive `.env.example` file is provided with all necessary configuration options:

- **Payment Integration**: Stripe API keys for subscription management
- **Storage**: Azure Blob Storage connection strings for media/assets
- **Database**: Connection string examples for LocalDB, SQL Server, and Azure SQL
- **Authentication**: JWT secrets and OAuth provider configuration
- **Email**: SendGrid API keys for transactional emails
- **AI Services**: Azure OpenAI endpoints for AI-powered features

Copy `.env.example` to `.env` and configure with your actual values:

```bash
cp .env.example .env
# Edit .env with your configuration values
```

**Important**: Never commit the `.env` file to version control. It's already included in `.gitignore`.

### Multi-Tenant SaaS Setup

To transform OrkinosaiCMS into a multi-tenant SaaS platform, see the comprehensive [SaaS Compatibility Guide](docs/SAAS_COMPATIBILITY.md) which covers:

1. **Database & Multi-Tenancy**: Tenant identification strategies and data isolation
2. **Authentication & Authorization**: OAuth providers and tenant-scoped users
3. **Branding & Theming**: Professional themes and tenant-specific customization
4. **Configuration Management**: Azure Key Vault integration and feature flags
5. **Payment Integration**: Stripe subscription management
6. **User Onboarding**: Streamlined sign-up and guided setup wizard
7. **API & Webhooks**: Public REST API and event notifications
8. **Analytics & Monitoring**: Application Insights and usage tracking
9. **Domain Management**: Custom domains and SSL certificate provisioning
10. **AI Agent Enhancement**: Customer-facing and admin AI assistants

For detailed configuration options, see [Application Settings Guide](docs/appsettings.md).

## 🛣️ Roadmap

### Current Release (v1.0)
- ✅ Core CMS framework
- ✅ Module system with discovery
- ✅ SharePoint-inspired page model
- ✅ Permission system
- ✅ Theme support
- ✅ Master Pages

### Future Releases
- 🔄 Multi-tenancy support
- 🔄 Content workflow engine
- 🔄 Page versioning
- 🔄 Full-text search
- 🔄 Localization
- 🔄 API layer for headless CMS
- 🔄 Real-time collaborative editing

## 🤝 Contributing

Contributions are welcome! Please read our contributing guidelines (coming soon) before submitting pull requests.

### Development Setup

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Oqtane CMS** - Inspiration for the modular architecture
- **SharePoint** - Inspiration for page model and permission system
- **.NET Team** - For the amazing .NET 10 and Blazor frameworks

## 🔧 Troubleshooting

### Common Issues

#### "Base branch not found" error
If you encounter `fatal: ambiguous argument 'refs/heads/main'`, this typically happens with shallow clones. Quick fix:
```bash
./scripts/fix-base-branch.sh
```
Or see our [Quick Fix Guide](docs/QUICK_FIX_GUIDE.md) for detailed solutions.

#### Copilot Agent Workflow Failures
If a GitHub Copilot agent workflow fails:
1. Check the [Quick Fix Guide](docs/QUICK_FIX_GUIDE.md) for immediate solutions
2. Review the [Copilot Agent Guide](docs/github-copilot-agent-guide.md) for best practices
3. See the [Troubleshooting Guide](docs/copilot-agent-troubleshooting.md) for detailed analysis

#### Database Connection Issues
See [Database Guide](docs/DATABASE.md) and [Setup Guide](docs/SETUP.md) for configuration help.

## 🧪 Testing

OrkinosaiCMS includes comprehensive unit and integration tests to ensure reliability of core authentication features.

### Running Tests

```bash
# Run all tests
dotnet test OrkinosaiCMS.sln

# Run tests with detailed output
dotnet test OrkinosaiCMS.sln --verbosity normal

# Run tests with code coverage
dotnet test OrkinosaiCMS.sln --collect:"XPlat Code Coverage"
```

### Test Coverage

#### Authentication Tests
- **OqtaneAuthServiceTests**: Unit tests for Oqtane demo authentication
  - ✅ Validates credentials and role assignment
  - ✅ Tests admin role detection (username contains "admin")
  - ✅ Tests user role assignment for non-admin users
  - ✅ Validates password requirements

- **AuthenticationIntegrationTests**: Integration tests for authentication flows
  - ✅ JWT token generation and validation
  - ✅ Token persistence across multiple validations (Issue #56)
  - ✅ Administrator role persistence in tokens
  - ✅ Failsafe mode JWT claims
  - ✅ Token expiration handling
  - ✅ Role detection (case-insensitive)

#### Issue #56: Login Redirect Bug
Tests specifically address the bug where successfully logged-in admins were shown the sign-in page again:
- ✅ JWT tokens remain valid across repeated state checks
- ✅ Administrator role persists through navigation
- ✅ Token validation doesn't fail on repeated checks

### CI/CD Integration

Tests are automatically run in GitHub Actions CI/CD pipeline:
- Triggered on pushes to `main`, `develop`, and `copilot/*` branches
- Triggered on pull requests to `main` and `develop`
- Results are uploaded as artifacts for review
- Code coverage reports are collected

## 📞 Support

- **Documentation**: [docs/](docs/)
- **Troubleshooting**: [Quick Fix Guide](docs/QUICK_FIX_GUIDE.md)
- **Issues**: [GitHub Issues](https://github.com/orkinosai25-org/orkinosaiCMS/issues)
- **Discussions**: [GitHub Discussions](https://github.com/orkinosai25-org/orkinosaiCMS/discussions)

## 🌐 Links

- **Website**: Coming soon
- **Documentation**: [docs/](docs/)
- **Module Gallery**: Coming soon

---

Built with ❤️ using .NET 10 and Blazor
