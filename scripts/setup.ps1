# setup.ps1 — Run once after cloning webium-dev
$ErrorActionPreference = "Stop"

# Resolve the sibling webium clone path
$webiumPath = Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "..") "webium"

if (-not (Test-Path $webiumPath)) {
    Write-Error "Webium clone not found at $webiumPath. Clone it: git clone https://github.com/ren0xy/webium.git `"$webiumPath`""
    exit 1
}

# Install npm dependencies
Push-Location (Join-Path (Join-Path $webiumPath "packages") "core")
npm install
npm run build:bundle
Pop-Location

# Place JS bundle as Unity resource
New-Item -ItemType Directory -Force -Path (Join-Path $webiumPath "Resources")
Copy-Item (Join-Path (Join-Path $webiumPath "build") "webium-bootstrap.js") (Join-Path (Join-Path $webiumPath "Resources") "webium-bootstrap.txt")

Write-Host "Setup complete. Open Unity and check for compilation errors."
