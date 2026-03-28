# Architecture

## Context

Family Hardware Store is intended for a small local business that needs dependable day-to-day store operations on a single Windows PC or a similarly constrained environment. The system must prioritize:

- local-first operation
- low deployment complexity
- predictable reporting and printing
- maintainable code for gradual expansion

The expected operational environment is not a cloud-native, highly distributed setup. It is a practical business application used at a counter, in stock management, and in routine reporting.

## Architectural Decision

The target architecture is a layered modular monolith built with:

- ASP.NET Core
- Razor Pages
- Entity Framework Core
- SQLite

This approach is preferred over microservices because it matches the business context:

- one primary local machine
- limited or unreliable internet access
- low infrastructure budget
- a stronger need for simplicity than horizontal scale

## Current Repository State

The current repository is an early-stage ASP.NET Core web app that already contains:

- one web project
- MVC controllers and views
- EF Core and SQLite package references
- initial domain and service classes for products, sales, and reports

This is a valid starting point, but it does not yet reflect the target layered structure. The next implementation phases should move the codebase toward the modular-monolith layout described below.

## Target Solution Structure

```text
hardware-store-local/
├─ src/
│  ├─ HardwareStore.Web/
│  ├─ HardwareStore.Application/
│  ├─ HardwareStore.Domain/
│  ├─ HardwareStore.Infrastructure/
│  └─ HardwareStore.Tests/
├─ docs/
│  ├─ architecture.md
│  ├─ database.md
│  ├─ printing.md
│  └─ roadmap.md
├─ scripts/
│  ├─ setup-dev.ps1
│  ├─ publish-local.ps1
│  └─ backup-db.ps1
├─ .gitignore
├─ README.md
└─ hardware-store-local.sln
```

## Layer Responsibilities

### Web

Responsibilities:

- Razor Pages UI
- authentication and authorization setup
- page models and UI composition
- dependency injection wiring
- user-facing validation and feedback

Rules:

- keep page models thin
- do not place core business rules in UI code
- delegate workflows to application services

### Application

Responsibilities:

- use-case orchestration
- business workflows
- DTOs and query models
- validation
- transaction boundaries for sales and purchases
- report generation coordination

Examples:

- `AuthService`
- `ProductService`
- `SupplierService`
- `PurchaseService`
- `SalesService`
- `InventoryService`
- `ReportingService`
- `SettingsService`
- `BackupService`
- `AuditService`

### Domain

Responsibilities:

- entities
- enums
- value-oriented business concepts
- invariants that belong to the business model itself

Examples:

- `Product`
- `Category`
- `Supplier`
- `Purchase`
- `PurchaseItem`
- `Sale`
- `SaleItem`
- `InventoryMovement`
- `StockAdjustment`
- `AuditLog`
- `AppSetting`
- `User`
- `Role`

Rules:

- avoid infrastructure concerns
- avoid UI concerns
- keep EF-specific attributes to a minimum

### Infrastructure

Responsibilities:

- EF Core `DbContext`
- Fluent API entity configuration
- SQLite persistence
- migrations
- seeding
- backup and restore implementation
- printing adapters
- file storage concerns

Rules:

- repositories should only be added where they provide real value
- otherwise prefer direct `DbContext` usage in application services
- external concerns should be hidden behind interfaces where it improves testability or future replacement

### Tests

Responsibilities:

- unit tests for business rules
- integration tests for SQLite-backed data access
- authorization boundary tests
- reporting filter tests
- receipt-number uniqueness tests

## Module Boundaries

The monolith should be structured internally around these business modules:

- Auth / Users
- Products
- Categories
- Suppliers
- Purchases
- Sales / POS
- Inventory
- Reports
- Printing
- Settings
- Backup / Restore
- Audit Log

Each module should keep its own:

- service layer behaviors
- page groupings
- query and command DTOs
- reporting logic where relevant

Even inside one deployable application, module boundaries reduce coupling and make later extraction or restructuring easier.

## Data Architecture

SQLite is the initial database choice because it is the most practical fit for a single-PC local installation.

Reasons:

