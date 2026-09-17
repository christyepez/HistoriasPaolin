param([switch]$ApplyPermissions)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot/common-sqlserver.ps1"
Read-DotEnv

Write-Host "Validating SQL Server tooling and connectivity..."
Invoke-ProjectSqlCmd -Query "SELECT @@VERSION;" -Database "master"

Write-Host "Creating HistoriasPaolinDb when missing..."
Invoke-ProjectSqlCmd -InputFile "$PSScriptRoot/../database/sqlserver/001-create-database.sql" -Database "master"

if ($ApplyPermissions) {
    Write-Host "Applying explicit permission script. Ensure it contains only local, reviewed values."
    Invoke-ProjectSqlCmd -InputFile "$PSScriptRoot/../database/sqlserver/002-create-login-user.sql" -Database "master"
}
else {
    Write-Host "Skipping permissions script. Pass -ApplyPermissions only after editing local values outside Git."
}

Write-Host "SQL Server initialization completed."
