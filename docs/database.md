# Database Management

## Database Engine

This application uses SQLite for local-first single-PC operation.

## Default Location

The database file is configured through:

- [appsettings.json](/Users/kyiphyu/Repo/FamilyHardwareStore/FamilyHardwareStore/src/HardwareStore.Web/appsettings.json)
- [InfrastructureServiceCollectionExtensions.cs](/Users/kyiphyu/Repo/FamilyHardwareStore/FamilyHardwareStore/src/HardwareStore.Infrastructure/Configuration/InfrastructureServiceCollectionExtensions.cs)

Default settings:

- `DataDirectory`: `App_Data`
- `DatabaseFileName`: `hardwarestore.db`

If `DefaultConnection` is empty, the app creates a local SQLite file automatically.

## Recommended Local Setup

For a clear fixed path during development, set:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=/Users/kyiphyu/Repo/FamilyHardwareStore/FamilyHardwareStore/localdb/hardwarestore.db"
}
```

Then create the folder:

```bash
mkdir -p /Users/kyiphyu/Repo/FamilyHardwareStore/FamilyHardwareStore/localdb
```

## Creation and Seeding

On startup the app:

1. creates the SQLite database if it does not exist
2. seeds default users, categories, and application settings

Seeded users:

- `admin` / `Admin@123`
- `cashier` / `Cashier@123`

## Backups

Current backup support:

- manual backup page in the app
- PowerShell helper in `scripts/backup-db.ps1`

Recommended practice:

- keep the live DB under a dedicated local folder
- copy backups to a separate `backups/` directory
- do not edit the live database file while the app is running

## Inspection

If SQLite CLI is installed:

```bash
sqlite3 /path/to/hardwarestore.db
```

Useful commands:

```sql
.tables
select * from Users;
select * from Categories;
select * from AppSettings;
```

## Operational Guidance

- use SQLite locally, not an online hosted database, for this app’s target environment
- back up the DB file regularly
- treat the DB file as shop-critical data
- when moving machines, copy the DB and backups together
- for larger future deployments, plan a migration to SQL Server or PostgreSQL
