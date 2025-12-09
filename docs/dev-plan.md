# OrkinosaiCMS Development Plan

This document tracks our current feature roadmap, priorities, and assignments for CMS core development.

## Core Features (Active Development)

### ✅ Completed Features
- **User & Role Management**: Full CRUD operations for users with password hashing (BCrypt), role assignment, and authentication support
- **Page Management**: Complete page CRUD with publish/draft workflow, hierarchical navigation, and reordering capabilities
- **Content Management**: Document and media management with CRUD operations, categorization, tagging, and search functionality
- **Permissions Model**: Role-based permission system with user permission checks and role-permission assignment
- **Modular Architecture**: Plugin-based system with attribute discovery (already implemented)
- **Database Layer**: EF Core with SQL Azure (production) and SQLite (development/testing) support
- **Theme Engine (Blazor)**: Already implemented

### 🔄 In Progress
- Fluent UI-based Admin Panel
- Full-text Search
- Versioning & Drafts (basic draft support implemented)
- Localization & Multilingual
- REST & GraphQL APIs
- Document Management: Upload, Preview, Metadata (entity structure in place)

## Planned Advanced Features (Next Sprints)

- Multi-tenancy
- Site Templates
- Workflow Designer
- Extensible Web Parts (Blazor/React)
- External Integrations (Office 365, OneDrive, Power Automate)
- SSO (Azure AD)
- Activity Logging & Auditing
- Analytics & Reporting
- Chat Agent Integration (for future SAAS)

## Assignments
- Track assignments in issues and PRs. To propose features, create an issue and tag relevant team members.

## Recent Updates (December 2025)

### Zoota Admin-Only Integration (December 9, 2025)
- **Authentication System Implemented**: Custom authentication state provider for admin users
- **Admin Panel Created**: Dedicated admin layout with navigation, login page, and dashboard
- **Zoota Restricted**: Chat agent now only appears for authenticated administrators
- **CMS API Endpoints**: RESTful API for managing pages, content, and users via Zoota
- **Services**: AuthenticationService, CustomAuthenticationStateProvider for session management
- **Admin Routes**: `/admin/login` and `/admin` with role-based authorization

## Recent Updates (December 2025)

### Database Configuration
- **SQL Azure Support**: Production connection string configured for `orkinosai.database.windows.net`
- **SQLite Support**: Development/testing mode using local SQLite database
- **Configuration**: Use `DatabaseProvider` setting in appsettings.json to switch between providers

### Services Implemented
- **IUserService/UserService**: User CRUD, role assignment, password management, authentication
- **IPageService/PageService**: Page CRUD, publish/unpublish, hierarchical navigation, reordering
- **IContentService/ContentService**: Content CRUD for documents/media, categorization, search
- **IRoleService/RoleService**: Role CRUD, permission assignment to roles
- **IPermissionService/PermissionService**: Permission CRUD, user permission validation

### Security
- Password hashing using BCrypt.Net-Next (industry standard)
- Soft delete implemented across all entities
- Role-based access control foundation in place

---

_Last updated: 2025-12-09_

## Dual Agent Strategy

### SaaS Conversational Agent for Visitors
This agent is used for all public-facing (visitor) interaction on client websites powered by OrkinosaiCMS. It is managed as a SaaS product and is not included with the CMS codebase; this makes it ideal for monetization and feature control across all sites.

### Zoota (Admin-Only) Agent ✅ IMPLEMENTED
The Zoota chat agent is now embedded only in the CMS admin (backend) panel. It appears when admins log in and provides:

**Current Capabilities:**
- ✅ Admin-only access with authentication
- ✅ Appears in admin panel after login
- ✅ Conversational AI assistance powered by Azure OpenAI
- ✅ CMS documentation and help
- ✅ RESTful API endpoints for CMS operations

**API Endpoints Available:**
- GET/POST/PUT/DELETE `/api/zoota/cms/pages` - Page management
- GET/POST/PUT/DELETE `/api/zoota/cms/content` - Content management
- GET `/api/zoota/cms/users` - User listing

**Future Enhancements:**
- Direct conversational commands for creating/editing assets
- Workflow automation suggestions
- Advanced search and navigation
- Image and media management

The Zoota agent is not available to site visitors—admin-only functionality. Architecture and UX keep these roles clearly separated for security, usability, and development clarity.


