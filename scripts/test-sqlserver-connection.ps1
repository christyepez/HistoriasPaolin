$ErrorActionPreference = "Stop"
. "$PSScriptRoot/common-sqlserver.ps1"
Read-DotEnv

$target = Get-SqlServerTarget
$hostOnly = ($target -split "[,\\]")[0]
Write-Host "Resolving SQL Server host: $hostOnly"
Resolve-DnsName $hostOnly -ErrorAction Stop | Select-Object -First 1 | Out-Host

if ([string]::IsNullOrWhiteSpace($env:SQLSERVER_INSTANCE) -and -not [string]::IsNullOrWhiteSpace($env:SQLSERVER_PORT)) {
    Write-Host "Testing TCP port $($env:SQLSERVER_PORT)..."
    Test-NetConnection -ComputerName $hostOnly -Port ([int]$env:SQLSERVER_PORT) | Out-Host
}

Write-Host "Testing SQL Server version..."
Invoke-ProjectSqlCmd -Query "SELECT @@VERSION; SELECT DB_NAME();" -Database "master"

Write-Host "Validating HistoriasPaolinDb access..."
Invoke-ProjectSqlCmd -InputFile "$PSScriptRoot/../database/sqlserver/003-validate-database.sql" -Database "master"
