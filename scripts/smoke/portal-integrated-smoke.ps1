param(
    [string]$PortalGatewayUrl = "http://localhost:8082",
    [string]$HistoriasPaolinUrl = "http://localhost:5080",
    [string]$JwtToken,
    [string]$JwtSecret,
    [string]$JwtIssuer = "portal-corporativo",
    [string]$JwtAudience = "portal-corporativo-clients"
)

$ErrorActionPreference = "Stop"

function Join-Url {
    param([string]$BaseUrl, [string]$Path)
    return $BaseUrl.TrimEnd("/") + "/" + $Path.TrimStart("/")
}

function ConvertTo-Base64Url {
    param([byte[]]$Bytes)
    return [Convert]::ToBase64String($Bytes).TrimEnd("=").Replace("+", "-").Replace("/", "_")
}

function New-DevJwt {
    param(
        [string]$Secret,
        [string]$Issuer,
        [string]$Audience
    )

    if ([string]::IsNullOrWhiteSpace($Secret) -or $Secret.Length -lt 32) {
        throw "JwtSecret must contain at least 32 characters."
    }

    $headerJson = @{ alg = "HS256"; typ = "JWT" } | ConvertTo-Json -Compress
    $payloadJson = @{
        sub = "historiaspaolin-portal-smoke"
        iss = $Issuer
        aud = $Audience
        exp = [DateTimeOffset]::UtcNow.AddMinutes(15).ToUnixTimeSeconds()
        permission = @(
            "historiaspaolin.episodes.view",
            "historiaspaolin.episodes.create"
        )
    } | ConvertTo-Json -Compress

    $header = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($headerJson))
    $payload = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($payloadJson))
    $unsigned = "$header.$payload"
    $hmac = [Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($Secret))
    $signature = ConvertTo-Base64Url ($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($unsigned)))
    return "$unsigned.$signature"
}

function Invoke-SmokeRequest {
    param(
        [string]$Method = "GET",
        [string]$Uri,
        [hashtable]$Headers,
        [string]$Body
    )

    try {
        $parameters = @{
            Uri = $Uri
            Method = $Method
            Headers = $Headers
            UseBasicParsing = $true
            TimeoutSec = 15
        }

        if ($Body) {
            $parameters.ContentType = "application/json"
            $parameters.Body = $Body
        }

        $response = Invoke-WebRequest @parameters
        return [pscustomobject]@{
            StatusCode = [int]$response.StatusCode
            Body = $response.Content
            Headers = $response.Headers
        }
    }
    catch {
        $response = $_.Exception.Response
        $statusCode = if ($response -and $response.StatusCode) { [int]$response.StatusCode } else { 0 }
        return [pscustomobject]@{
            StatusCode = $statusCode
            Body = $_.Exception.Message
            Headers = if ($response) { $response.Headers } else { @{} }
        }
    }
}

$results = [System.Collections.Generic.List[object]]::new()

function Add-Result {
    param(
        [string]$Name,
        [string]$Status,
        [string]$Detail
    )

    $script:results.Add([pscustomobject]@{
        Test = $Name
        Status = $Status
        Detail = $Detail
    })
}

if ([string]::IsNullOrWhiteSpace($JwtToken) -and -not [string]::IsNullOrWhiteSpace($JwtSecret)) {
    $JwtToken = New-DevJwt -Secret $JwtSecret -Issuer $JwtIssuer -Audience $JwtAudience
}

$correlationId = "hp-smoke-$([Guid]::NewGuid().ToString("N"))"
$baseHeaders = @{ "X-Correlation-Id" = $correlationId }
$authHeaders = @{} + $baseHeaders
if (-not [string]::IsNullOrWhiteSpace($JwtToken)) {
    $authHeaders.Authorization = "Bearer $JwtToken"
}

