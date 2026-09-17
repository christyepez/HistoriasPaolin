function Read-DotEnv {
    param([string]$Path = ".env")
    if (-not (Test-Path -LiteralPath $Path)) { return }
    Get-Content -LiteralPath $Path | ForEach-Object {
        $line = $_.Trim()
        if ($line.Length -eq 0 -or $line.StartsWith("#")) { return }
        $parts = $line.Split("=", 2)
        if ($parts.Length -eq 2 -and -not [Environment]::GetEnvironmentVariable($parts[0])) {
            [Environment]::SetEnvironmentVariable($parts[0], $parts[1], "Process")
        }
    }
}

function Get-SqlServerTarget {
    $hostName = $env:SQLSERVER_HOST
    if ([string]::IsNullOrWhiteSpace($hostName)) { $hostName = "host.docker.internal" }
    $instance = $env:SQLSERVER_INSTANCE
    $port = $env:SQLSERVER_PORT

    if (-not [string]::IsNullOrWhiteSpace($instance)) {
        if (-not $instance.StartsWith("\")) { $instance = "\$instance" }
        return "$hostName$instance"
    }

    if (-not [string]::IsNullOrWhiteSpace($port)) { return "$hostName,$port" }
    return $hostName
}

function Get-SqlCmdAuthArgs {
    $integrated = $env:SQLSERVER_INTEGRATED_SECURITY -eq "true"
    if ($integrated) { return @("-E") }
    if ([string]::IsNullOrWhiteSpace($env:SQLSERVER_USER) -or [string]::IsNullOrWhiteSpace($env:SQLSERVER_PASSWORD)) {
        throw "SQLSERVER_USER and SQLSERVER_PASSWORD are required when SQLSERVER_INTEGRATED_SECURITY=false."
    }
    return @("-U", $env:SQLSERVER_USER, "-P", $env:SQLSERVER_PASSWORD)
}

function Invoke-ProjectSqlCmd {
    param(
        [string]$Query,
        [string]$InputFile,
        [string]$Database = "master"
    )

    if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
        throw "sqlcmd is not installed or not available in PATH."
    }

    $target = Get-SqlServerTarget
    $auth = Get-SqlCmdAuthArgs
    $args = @("-S", $target, "-d", $Database, "-C", "-b") + $auth
    if ($Query) { $args += @("-Q", $Query) }
    if ($InputFile) { $args += @("-i", $InputFile) }
    & sqlcmd @args
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd failed with exit code $LASTEXITCODE." }
}
