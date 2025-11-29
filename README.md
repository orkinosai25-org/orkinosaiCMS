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

## 📚 Documentation

- **[Architecture Guide](docs/ARCHITECTURE.md)** - Understand the system design and architecture
- **[Setup Guide](docs/SETUP.md)** - Detailed setup and configuration instructions
- **[Database Guide](docs/DATABASE.md)** - Database architecture and data access patterns
- **[Azure Deployment](docs/AZURE_DEPLOYMENT.md)** - Deploy to Azure Web Apps with Azure SQL
- **[Migration Guide](docs/MIGRATION.md)** - Migrating from Oqtane v10
- **[Extensibility Guide](docs/EXTENSIBILITY.md)** - Creating custom modules, themes, and extensions

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
└── tests/                               # Unit and integration tests (coming soon)
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
4. Add tests (when test infrastructure is available)
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Oqtane CMS** - Inspiration for the modular architecture
- **SharePoint** - Inspiration for page model and permission system
- **.NET Team** - For the amazing .NET 10 and Blazor frameworks

## 📞 Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/orkinosai25-org/orkinosaiCMS/issues)
- **Discussions**: [GitHub Discussions](https://github.com/orkinosai25-org/orkinosaiCMS/discussions)

## 🌐 Links

- **Website**: Coming soon
- **Documentation**: [docs/](docs/)
- **Module Gallery**: Coming soon

---

Built with ❤️ using .NET 10 and Blazor