- simple deployment
- low operational overhead
- easy backup as a file
- fully supported by EF Core
- appropriate performance profile for a small business application

Recommended local storage location:

- `App_Data/`
- or a `data/` directory under the deployed application root

Important database design rules:

- unique receipt numbers
- unique SKU values
- indexes on product name, SKU, barcode, sale date, purchase date, and inventory movement product/date
- soft-delete or status-based handling for sales and purchases instead of hard deletes
- transactional consistency for sale and purchase workflows

## Transactional Workflows

Two workflows must remain transactional:

### Sale creation

The following must succeed or fail together:

- sale header
- sale items
- payment records if applicable
- inventory movements
- audit log entry where required

### Purchase creation

The following must succeed or fail together:

- purchase header
- purchase items
- inventory movements
- audit log entry where required

This should be enforced in the application layer using EF Core transaction support.

## Authentication and Authorization

The system should use local authentication with role-based authorization.

Initial roles:

- `Admin`
- `Cashier`

Authorization intent:

- Admin manages products, settings, price-sensitive operations, voids, restore, and profit reporting
- Cashier performs sales, prints receipts, and views limited reports

Critical audit events:

- login attempts
- price changes
- stock adjustments
- voided sales
- settings changes
- backup and restore actions

## Reporting Architecture

Reports should be implemented as application-layer queries with clear filter objects, then rendered in Razor Pages as print-friendly HTML tables.

Required report families:

- daily sales
- monthly sales
- sales by product
- stock on hand
- low stock
- purchase history
- profit summary
- cashier activity
- voided sales

Common filters:

- from date
- to date
- category
- supplier
- product
- cashier
- payment method
- include voided

Output strategy:

- HTML first
- browser print support
- CSV export
- code structure that leaves room for later PDF export

## Printing Strategy

The first implementation should use browser printing rather than direct device integration.

Phase 1:

- thermal receipt HTML template
- A4 report print templates
- reprint by receipt number
- settings-driven shop identity fields

Future phase:

- direct printer integration hidden behind a printing interface

This keeps the first version reliable and easier to deploy.

## Backup and Restore

Because the system is local-first, backup and restore are operational requirements, not optional extras.

Design expectations:

- manual backup button
- automatic daily backup
- timestamped database copies
- local `backups/` folder
- optional ZIP packaging
- clear restore warnings and confirmation steps
- audit log entries for backup and restore events

## Deployment Model

Primary deployment target:

- one local Windows PC in the shop

Expected characteristics:

- no dependency on constant internet access
- lightweight runtime footprint
- simple local database file handling
- straightforward publishing through scripted deployment

Recommended supporting scripts:

- development setup
- local publish
- database backup

## Migration Path

The architecture should preserve these future options:

### Database migration

- SQLite to SQL Server
- SQLite to PostgreSQL

### Hosting migration

- local desktop-style hosting to shop LAN hosting
- one-PC deployment to small multi-device usage

### Capability migration

- browser print only to direct printer support
- manual operations to more automated operational workflows

The layered structure and SQLite-first persistence strategy should make these migrations evolutionary rather than disruptive.

## Non-Goals

At this stage, the architecture does not optimize for:

- internet-scale traffic
- multi-tenant SaaS deployment
- microservice distribution
- high-complexity event-driven infrastructure

Those choices would add cost and operational burden without matching the current business need.

## Implementation Guidance

Engineering standards for this repository:

- use small focused classes
- use async EF Core operations where appropriate
- keep business logic out of page models
- prefer Fluent API over scattered data annotations
- avoid generic abstractions that do not solve a real problem
- write code for a real store workflow, not a demo

## Immediate Next Steps

1. restructure the solution into Web, Application, Domain, Infrastructure, and Tests projects
2. move the UI direction toward Razor Pages
3. formalize the domain model for products, suppliers, purchases, sales, inventory, settings, and audit logging
4. add `AppDbContext`, Fluent API configuration, SQLite migration support, and seeding
5. implement transactional application services for sales and purchases
6. add reporting, receipt printing, authentication, backup, and test coverage
