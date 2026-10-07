# Kolok SalesHub - Internal CRM POC

## Requirements
- .NET 10 SDK (Visual Studio 2026 / latest VS with .NET 10)
- SQL Server Developer (or Express)

## Run it (easiest)
1. Extract the zip.
2. If SQL Server is a named instance (e.g. SQLEXPRESS), edit `src/SalesHub.Api/appsettings.json`:
   `Server=localhost\\SQLEXPRESS;...`
3. Double-click **run.bat**. It builds, creates the InitialCreate migration, creates SalesHubDb and starts the API.
4. Open https://localhost:7180/swagger

## Run from Visual Studio
1. Open `SalesHub.sln`. Set **SalesHub.Api** as startup project.
2. Package Manager Console, Default project = SalesHub.Infrastructure:
   Add-Migration InitialCreate -Project SalesHub.Infrastructure -StartupProject SalesHub.Api
   Update-Database -Project SalesHub.Infrastructure -StartupProject SalesHub.Api
3. F5 -> browser opens /swagger.

## Endpoints
- GET  /api/health
- GET/POST /api/customers   (GET supports ?search=)
- GET/PUT  /api/customers/{id}
- GET/POST /api/customers/{id}/contacts
- /openapi/v1.json, /swagger

## Seed test data
- Users: Sales Manager (1), Test Salesperson (2)
- Customer: TEST001 ABC Office Supplies, contact John Smith
