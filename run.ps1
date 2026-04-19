param(
    [switch]$Build,
    [switch]$Down
)

$root = $PSScriptRoot
$composeFile = Join-Path $root "src\docker-compose.yml"
$envFile = Join-Path $root ".env"

if (-not (Test-Path $envFile)) {
    Write-Error ".env file not found at $envFile. Copy .env.example and fill in values."
    exit 1
}

if ($Down) {
    docker compose -f $composeFile --env-file $envFile down
    exit $LASTEXITCODE
}

$composeArgs = @("-f", $composeFile, "--env-file", $envFile, "up")
if ($Build) { $composeArgs += "--build" }

docker compose @composeArgs
exit $LASTEXITCODE
