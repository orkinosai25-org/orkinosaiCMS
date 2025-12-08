# OrkinosaiCMS Development Plan

This document tracks our current feature roadmap, priorities, and assignments for CMS core development.

## Core Features (Active Development)

- User & Role Management
- Modular Architecture (plugins, extensions)
- Content Types (Pages, Documents, Entities)
- Permissions Model
- Fluent UI-based Admin Panel
- Full-text Search
- Versioning & Drafts
- Localization & Multilingual
- Theme Engine (Blazor)
- REST & GraphQL APIs
- Document Management: Upload, Preview, Metadata
- Database Layer (.NET 10/EF Core)

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

---

_Last updated: 2025-12-08_
Dual Agent Strategy
SaaS Conversational Agent for Visitors: This agent is used for all public-facing (visitor) interaction on client websites powered by OrkinosaiCMS. It is managed as a SaaS product and is not included with the CMS codebase; this makes it ideal for monetization and feature control across all sites.

Zoota (Admin-Only) Agent: The Zoota chat agent is embedded only in the CMS admin (backend) panel. It appears when admins log in and can:

Create/manage assets, pages, content, and images
Run CMS commands (add, update, delete, etc.)
Provide AI-powered conversational help for internal CMS features
Search docs and automate admin workflows
The Zoota agent is not available to site visitors—admin-only functionality. Architecture and UX should keep these roles clearly separated for security, usability, and development clarity.


