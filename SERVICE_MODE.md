# FreeWinBackup Service Mode Documentation

## Overview

FreeWinBackup can run in two modes:

1. **Interactive Mode** - The WPF desktop application for configuring and managing backup schedules
2. **Service Mode** - A Windows Service that runs backups automatically in the background

This document explains the Service Mode architecture, installation, configuration, and troubleshooting.

## Architecture

### Components

The solution consists of three projects:

- **FreeWinBackup.Core** - Shared class library containing:
  - Models (BackupSchedule, LogEntry, etc.)
  - Services (BackupService, SchedulerService, LoggingService, RetentionService, etc.)
  - Storage (JsonStorageService, XmlStorageService)

- **FreeWinBackup** - WPF desktop application for:
  - Creating and editing backup schedules
  - Viewing logs
  - Manual backup execution
  - Configuration management

- **FreeWinBackup.ServiceHost** - Windows Service executable that:
  - Loads schedules from configuration
  - Runs backups automatically on schedule
  - Logs to files and Windows Event Log
  - Operates without user interaction

### How It Works

1. **Configuration Storage**: Both the WPF app and service use the same configuration file located at:
   ```
   %AppData%\FreeWinBackup\settings.json
   ```

2. **Service Startup**: When the Windows Service starts, it:
   - Initializes the logging service
   - Loads backup schedules from settings.json
   - Starts the scheduler service
   - Begins monitoring for scheduled backup times

3. **Scheduled Execution**: The scheduler checks every minute if any enabled schedule should run based on:
   - Current day/time
   - Schedule frequency (Daily/Weekly/Monthly)
   - Last run timestamp

4. **Backup Execution**: When a backup runs:
   - Creates a timestamped backup folder (backup_YYYYMMDD_HHmmss)
   - Optionally stops specified Windows services
   - Copies all files from source to destination
   - Optionally starts the stopped services
   - Applies retention policy (deletes old backups)
   - Logs results to JSON log file and Event Log

### Key Differences from Interactive Mode

| Feature | Interactive Mode | Service Mode |
|---------|-----------------|--------------|
| **User Interface** | WPF GUI | None (background) |
| **Requires Login** | Yes | No |
| **Runs After Reboot** | Only if auto-started | Always (if set to automatic) |
| **Configuration** | Via GUI | Via WPF app or manual JSON editing |
| **Log Viewing** | Built-in log viewer | JSON file or Event Viewer |
| **Manual Execution** | "Run Now" button | Not available (scheduled only) |

## Installation

### Prerequisites

- Windows Server 2008 R2 or later / Windows 7 or later
- .NET Framework 4.8
- Administrator privileges
- FreeWinBackup built in Release configuration

### Installation Steps

1. **Build the Solution**
   ```
   Open FreeWinBackup.sln in Visual Studio
   Select "Release" configuration
   Build > Build Solution
   ```

2. **Run Installation Script**
   
   Open PowerShell as Administrator and navigate to the scripts folder:
   ```powershell
   cd path\to\FreeWinBackup\scripts
   .\Install-FreeWinBackupService.ps1
   ```

   **Optional Parameters:**
   ```powershell
   # Custom service name
   .\Install-FreeWinBackupService.ps1 -ServiceName "MyBackupService"
   
   # Custom binary path
   .\Install-FreeWinBackupService.ps1 -BinaryPath "C:\Program Files\FreeWinBackup\FreeWinBackup.ServiceHost.exe"
   
   # Custom log directory
   .\Install-FreeWinBackupService.ps1 -LogPath "C:\Logs\FreeWinBackup"
   ```

3. **Verify Installation**
   ```powershell
   Get-Service FreeWinBackup
   ```

### Alternative: Manual Installation with InstallUtil

If you prefer to use InstallUtil.exe:

```cmd
cd FreeWinBackup.ServiceHost\bin\Release
%windir%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe FreeWinBackup.ServiceHost.exe
```

**Note**: The PowerShell installation script is recommended as it also configures service recovery options.

## Configuration

### Backup Schedules

The service reads schedules from `%AppData%\FreeWinBackup\settings.json`.

**Option 1: Use the WPF Application (Recommended)**

1. Run FreeWinBackup.exe as Administrator
2. Create and configure backup schedules
3. Save changes
4. The service will automatically use the new schedules (no restart required)

**Option 2: Manual JSON Editing**

Edit `%AppData%\FreeWinBackup\settings.json`:

```json
{
  "Schedules": [
    {
      "Id": "a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d",
      "Name": "Daily Database Backup",
      "SourceFolder": "C:\\DatabaseData",
      "DestinationFolder": "D:\\Backups\\Database",
      "Frequency": 0,
      "RunTime": "02:00:00",
      "IsEnabled": true,
      "ServicesToStop": ["MSSQLSERVER"],
      "DayOfWeek": null,
      "DayOfMonth": null,
      "EnableRetentionPolicy": true,
      "RetentionDays": 7,
      "LastRun": null,
      "CreatedDate": "2024-01-01T00:00:00"
    }
  ],
  "MaxLogDays": 30,
  "EnableEmailNotifications": false
}
```

**Frequency Values:**
- 0 = Daily
- 1 = Weekly (requires DayOfWeek)
- 2 = Monthly (requires DayOfMonth)

### Service Configuration

The service configuration file is located at:
```
FreeWinBackup.ServiceHost.exe.config
```

**Available Settings:**

```xml
<appSettings>
  <!-- Optional: Custom log directory. If empty, uses %AppData%\FreeWinBackup -->
  <add key="LogDirectory" value="C:\Logs\FreeWinBackup" />
</appSettings>
```

### Logging

The service logs to two locations:

1. **JSON Log File**: `%AppData%\FreeWinBackup\logs.json`
   - Detailed backup operation logs
   - Includes file counts, bytes copied, duration
   - Limited to last 1000 entries

2. **Windows Event Log**: Application log with source "FreeWinBackup"
   - Service start/stop events
   - Critical errors
   - View with Event Viewer or PowerShell:
     ```powershell
     Get-EventLog -LogName Application -Source FreeWinBackup -Newest 10
     ```

## Service Management

### Start/Stop the Service

```powershell
# Start
Start-Service FreeWinBackup

# Stop
Stop-Service FreeWinBackup

# Restart
Restart-Service FreeWinBackup

# Check status
Get-Service FreeWinBackup
```

### Service Recovery

The installation script configures the service to automatically restart on failure:
- First failure: Restart after 60 seconds
- Second failure: Restart after 60 seconds
- Subsequent failures: Restart after 60 seconds

To modify recovery options:
```cmd
sc failure FreeWinBackup reset= 86400 actions= restart/60000/restart/120000/restart/300000
```

### Running Under a Service Account

For production use, run the service under a dedicated service account with appropriate permissions:

1. Create a service account (e.g., DOMAIN\svc_backup)
2. Grant permissions:
   - Read access to source folders
   - Write access to destination folders
   - "Log on as a service" right
   - Permission to control specified Windows services (if using service control)

3. Change service account:
   ```powershell
   $credential = Get-Credential
   $service = Get-WmiObject -Class Win32_Service -Filter "Name='FreeWinBackup'"
   $service.Change($null,$null,$null,$null,$null,$null,$credential.UserName,$credential.GetNetworkCredential().Password)
   ```

   Or use Services console (services.msc):
   - Right-click service > Properties > Log On tab

## Upgrade Process

To upgrade the service to a new version:

1. Stop the service:
   ```powershell
   Stop-Service FreeWinBackup
   ```

2. Backup configuration (optional but recommended):
   ```powershell
   Copy-Item "$env:APPDATA\FreeWinBackup" "$env:APPDATA\FreeWinBackup.backup" -Recurse
   ```

3. Replace the service executable and DLLs:
   ```powershell
   # Copy new files to service installation directory
   Copy-Item "path\to\new\FreeWinBackup.ServiceHost.exe" -Destination "C:\ServicePath\" -Force
   Copy-Item "path\to\new\FreeWinBackup.Core.dll" -Destination "C:\ServicePath\" -Force
   Copy-Item "path\to\new\Newtonsoft.Json.dll" -Destination "C:\ServicePath\" -Force
   ```

4. Start the service:
   ```powershell
   Start-Service FreeWinBackup
   ```

## Uninstallation

### Using PowerShell Script

```powershell
cd path\to\FreeWinBackup\scripts
.\Uninstall-FreeWinBackupService.ps1
```

To also remove configuration and logs:
```powershell
.\Uninstall-FreeWinBackupService.ps1 -RemoveData
```

### Manual Uninstallation

```cmd
sc stop FreeWinBackup
sc delete FreeWinBackup
```

To remove data:
```powershell
Remove-Item "$env:APPDATA\FreeWinBackup" -Recurse -Force
```

## Troubleshooting

### Service Won't Start

**Check Event Log:**
```powershell
Get-EventLog -LogName Application -Source FreeWinBackup -Newest 5
```

**Common Causes:**
1. Missing .NET Framework 4.8
2. Missing dependencies (Newtonsoft.Json.dll)
3. Invalid configuration file
4. Insufficient permissions

**Solution:**
- Verify all DLLs are in the same directory as the executable
- Check that the service account has proper permissions
- Verify .NET Framework 4.8 is installed

### Backups Not Running

