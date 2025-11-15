<#
.SYNOPSIS
    Uninstalls the FreeWinBackup Windows Service

.DESCRIPTION
    This script stops and removes the FreeWinBackup Windows Service.
    Configuration files and logs in %AppData%\FreeWinBackup are preserved by default.

.PARAMETER ServiceName
    The name of the Windows service to uninstall. Default is "FreeWinBackup"

.PARAMETER RemoveData
    If specified, also removes configuration and log files from %AppData%\FreeWinBackup

.EXAMPLE
    .\Uninstall-FreeWinBackupService.ps1
    Uninstalls the service but keeps configuration and logs

.EXAMPLE
    .\Uninstall-FreeWinBackupService.ps1 -RemoveData
    Uninstalls the service and removes all configuration and log files

.EXAMPLE
    .\Uninstall-FreeWinBackupService.ps1 -ServiceName "MyBackupService"
    Uninstalls a service with a custom name

.NOTES
    This script requires Administrator privileges to uninstall Windows services.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FreeWinBackup",
    
    [Parameter(Mandatory=$false)]
    [switch]$RemoveData
)

# Ensure running as Administrator
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error "This script must be run as Administrator. Please run PowerShell as Administrator and try again."
    exit 1
}

Write-Host "Uninstalling FreeWinBackup Windows Service..." -ForegroundColor Green

# Check if service exists
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if (-not $service) {
    Write-Warning "Service '$ServiceName' is not installed."
    exit 0
}

# Stop the service if running
if ($service.Status -eq 'Running') {
    Write-Host "Stopping service..." -ForegroundColor Cyan
    try {
        Stop-Service -Name $ServiceName -Force -ErrorAction Stop
        Write-Host "Service stopped successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to stop service: $_"
        exit 1
    }
    
    # Wait for service to fully stop
    Start-Sleep -Seconds 2
}

# Delete the service
Write-Host "Removing service..." -ForegroundColor Cyan
try {
    $result = sc.exe delete $ServiceName
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Service removed successfully." -ForegroundColor Green
    }
    else {
        Write-Error "Failed to remove service. sc.exe returned: $result"
        exit 1
    }
}
catch {
    Write-Error "Failed to remove service: $_"
    exit 1
}

# Remove data if requested
if ($RemoveData) {
    $appDataPath = Join-Path $env:APPDATA "FreeWinBackup"
    
    if (Test-Path $appDataPath) {
        Write-Host "`nRemoving configuration and log files..." -ForegroundColor Yellow
        Write-Warning "This will delete all backup schedules and logs from: $appDataPath"
        
        $confirmation = Read-Host "Are you sure you want to delete all data? (yes/no)"
        if ($confirmation -eq 'yes') {
            try {
                Remove-Item -Path $appDataPath -Recurse -Force -ErrorAction Stop
                Write-Host "Data removed successfully." -ForegroundColor Green
            }
            catch {
                Write-Error "Failed to remove data: $_"
            }
        }
        else {
            Write-Host "Data removal cancelled. Configuration and logs preserved." -ForegroundColor Cyan
        }
    }
    else {
        Write-Host "No data directory found at: $appDataPath" -ForegroundColor Cyan
    }
}
else {
    Write-Host "`nConfiguration and logs preserved in: $env:APPDATA\FreeWinBackup" -ForegroundColor Cyan
    Write-Host "To remove data, run: .\Uninstall-FreeWinBackupService.ps1 -RemoveData" -ForegroundColor Yellow
}

Write-Host "`nUninstallation complete!" -ForegroundColor Green
