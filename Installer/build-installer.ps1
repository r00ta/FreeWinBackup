# Build FreeWinBackup Installer
# Simple script to build the solution and create the MSI installer

$ErrorActionPreference = "Stop"

$SolutionRoot = Split-Path -Parent $PSScriptRoot
$SolutionFile = Join-Path $SolutionRoot "FreeWinBackup.sln"
$InstallerProject = Join-Path $PSScriptRoot "FreeWinBackup.Installer.wixproj"

Write-Host "=== Building FreeWinBackup Installer ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build the main solution
Write-Host "Building solution: $SolutionFile" -ForegroundColor Yellow
& msbuild $SolutionFile /p:Configuration=Release /p:Platform="Any CPU" /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Solution build failed!"
    exit 1
}

Write-Host "Solution build successful!" -ForegroundColor Green
Write-Host ""

# Step 2: Build the installer
Write-Host "Building installer: $InstallerProject" -ForegroundColor Yellow
& msbuild $InstallerProject /p:Configuration=Release /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Installer build failed!"
    exit 1
}

Write-Host "Installer build successful!" -ForegroundColor Green
Write-Host ""

$msiPath = Join-Path $PSScriptRoot "bin\Release\FreeWinBackup.msi"
if (Test-Path $msiPath) {
    $msiInfo = Get-Item $msiPath
    Write-Host "Installer created: $msiPath" -ForegroundColor Green
    Write-Host "Size: $([math]::Round($msiInfo.Length / 1MB, 2)) MB"
    Write-Host "Created: $($msiInfo.CreationTime)"
}

Write-Host ""
Write-Host "Build complete!" -ForegroundColor Cyan
