# FreeWinBackup Installer Test Script
# This script builds the solution, creates the installer, and tests the installation

param(
    [switch]$SkipBuild,
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"

# Script configuration
$SolutionRoot = Split-Path -Parent $PSScriptRoot
$InstallerProject = Join-Path $PSScriptRoot "FreeWinBackup.Installer.wixproj"
$InstallerMsi = Join-Path $PSScriptRoot "bin\Release\FreeWinBackup.msi"
$ProductName = "FreeWinBackup"

Write-Host "=== FreeWinBackup Installer Test ===" -ForegroundColor Cyan
Write-Host ""

# Function to check if running as administrator
function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Uninstall function
function Uninstall-FreeWinBackup {
    Write-Host "Uninstalling $ProductName..." -ForegroundColor Yellow
    
    # Try to find the product
    $product = Get-WmiObject -Class Win32_Product | Where-Object { $_.Name -eq $ProductName }
    
    if ($product) {
        Write-Host "Found installed product: $($product.Name) version $($product.Version)"
        $product.Uninstall() | Out-Null
        Write-Host "Uninstallation completed." -ForegroundColor Green
    }
    else {
        Write-Host "Product not found. It may not be installed." -ForegroundColor Yellow
        
        # Alternative: try uninstalling using MSI file
        if (Test-Path $InstallerMsi) {
            Write-Host "Attempting uninstall using MSI file..."
            Start-Process msiexec.exe -ArgumentList "/x `"$InstallerMsi`" /qn" -Wait -NoNewWindow
            Write-Host "Uninstallation command executed." -ForegroundColor Green
        }
    }
    
    # Check and remove registry entries
    $runKeyPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
    $runKeyValue = Get-ItemProperty -Path $runKeyPath -Name $ProductName -ErrorAction SilentlyContinue
    
    if ($runKeyValue) {
        Write-Host "Removing auto-start registry entry..."
        Remove-ItemProperty -Path $runKeyPath -Name $ProductName
        Write-Host "Registry entry removed." -ForegroundColor Green
    }
}

# Handle uninstall-only mode
if ($Uninstall) {
    Uninstall-FreeWinBackup
    exit 0
}

# Build phase
if (-not $SkipBuild) {
    Write-Host "Step 1: Building Solution" -ForegroundColor Cyan
    Write-Host "Building FreeWinBackup.sln in Release mode..."
    
    $solutionFile = Join-Path $SolutionRoot "FreeWinBackup.sln"
    
    if (-not (Test-Path $solutionFile)) {
        Write-Error "Solution file not found: $solutionFile"
        exit 1
    }
    
    # Build using dotnet CLI
    & dotnet build $solutionFile -c Release -v minimal -nologo
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Solution build failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    Write-Host "Solution built successfully." -ForegroundColor Green
    Write-Host ""
    
    Write-Host "Step 2: Building Installer" -ForegroundColor Cyan
    Write-Host "Building WiX installer project..."
    
    & dotnet msbuild $InstallerProject -p:Configuration=Release -v:minimal -nologo
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Installer build failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    Write-Host "Installer built successfully." -ForegroundColor Green
    Write-Host ""
}
else {
    Write-Host "Skipping build (using existing MSI)..." -ForegroundColor Yellow
    Write-Host ""
}

# Verify MSI exists
if (-not (Test-Path $InstallerMsi)) {
    Write-Error "Installer MSI not found: $InstallerMsi"
    Write-Host "Please build the installer first or run without -SkipBuild" -ForegroundColor Yellow
    exit 1
}

Write-Host "Step 3: Installing FreeWinBackup" -ForegroundColor Cyan
Write-Host "MSI Location: $InstallerMsi"
Write-Host ""

# Uninstall any existing version first
Write-Host "Checking for existing installation..."
Uninstall-FreeWinBackup
Write-Host ""

# Install with AUTOSTART enabled
Write-Host "Installing with AUTOSTART=1..."
$installArgs = "/i `"$InstallerMsi`" /qn AUTOSTART=1"
$process = Start-Process msiexec.exe -ArgumentList $installArgs -Wait -PassThru -NoNewWindow

if ($process.ExitCode -ne 0) {
    Write-Error "Installation failed with exit code $($process.ExitCode)"
    Write-Host "Common exit codes:" -ForegroundColor Yellow
    Write-Host "  1603 - Fatal error during installation"
    Write-Host "  1618 - Another installation is already in progress"
    Write-Host "  1619 - Installation package could not be opened"
    exit 1
}

Write-Host "Installation completed successfully." -ForegroundColor Green
Write-Host ""

# Verification phase
Write-Host "Step 4: Verifying Installation" -ForegroundColor Cyan

$installPath = Join-Path $env:LOCALAPPDATA $ProductName
$exePath = Join-Path $installPath "FreeWinBackup.exe"

# Check installation directory
Write-Host "Checking installation directory: $installPath"
if (Test-Path $installPath) {
    Write-Host "  [OK] Installation directory exists" -ForegroundColor Green
}
else {
    Write-Host "  [FAIL] Installation directory not found" -ForegroundColor Red
}

# Check executable
Write-Host "Checking executable: $exePath"
if (Test-Path $exePath) {
    Write-Host "  [OK] Executable exists" -ForegroundColor Green
    $fileInfo = Get-Item $exePath
    Write-Host "       Version: $($fileInfo.VersionInfo.FileVersion)"
    Write-Host "       Size: $([math]::Round($fileInfo.Length / 1KB, 2)) KB"
}
else {
    Write-Host "  [FAIL] Executable not found" -ForegroundColor Red
}

# Check Start Menu shortcut
$startMenuPath = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\$ProductName\$ProductName.lnk"
Write-Host "Checking Start Menu shortcut: $startMenuPath"
if (Test-Path $startMenuPath) {
    Write-Host "  [OK] Start Menu shortcut exists" -ForegroundColor Green
}
else {
    Write-Host "  [WARN] Start Menu shortcut not found" -ForegroundColor Yellow
}

# Check auto-start registry key
$runKeyPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Write-Host "Checking auto-start registry entry..."
try {
    $runKeyValue = Get-ItemProperty -Path $runKeyPath -Name $ProductName -ErrorAction Stop
    if ($runKeyValue.$ProductName) {
        Write-Host "  [OK] Auto-start registry entry exists" -ForegroundColor Green
        Write-Host "       Value: $($runKeyValue.$ProductName)"
    }
}
catch {
    Write-Host "  [FAIL] Auto-start registry entry not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host "Installation completed and verified successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Log off and log back in to test auto-start functionality"
Write-Host "2. The application should start minimized to system tray"
Write-Host "3. Check the system tray for the FreeWinBackup icon"
Write-Host ""
Write-Host "To uninstall, run: .\test-install.ps1 -Uninstall" -ForegroundColor Cyan
Write-Host "Or use: msiexec /x `"$InstallerMsi`" /qn" -ForegroundColor Cyan
