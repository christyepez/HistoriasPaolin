param([string]$Confirm)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot/common-sqlserver.ps1"
Read-DotEnv

if ($env:ASPNETCORE_ENVIRONMENT -ne "Development") {
    throw "Reset is allowed only when ASPNETCORE_ENVIRONMENT=Development."
}

if ($env:SQLSERVER_DATABASE -ne "HistoriasPaolinDb") {
    throw "Reset blocked. SQLSERVER_DATABASE must be exactly HistoriasPaolinDb."
}

if ($Confirm -ne "RESET HistoriasPaolinDb") {
    throw "Explicit confirmation required: -Confirm 'RESET HistoriasPaolinDb'"
}

Write-Host "Dropping and recreating HistoriasPaolinDb..."
Invoke-ProjectSqlCmd -Query "IF DB_ID(N'HistoriasPaolinDb') IS NOT NULL BEGIN ALTER DATABASE [HistoriasPaolinDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [HistoriasPaolinDb]; END; CREATE DATABASE [HistoriasPaolinDb];" -Database "master"

& "$PSScriptRoot/apply-migrations.ps1"