$gatewayHealth = Invoke-SmokeRequest -Uri (Join-Url $PortalGatewayUrl "/health/ready") -Headers $baseHeaders
if ($gatewayHealth.StatusCode -eq 200) {
    Add-Result "Portal Gateway health" "PASS" "Expected 200, got 200."
}
else {
    Add-Result "Portal Gateway health" "FAIL" "Expected 200, got $($gatewayHealth.StatusCode)."
}

$directHealth = Invoke-SmokeRequest -Uri (Join-Url $HistoriasPaolinUrl "/health") -Headers $baseHeaders
if ($directHealth.StatusCode -eq 200) {
    Add-Result "HistoriasPaolin direct health" "PASS" "Expected 200, got 200."
}
else {
    Add-Result "HistoriasPaolin direct health" "FAIL" "Expected 200, got $($directHealth.StatusCode)."
}

$gatewayHpHealth = Invoke-SmokeRequest -Uri (Join-Url $PortalGatewayUrl "/api/historiaspaolin/health") -Headers $baseHeaders
if ($gatewayHpHealth.StatusCode -eq 200) {
    Add-Result "HistoriasPaolin health through Gateway" "PASS" "Expected 200, got 200."
}
else {
    Add-Result "HistoriasPaolin health through Gateway" "FAIL" "Expected 200, got $($gatewayHpHealth.StatusCode)."
}

$protectedNoToken = Invoke-SmokeRequest -Uri (Join-Url $PortalGatewayUrl "/api/historiaspaolin/episodes") -Headers $baseHeaders
if ($protectedNoToken.StatusCode -eq 401) {
    Add-Result "Protected endpoint without token" "PASS" "Expected 401, got 401."
}
else {
    Add-Result "Protected endpoint without token" "FAIL" "Expected 401, got $($protectedNoToken.StatusCode)."
}

$protectedWithToken = $null
if ([string]::IsNullOrWhiteSpace($JwtToken)) {
    Add-Result "Protected endpoint with token" "SKIP" "JwtToken/JwtSecret was not provided."
}
else {
    $protectedWithToken = Invoke-SmokeRequest -Uri (Join-Url $PortalGatewayUrl "/api/historiaspaolin/episodes") -Headers $authHeaders
    if ($protectedWithToken.StatusCode -eq 200 -or $protectedWithToken.StatusCode -eq 403) {
        Add-Result "Protected endpoint with token" "PASS" "Expected 200 or 403, got $($protectedWithToken.StatusCode)."
    }
    else {
        Add-Result "Protected endpoint with token" "FAIL" "Expected 200 or 403, got $($protectedWithToken.StatusCode)."
    }
}

$correlationHeader = $gatewayHpHealth.Headers["X-Correlation-ID"]
if (-not $correlationHeader) {
    $correlationHeader = $gatewayHpHealth.Headers["X-Correlation-Id"]
}

if (($correlationHeader -join ",") -like "*$correlationId*") {
    Add-Result "Correlation ID" "PASS" "Correlation ID was returned by the gateway/downstream response."
}
else {
    Add-Result "Correlation ID" "FAIL" "Correlation ID was not observed in the gateway/downstream response."
}

if ($protectedWithToken -and $protectedWithToken.StatusCode -eq 200) {
    Add-Result "SQL indirect endpoint" "PASS" "Protected episode list returned 200 through Gateway."
}
elseif ([string]::IsNullOrWhiteSpace($JwtToken)) {
    Add-Result "SQL indirect endpoint" "SKIP" "JwtToken/JwtSecret was not provided."
}
else {
    Add-Result "SQL indirect endpoint" "FAIL" "Expected protected episode list to return 200, got $($protectedWithToken.StatusCode)."
}

$results | Format-Table -AutoSize

$failed = @($results | Where-Object { $_.Status -eq "FAIL" }).Count
$skipped = @($results | Where-Object { $_.Status -eq "SKIP" }).Count

Write-Host "Summary: PASS=$(@($results | Where-Object { $_.Status -eq "PASS" }).Count) FAIL=$failed SKIP=$skipped"

if ($failed -gt 0) {
    exit 1
}

exit 0
