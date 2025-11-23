# FreeWinBackup

![Build Status](https://github.com/r00ta/FreeWinBackup/actions/workflows/build.yml/badge.svg)

A free, open-source Windows backup scheduler application built with WPF and .NET Framework 4.8. Designed for Windows Server 2008 R2 and later versions.

**Available in two modes:**
- **Interactive Mode**: WPF desktop application for configuration and management
- **Service Mode**: Windows Service for automated background backups

## Features

- **System Tray Integration**: Minimized operation with status indicator and quick access menu
- **Auto-Start Support**: Optional automatic launch at Windows logon
- **Windows Service Mode**: Run backups automatically in the background without user login
- **Versioned Backups**: Each backup run creates an independent backup set in a timestamped subfolder
- **Flexible Scheduling**: Create backup schedules with daily, weekly, or monthly frequencies
- **Service Control**: Automatically stop and start Windows services before and after backups
- **Folder Backup**: Full folder copy with recursive subdirectory support
- **Retention Policy**: Automatically delete old backup sets based on configurable retention days
- **Comprehensive Logging**: Track all backup operations with detailed logs
- **MVVM Architecture**: Clean separation of concerns for maintainability
- **JSON/XML Storage**: Configure using JSON or XML for easy backup and portability
- **User-Friendly UI**: Intuitive WPF interface for managing schedules, viewing logs, and configuring settings
- **MSI Installer**: Easy installation with WiX Toolset-based installer

## System Requirements

- Windows Server 2008 R2 or later / Windows 7 or later
- .NET Framework 4.8
- Administrator privileges (for service control and folder access)

## Installation

### Option 1: Using the MSI Installer (Recommended)

1. Download the latest `FreeWinBackup.msi` from the [Releases](../../releases) page
2. Run the installer (double-click or use command line)
3. Follow the installation wizard

**Installation Options:**
- **Auto-Start**: Enable automatic launch at user logon
- **Desktop Shortcut**: Create a shortcut on the desktop

**Command-Line Installation:**
```powershell
# Basic installation
msiexec /i FreeWinBackup.msi

# Silent installation with auto-start
msiexec /i FreeWinBackup.msi /qn AUTOSTART=1

# Installation with desktop shortcut
msiexec /i FreeWinBackup.msi /qn ADDDESKTOPSHORTCUT=1
```

**Uninstallation:**
```powershell
# Via Control Panel: Programs and Features > FreeWinBackup > Uninstall
# Or via command line:
msiexec /x FreeWinBackup.msi /qn
```

For detailed installer documentation, see [Installer/README.md](Installer/README.md).

### Option 2: Manual Installation

1. Build the solution in Visual Studio (Release configuration)
2. Copy files from `FreeWinBackup\bin\Release\` to your preferred location
3. Run `FreeWinBackup.exe`

## Building the Application

> **📖 For comprehensive build instructions, troubleshooting, and CI/CD information, see [BUILD.md](BUILD.md)**

### Quick Build

```powershell
# Clone and build
git clone https://github.com/r00ta/FreeWinBackup.git
cd FreeWinBackup
nuget restore FreeWinBackup.sln
msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"
```

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.8 SDK
- WiX Toolset v3.11 or later (optional, for building the installer)

### Visual Studio Build

1. Open `FreeWinBackup.sln` in Visual Studio
2. Restore NuGet packages (automatic)
3. Build the solution (F6)
4. Run the application (F5)

For detailed build instructions, troubleshooting, and CI/CD information, see **[BUILD.md](BUILD.md)**.

### Building the Installer

See [Installer/README.md](Installer/README.md) for detailed instructions on building the MSI installer.

## Usage

### System Tray Icon

FreeWinBackup runs with a system tray icon that provides quick access and status information:

- **Blue Icon**: Idle - No backup currently running
- **Green Icon**: Active - Backup in progress
- **Red Icon**: Error - Last backup encountered an error

**Tray Icon Menu:**
- **Open FreeWinBackup**: Show the main application window
- **Run Backup Now**: Manually trigger a backup operation
- **Exit**: Close the application

**Tips:**
- Double-click the tray icon to open the main window
- Right-click for the context menu
- The application can start minimized to the tray (see Settings)

### Auto-Start Configuration

Configure the application to start automatically when you log in to Windows:

1. Navigate to the **Settings** page
2. Check **"Start automatically when I log in to Windows"**
3. Optionally, check **"Start minimized to system tray"** to have it run quietly in the background
4. Click **Save Settings**

When auto-start is enabled, FreeWinBackup adds a registry entry to launch at login. This can be configured during installation or changed later in Settings.

### Creating a Backup Schedule

1. Navigate to the **Schedules** page
2. Click **New** to create a new backup schedule
3. Configure the schedule:
   - Enter a name for the schedule
   - Select source and destination folders
   - Choose frequency (Daily, Weekly, or Monthly)
   - Set the run time
   - Optionally, specify Windows services to stop/start
   - Optionally, enable retention policy and set retention days
4. Click **Save** to save the schedule

### Versioned Backups

FreeWinBackup creates independent backup sets for each backup run:

- **Timestamped Folders**: Each backup is stored in a subfolder with the naming pattern `backup_YYYYMMDD_HHmmss` (e.g., `backup_20231115_143025`)
- **Non-Destructive**: Previous backups are preserved; each run creates a new complete backup set
- **Easy Recovery**: Navigate to any timestamped folder to access a complete backup from that point in time
- **Works with Retention**: The retention policy automatically removes entire old backup sets

**Example Structure**:
```
DestinationFolder\
├── backup_20231113_020000\
│   ├── file1.txt
│   └── subfolder\
├── backup_20231114_020000\
│   ├── file1.txt
│   └── subfolder\
└── backup_20231115_020000\
    ├── file1.txt
    └── subfolder\
```

### Retention Policy

The retention policy feature automatically deletes old backup sets to save disk space:

- **Enable/Disable**: Toggle retention policy per schedule
- **Retention Days**: Specify how many days to keep backup sets (backup sets older than this will be deleted)
- **Automatic Cleanup**: After each backup completes, old backup sets are automatically removed
- **Complete Set Deletion**: Entire backup folders are removed when they exceed the retention period
- **Logging**: All retention actions are logged for audit purposes

**Example**: If you set retention to 7 days, backup sets older than 7 days in the destination folder will be automatically deleted after each backup.

### Managing Schedules

- **Edit**: Select a schedule and click **Edit** to modify it
- **Delete**: Select a schedule and click **Delete** to remove it
- **Toggle Enabled**: Enable or disable a schedule without deleting it
- **Run Now**: Execute a backup immediately without waiting for the scheduled time

### Viewing Logs

Navigate to the **Logs** page to view a history of all backup operations, including:
- Timestamp
- Schedule name
- Status (Success/Error)
- Files copied
- Duration
- Error messages (if any)

### Settings

Configure application settings on the **Settings** page:

**Application Settings:**
- **Start automatically when I log in to Windows**: Enables/disables auto-start at login
- **Start minimized to system tray**: When enabled, application starts minimized to tray instead of showing the main window

**General Settings:**
- Log retention period
- Log file path

**Email Notifications:**
- Email notification settings (future enhancement)
- SMTP server configuration
- Email recipient address

## Windows Service Mode

FreeWinBackup can run as a Windows Service for automated background backups without requiring user login.

### Quick Start

1. **Build the solution** in Release mode
2. **Install the service** using PowerShell (as Administrator):
   ```powershell
   cd scripts
   .\Install-FreeWinBackupService.ps1
   ```
3. **Configure schedules** using the WPF application (or edit `%AppData%\FreeWinBackup\settings.json`)
4. **Start the service**:
   ```powershell
   Start-Service FreeWinBackup
   ```

### Service Features

- **Automatic Startup**: Service starts on system boot
- **Background Operation**: Runs without user login
- **Auto-Recovery**: Configured to restart automatically on failure
- **Event Log Integration**: Logs critical events to Windows Event Log
- **Same Configuration**: Uses the same settings.json as the WPF app

### Management

```powershell
# Check service status
Get-Service FreeWinBackup

# Start/Stop/Restart
Start-Service FreeWinBackup
Stop-Service FreeWinBackup
Restart-Service FreeWinBackup

# View recent service events
Get-EventLog -LogName Application -Source FreeWinBackup -Newest 10

# Uninstall
.\scripts\Uninstall-FreeWinBackupService.ps1
```

For detailed documentation on Service Mode, see **[SERVICE_MODE.md](SERVICE_MODE.md)** including:
- Installation and configuration
- Running under service accounts
- Troubleshooting
- Best practices
- Example configurations

## Architecture

The solution consists of three projects:

- **FreeWinBackup.Core**: Shared class library with models and services
- **FreeWinBackup**: WPF desktop application for interactive management
- **FreeWinBackup.ServiceHost**: Windows Service executable for background operation

The application follows the MVVM (Model-View-ViewModel) pattern:

- **Models**: Data structures for schedules, logs, and settings
- **ViewModels**: Business logic and data binding
- **Views**: XAML-based user interface
- **Services**: Core functionality including:
  - `BackupService`: Handles folder copying
  - `SchedulerService`: Manages schedule execution
  - `ServiceControlService`: Controls Windows services
  - `LoggingService`: Manages application logs
  - `JsonStorageService` / `XmlStorageService`: Configuration persistence

## Data Storage

Configuration files are stored in:
```
%AppData%\FreeWinBackup\
├── settings.json   (or settings.xml)
└── logs.json
```

## License

Licensed under the Apache License 2.0. See [LICENSE](LICENSE) file for details.

## Release Process

The project uses GitHub Actions to automate the release process. When a version tag is pushed, the workflow automatically builds the application and creates a GitHub Release with the compiled binaries.

### Creating a Release

1. **Tag the release**: Create an annotated tag with the version number (format: `vX.Y.Z`)
   ```bash
   git tag -a v0.1.0 -m "First release"
   ```

2. **Push the tag**: Push the tag to GitHub to trigger the release workflow
   ```bash
   git push origin v0.1.0
   ```

3. **Automated build**: The GitHub Actions workflow will:
   - Build the solution in Release configuration using MSBuild
   - Package all necessary files (executables, DLLs, configuration files)
   - Create a ZIP archive named `FreeWinBackup-vX.Y.Z.zip`
   - Publish a GitHub Release with the archive attached
   - Generate release notes from commit messages since the previous tag

4. **Download**: Once the workflow completes, the release will be available on the [Releases page](../../releases) with the packaged application ready to download.

### Release Artifacts

The release ZIP file contains:
- `FreeWinBackup.exe` - Main application executable
- Required DLL dependencies (including Newtonsoft.Json)
- Configuration files (App.config)
- Debug symbols (PDB files) for troubleshooting

### Version Numbering

Follow [Semantic Versioning](https://semver.org/):
- **Major version** (vX.0.0): Breaking changes or major feature additions
- **Minor version** (v0.X.0): New features, backward-compatible
- **Patch version** (v0.0.X): Bug fixes and minor improvements

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Security Considerations

- The application requires administrator privileges to control Windows services
- Ensure proper file permissions on source and destination folders
- Keep backup destinations on separate physical drives for data safety
- Review service control settings carefully to avoid disrupting critical services
