@echo off
setlocal
cd /d "%~dp0"
echo ===== Kolok SalesHub - setup and run =====

where dotnet >nul 2>nul || (echo .NET SDK not found. Install the .NET 10 SDK first. & pause & exit /b 1)

echo [1/4] Restoring tools (dotnet-ef)...
dotnet tool restore || goto :fail

echo [2/4] Building solution...
dotnet build SalesHub.sln || goto :fail

if not exist "src\SalesHub.Infrastructure\Migrations" (
  echo [3/4] Creating InitialCreate migration...
  dotnet ef migrations add InitialCreate --project src\SalesHub.Infrastructure --startup-project src\SalesHub.Api || goto :fail
) else (
  echo [3/4] Migrations already exist - skipping.
)

echo [4/4] Updating database and starting API...
dotnet ef database update --project src\SalesHub.Infrastructure --startup-project src\SalesHub.Api || goto :fail

echo.
echo Open:  https://localhost:7180/swagger
echo        https://localhost:7180/api/health
echo.
dotnet run --project src\SalesHub.Api --launch-profile https
goto :eof

:fail
echo.
echo *** Something failed. Check the message above (usually the SQL Server name in src\SalesHub.Api\appsettings.json). ***
pause
exit /b 1
