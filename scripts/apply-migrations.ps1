$ErrorActionPreference = "Stop"
. "$PSScriptRoot/common-sqlserver.ps1"
Read-DotEnv

Write-Host "Validating database before migrations..."
Invoke-ProjectSqlCmd -InputFile "$PSScriptRoot/../database/sqlserver/003-validate-database.sql" -Database "master"

Write-Host "Listing pending EF Core migrations..."
dotnet ef migrations list --project "$PSScriptRoot/../src/HistoriasPaolin.Infrastructure/HistoriasPaolin.Infrastructure.csproj" --startup-project "$PSScriptRoot/../src/HistoriasPaolin.Api/HistoriasPaolin.Api.csproj"
if ($LASTEXITCODE -ne 0) { throw "dotnet ef migrations list failed." }

Write-Host "Applying EF Core migrations..."
dotnet ef database update --project "$PSScriptRoot/../src/HistoriasPaolin.Infrastructure/HistoriasPaolin.Infrastructure.csproj" --startup-project "$PSScriptRoot/../src/HistoriasPaolin.Api/HistoriasPaolin.Api.csproj"
if ($LASTEXITCODE -ne 0) { throw "dotnet ef database update failed." }

Write-Host "Validating database after migrations..."
Invoke-ProjectSqlCmd -InputFile "$PSScriptRoot/../database/sqlserver/003-validate-database.sql" -Database "master"
