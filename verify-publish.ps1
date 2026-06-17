$ErrorActionPreference = "Stop"

Write-Host "=== Starting local publish verification ===" -ForegroundColor Cyan

# 1. Run npm install
Write-Host "Running npm install..." -ForegroundColor Green
npm install
if ($LASTEXITCODE -ne 0) {
    Write-Error "npm install failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# 2. Run dotnet publish
Write-Host "Running dotnet publish..." -ForegroundColor Green
dotnet publish MorgueManager.Web.csproj -c Release -o publish
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# 3. Verify output files exist
$wwwrootPath = Join-Path $PSScriptRoot "publish/wwwroot"
if (-not (Test-Path $wwwrootPath)) {
    Write-Error "Verification failed: publish/wwwroot directory does not exist."
    exit 1
}

# Check for index.html or _framework
$indexPath = Join-Path $wwwrootPath "index.html"
if (-not (Test-Path $indexPath)) {
    Write-Error "Verification failed: index.html not found in publish/wwwroot."
    exit 1
}

Write-Host "=== Verification Succeeded: Build completed successfully! ===" -ForegroundColor Green
exit 0
