<#
.SYNOPSIS
    Installs the FreeWinBackup Windows Service

.DESCRIPTION
    This script installs and configures the FreeWinBackup Windows Service for automated background backups.
    It creates the service, configures it to start automatically, and sets up service recovery options.

.PARAMETER ServiceName
    The name of the Windows service. Default is "FreeWinBackup"

.PARAMETER BinaryPath
    The full path to FreeWinBackup.ServiceHost.exe. If not specified, looks in the Release build output.

.PARAMETER LogPath
    The directory where service logs will be written. If not specified, uses %AppData%\FreeWinBackup

.PARAMETER DisplayName
    The display name for the service. Default is "FreeWinBackup Service"

.PARAMETER Description
    The service description. Default is "Automated backup scheduler and execution service"

.EXAMPLE
    .\Install-FreeWinBackupService.ps1
    Installs the service with default settings

.EXAMPLE
    .\Install-FreeWinBackupService.ps1 -BinaryPath "C:\Program Files\FreeWinBackup\FreeWinBackup.ServiceHost.exe"
    Installs the service from a custom location

.EXAMPLE
    .\Install-FreeWinBackupService.ps1 -ServiceName "MyBackupService" -LogPath "C:\Logs\FreeWinBackup"
    Installs with custom service name and log path

.NOTES
    This script requires Administrator privileges to install Windows services.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FreeWinBackup",
    
    [Parameter(Mandatory=$false)]
    [string]$BinaryPath,
    
    [Parameter(Mandatory=$false)]
    [string]$LogPath,
    
    [Parameter(Mandatory=$false)]
    [string]$DisplayName = "FreeWinBackup Service",
    
    [Parameter(Mandatory=$false)]
    [string]$Description = "Automated backup scheduler and execution service for FreeWinBackup"
)

# Ensure running as Administrator
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error "This script must be run as Administrator. Please run PowerShell as Administrator and try again."
    exit 1
}

Write-Host "Installing FreeWinBackup Windows Service..." -ForegroundColor Green

# Determine binary path if not specified
if ([string]::IsNullOrWhiteSpace($BinaryPath)) {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    $possiblePaths = @(
        Join-Path $scriptDir "..\FreeWinBackup.ServiceHost\bin\Release\FreeWinBackup.ServiceHost.exe",
        Join-Path $scriptDir "..\FreeWinBackup.ServiceHost\bin\Debug\FreeWinBackup.ServiceHost.exe"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $BinaryPath = Resolve-Path $path
            break
        }
    }
    
    if ([string]::IsNullOrWhiteSpace($BinaryPath)) {
        Write-Error "Could not find FreeWinBackup.ServiceHost.exe. Please build the solution or specify -BinaryPath"
        exit 1
    }
}

# Verify binary exists
if (-not (Test-Path $BinaryPath)) {
    Write-Error "Service executable not found at: $BinaryPath"
    exit 1
}

Write-Host "Using binary: $BinaryPath" -ForegroundColor Cyan

# Create log directory if specified
if (-not [string]::IsNullOrWhiteSpace($LogPath)) {
    if (-not (Test-Path $LogPath)) {
        Write-Host "Creating log directory: $LogPath" -ForegroundColor Cyan
        New-Item -Path $LogPath -ItemType Directory -Force | Out-Null
    }
    
    # Update config file if it exists
    $configPath = "$BinaryPath.config"
    if (Test-Path $configPath) {
        Write-Host "Updating configuration with log path..." -ForegroundColor Cyan
        [xml]$config = Get-Content $configPath
        $logDirSetting = $config.configuration.appSettings.add | Where-Object { $_.key -eq "LogDirectory" }
        if ($logDirSetting) {
            $logDirSetting.value = $LogPath
            $config.Save($configPath)
        }
    }
}

# Check if service already exists
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Warning "Service '$ServiceName' already exists. Stopping and removing it first..."
    
    if ($existingService.Status -eq 'Running') {
        Stop-Service -Name $ServiceName -Force
        Start-Sleep -Seconds 2
    }
    
    # Use sc.exe to delete the service
    $result = sc.exe delete $ServiceName
    Start-Sleep -Seconds 2
    
    Write-Host "Existing service removed." -ForegroundColor Yellow
}

# Create the service
Write-Host "Creating Windows Service..." -ForegroundColor Cyan
try {
    New-Service -Name $ServiceName `
                -BinaryPathName $BinaryPath `
                -DisplayName $DisplayName `
                -Description $Description `
                -StartupType Automatic `
                -ErrorAction Stop
    
    Write-Host "Service created successfully." -ForegroundColor Green
}
catch {
    Write-Error "Failed to create service: $_"
    exit 1
}

# Configure service recovery options (restart on failure)
Write-Host "Configuring service recovery options..." -ForegroundColor Cyan
$result = sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000
if ($LASTEXITCODE -eq 0) {
    Write-Host "Service recovery configured: Will restart after 60 seconds on failure." -ForegroundColor Green
}
else {
    Write-Warning "Failed to configure service recovery options. Service will not auto-restart on failure."
}

# Start the service
Write-Host "Starting service..." -ForegroundColor Cyan
try {
    Start-Service -Name $ServiceName -ErrorAction Stop
    Write-Host "Service started successfully!" -ForegroundColor Green
}
catch {
    Write-Warning "Service created but failed to start: $_"
    Write-Host "You can start it manually using: Start-Service -Name $ServiceName" -ForegroundColor Yellow
}

# Display service status
Write-Host "`nService Status:" -ForegroundColor Cyan
Get-Service -Name $ServiceName | Format-Table -AutoSize

Write-Host "`nInstallation complete!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Configure backup schedules using the FreeWinBackup WPF application"
Write-Host "2. Or manually edit: %AppData%\FreeWinBackup\settings.json"
Write-Host "3. Monitor logs in: %AppData%\FreeWinBackup\logs.json"
Write-Host "4. Check Windows Event Viewer > Application for service events"

Write-Host "`nUseful commands:" -ForegroundColor Cyan
Write-Host "  Start service:   Start-Service -Name $ServiceName"
Write-Host "  Stop service:    Stop-Service -Name $ServiceName"
Write-Host "  Service status:  Get-Service -Name $ServiceName"
Write-Host "  View logs:       Get-EventLog -LogName Application -Source FreeWinBackup -Newest 10"
