param(
    [switch]$Build,
    [switch]$Down,
    [switch]$SyncCerts
)

function ConvertTo-WslPath($windowsPath) {
    $path = $windowsPath -replace '\\', '/'
    if ($path -match '^([A-Za-z]):(.*)') {
        return "/mnt/$($Matches[1].ToLower())$($Matches[2])"
    }
    return $path
}

function Sync-CaCertificates {
    Write-Host "Syncing Windows CA certificates to WSL..."

    $certs = Get-ChildItem -Path Cert:\LocalMachine\Root
    $lines = New-Object System.Collections.Generic.List[string]
    foreach ($cert in $certs) {
        $b64 = [Convert]::ToBase64String($cert.RawData)
        $lines.Add("-----BEGIN CERTIFICATE-----")
        for ($i = 0; $i -lt $b64.Length; $i += 64) {
            $lines.Add($b64.Substring($i, [Math]::Min(64, $b64.Length - $i)))
        }
        $lines.Add("-----END CERTIFICATE-----")
    }

    $tempFile = Join-Path $env:TEMP "windows-ca-bundle.crt"
    [System.IO.File]::WriteAllLines($tempFile, $lines)

    $wslTempFile = ConvertTo-WslPath $tempFile
    wsl bash -c "sudo cp '$wslTempFile' /usr/local/share/ca-certificates/windows-ca-bundle.crt && sudo update-ca-certificates &>/dev/null"
    Write-Host "CA certificates synced."
}

$root = $PSScriptRoot
$composeFile = Join-Path $root "src\docker-compose.yml"
$envFile = Join-Path $root ".env"

if (-not (Test-Path $envFile)) {
    Write-Error ".env file not found at $envFile. Copy .env.example and fill in values."
    exit 1
}

$wslComposeFile = ConvertTo-WslPath $composeFile
$wslEnvFile = ConvertTo-WslPath $envFile

$certSynced = $false
$certBundleExists = wsl bash -c "test -f /usr/local/share/ca-certificates/windows-ca-bundle.crt && echo yes"
if ($SyncCerts -or $certBundleExists -ne "yes") {
    Sync-CaCertificates
    $certSynced = $true
}

$dockerRunning = wsl bash -c "docker info &>/dev/null && echo running"
if ($certSynced -and $dockerRunning -eq "running") {
    Write-Host "Restarting Docker daemon to apply updated certificates..."
    wsl bash -c "sudo pkill dockerd; sleep 2"
    $dockerRunning = "stopped"
}

if ($dockerRunning -ne "running") {
    Write-Host "Starting Docker daemon in WSL..."
    wsl bash -c "sudo dockerd &>/dev/null &"
    Write-Host "Waiting for Docker daemon to be ready..."
    wsl bash -c "until docker info &>/dev/null; do sleep 1; done"
}

if ($Down) {
    wsl docker compose -f $wslComposeFile --env-file $wslEnvFile down
    exit $LASTEXITCODE
}

$mcpToken = (Get-Content $envFile | Where-Object { $_ -match '^MCP_PROXY_AUTH_TOKEN=' }) -replace '^MCP_PROXY_AUTH_TOKEN=', ''
$inspectorUrl = "http://localhost:6274?MCP_PROXY_AUTH_TOKEN=$mcpToken"

Write-Host ""
Write-Host "+------------------------------+---------------+------------------------------------------+"
Write-Host "| Container                    | Type          | Link                                     |"
Write-Host "+------------------------------+---------------+------------------------------------------+"
Write-Host "| task-manager-db              | Postgres      |                                          |"
Write-Host "| task-manager-api             | .NET API      | http://localhost:8080/docs (Swagger UI)  |"
Write-Host "| task-manager-mcp             | MCP Server    | http://localhost:5050                    |"
Write-Host "| task-manager-mcp-inspector   | MCP Inspector | $inspectorUrl                            |"
Write-Host "+------------------------------+---------------+------------------------------------------+"
Write-Host ""

if ($Build) {
    wsl docker compose -f $wslComposeFile --env-file $wslEnvFile up --build
} else {
    wsl docker compose -f $wslComposeFile --env-file $wslEnvFile up
}
exit $LASTEXITCODE
