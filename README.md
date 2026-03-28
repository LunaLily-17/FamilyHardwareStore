# Family Hardware Store

A local-first hardware store management system for small shops, with both ASP.NET Core web and Windows desktop variants sharing the same domain and data layers.

## Overview

This repository is intended to become a production-oriented store operations system for a family-run hardware business with a single-PC or low-connectivity setup. The application is being shaped around practical shop workflows:

- product and category management
- supplier and purchase tracking
- sales and receipt reprint
- inventory movement history
- daily and monthly reporting
- local backup and restore
- role-based access for admin and cashier users

The long-term direction is an ASP.NET Core Razor Pages modular monolith backed by SQLite, with a clean path to SQL Server or PostgreSQL later if the shop grows beyond one machine.

## Project Status

The repository now contains two UI directions on top of the shared modular backend:

Current state:

- `HardwareStore.Web`: local-first Razor Pages web app
- `HardwareStore.Desktop`: Windows WPF desktop app scaffold
- shared `Application`, `Domain`, `Infrastructure`, and `Tests` projects
- EF Core with SQLite and local seeding
- early sales, purchases, products, suppliers, categories, receipt printing, and backup flows

Target state:

- both web and desktop front ends can coexist
- web stays useful for browser-based local/LAN scenarios
- desktop stays useful for single-PC shop operation
- both variants share the same business rules and SQLite infrastructure

This repository is therefore both:

- the working codebase
- the architectural roadmap for the next implementation phases

## Goals

- run reliably on one local Windows PC
- support offline-first day-to-day shop operations
- keep deployment simple for a small business
- produce readable reports and printable receipts
- keep the codebase maintainable and suitable for GitHub publication
- preserve a future migration path to multi-device LAN usage

## Why This Architecture

This system intentionally uses a modular monolith instead of microservices because the target environment is a small local business with limited internet access, single-PC operation, and a need for reliability, simple deployment, and maintainable reporting and printing workflows.

This is the right tradeoff for:

- straightforward setup
- lower operational complexity
- easier local troubleshooting
- cleaner reporting and receipt generation
- future growth without premature infrastructure overhead

## Planned Architecture

The intended solution structure is:

```text
src/
  HardwareStore.Web/
  HardwareStore.Application/
  HardwareStore.Domain/
  HardwareStore.Infrastructure/
  HardwareStore.Tests/
docs/
  architecture.md
  database.md
  printing.md
  roadmap.md
scripts/
  setup-dev.ps1
  publish-local.ps1
  backup-db.ps1
hardware-store-local.sln
```

### Layers

- `HardwareStore.Web`: Razor Pages UI, startup, authentication, page models
- `HardwareStore.Desktop`: Windows desktop UI, local login, operator shell
- `HardwareStore.Application`: business services, DTOs, validation, reporting orchestration
- `HardwareStore.Domain`: entities, enums, core invariants, business concepts
- `HardwareStore.Infrastructure`: EF Core, SQLite, repositories where justified, backup, printing integration
- `HardwareStore.Tests`: unit and integration tests

### Modules

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

## Tech Stack

- C#
- ASP.NET Core
- WPF for the Windows desktop variant
- Razor Pages for the target UI architecture
- Entity Framework Core
- SQLite for local-first storage
- Bootstrap for practical admin styling
- xUnit for tests
- HTML/CSS browser printing for the first printing version

## Key Functional Areas

### Catalog

- manage products, categories, and suppliers
- track SKU and barcode values
- support unit types such as piece, box, meter, kg, and litre

### Purchases

- record supplier purchases
- update stock on receipt
- maintain purchase history

### Sales / POS

- create sales with sale items and payments
- generate unique receipt numbers
- support receipt reprint and sale voiding
- keep cashier workflows keyboard-friendly

### Inventory

- maintain stock on hand
- record inventory movements for purchase, sale, return, and adjustment
- support stock adjustments with audit logging

### Reporting

- daily sales summary
- monthly sales summary
- sales by product
- purchase history
- stock on hand
- low stock report
- profit summary
- cashier activity
- voided sales

### Operations

- local login with roles
- audit trail for critical actions
- manual and scheduled backup
- restore workflow with warnings and confirmation

## Reporting and Printing Strategy

The first version should prefer browser printing over direct printer integration.

Phase 1 printing approach:

- print-friendly receipt page
- reprint by receipt number
- print-friendly A4 report pages
- CSS optimized for browser print dialogs

This keeps the first release reliable and simple while leaving direct printer support behind an interface for later work.

## Suggested Development Phases

### Phase 1

- solution foundation
- domain model
- EF Core SQLite setup
- products, categories, suppliers
- purchases, sales, inventory ledger
- basic reports
- browser-based printing

### Phase 2

- authentication and authorization
- audit logging
- backup and restore
- CSV export
- improved operational settings

### Phase 3

- richer reporting
- stronger printing support
- LAN deployment path
- database migration path beyond SQLite

## Recommended Branch Strategy

- `main`: stable releases
- `develop`: integration branch
- `feature/domain-model`
- `feature/sales-module`
- `feature/reporting`
- `feature/printing`

## Suggested Commit Style

- `feat: add product and category domain entities`
- `feat: implement sqlite db context and initial migration`
- `feat: add sales service with inventory transaction handling`
- `feat: add daily and monthly report pages`
- `feat: add receipt printing template`
- `fix: correct stock adjustment calculation`
- `docs: improve installation and usage guide`

## Setup

The repository is still evolving, so setup steps will change as the solution is restructured. For the current codebase:

```bash
dotnet restore hardware-store-local.sln
dotnet run --project src/HardwareStore.Web
```

For the web app:

```bash
dotnet restore
dotnet run --project src/HardwareStore.Web
```

For the Windows desktop app:

```bash
dotnet restore
dotnet build src/HardwareStore.Desktop
```

Note:

- `HardwareStore.Desktop` targets Windows WPF and should be built and run on Windows
- the current Codex environment may scaffold the project but cannot execute the Windows UI on macOS

## Engineering Standards

- use clear names and small focused classes
- keep page models thin
- keep business logic in application services
- prefer async for database work
- use EF Core Fluent API for configuration
- avoid overengineering and unnecessary abstractions
- build for a real local business workflow, not a demo-only sample

## Roadmap

Near-term priorities:

- restructure the solution into layered projects
- move UI toward Razor Pages
- formalize the domain model and application services
- add SQLite migrations and seed data
- implement receipts, reports, auth, backup, and audit logging

## Documentation

- [Architecture](./docs/architecture.md)
- [Database](./docs/database.md)

Additional docs planned:

- `docs/database.md`
- `docs/printing.md`
- `docs/roadmap.md`

## License

Choose and add a license before public release.