**Check:**
1. Service is running: `Get-Service FreeWinBackup`
2. Schedules are enabled in settings.json
3. Schedule times are correct
4. Source/destination paths are accessible

**View Logs:**
```powershell
# Open log file
notepad "$env:APPDATA\FreeWinBackup\logs.json"

# Or format as table
Get-Content "$env:APPDATA\FreeWinBackup\logs.json" | ConvertFrom-Json | 
    Select-Object Timestamp, ScheduleName, Message, Level | Format-Table -AutoSize
```

### Permission Errors

**Error:** "Access to path 'C:\...' is denied"

**Solution:**
1. Grant the service account read/write permissions to source/destination folders
2. For network shares, use domain account with appropriate access
3. For local system folders, ensure service account has necessary rights

### Service Control Failures

**Error:** "Failed to stop service 'ServiceName'"

**Solution:**
1. Verify service name is correct (case-sensitive)
2. Ensure service account has permission to control the specified service
3. Grant "Start/Stop/Pause" permissions using sc.exe:
   ```cmd
   sc sdset ServiceName D:(A;;RPWPCR;;;S-1-5-21-...)
   ```

### High Memory/CPU Usage

**Possible Causes:**
1. Very large backup operations
2. Too many concurrent backups
3. Retention policy scanning large directories

**Solutions:**
- Schedule backups during off-peak hours
- Reduce backup frequency
- Optimize retention policy (fewer backups to scan)
- Exclude unnecessary files/folders

### Debug Mode

To run the service in console mode for debugging:

1. Modify Program.cs temporarily to run interactively:
   ```csharp
   // Comment out service run, add console mode
   var service = new FreeWinBackupWindowsService();
   service.OnStart(args);
   Console.WriteLine("Press Enter to stop...");
   Console.ReadLine();
   service.OnStop();
   ```

2. Build and run from command line:
   ```cmd
   cd FreeWinBackup.ServiceHost\bin\Debug
   FreeWinBackup.ServiceHost.exe
   ```

## Limitations

1. **No GUI**: Service has no user interface. Use WPF app for configuration.

2. **Scheduled Only**: Cannot trigger manual backups from service (use WPF app "Run Now" button).

3. **No Interactive Prompts**: Service cannot display dialogs or request user input.

4. **Session 0 Isolation**: Runs in isolated session, cannot interact with user desktop.

5. **Path Resolution**: Avoid relative paths; always use absolute paths for source/destination.

6. **Network Drives**: May require service to run under domain account, not Local System.

## Best Practices

1. **Use Dedicated Service Account**: Don't run as Local System in production
2. **Test Schedules First**: Use WPF app to test backups before relying on service
3. **Monitor Logs Regularly**: Set up automated log monitoring
4. **Backup Configuration**: Keep backups of settings.json
5. **Separate Disks**: Backup to different physical drive than source
6. **Test Restores**: Periodically verify backup integrity by restoring
7. **Document Schedules**: Maintain documentation of what's being backed up
8. **Retention Policy**: Set appropriate retention based on disk space and compliance needs

## Support and Troubleshooting Resources

- **Log Files**: `%AppData%\FreeWinBackup\logs.json`
- **Event Viewer**: Application log, source "FreeWinBackup"
- **Configuration**: `%AppData%\FreeWinBackup\settings.json`
- **Service Properties**: `services.msc` → FreeWinBackup
- **GitHub Issues**: [Report issues](https://github.com/r00ta/FreeWinBackup/issues)

## Example Use Cases

### Case 1: Nightly Database Backup

```json
{
  "Name": "SQL Server Nightly Backup",
  "SourceFolder": "C:\\Program Files\\Microsoft SQL Server\\MSSQL15.MSSQLSERVER\\MSSQL\\DATA",
  "DestinationFolder": "D:\\Backups\\SQLServer",
  "Frequency": 0,
  "RunTime": "02:00:00",
  "ServicesToStop": ["MSSQLSERVER"],
  "EnableRetentionPolicy": true,
  "RetentionDays": 14
}
```

### Case 2: Weekly Full System Backup

```json
{
  "Name": "Weekly Full Backup",
  "SourceFolder": "C:\\ImportantData",
  "DestinationFolder": "\\\\NAS\\Backups\\ServerName",
  "Frequency": 1,
  "DayOfWeek": 0,
  "RunTime": "01:00:00",
  "EnableRetentionPolicy": true,
  "RetentionDays": 30
}
```

### Case 3: Monthly Archive

```json
{
  "Name": "Monthly Archive",
  "SourceFolder": "C:\\Archives",
  "DestinationFolder": "E:\\MonthlyBackups",
  "Frequency": 2,
  "DayOfMonth": 1,
  "RunTime": "00:00:00",
  "EnableRetentionPolicy": true,
  "RetentionDays": 365
}
```
