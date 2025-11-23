# FreeWinBackup Installer Quick Start

This guide helps you quickly install FreeWinBackup using the MSI installer.

## Download

Download the latest `FreeWinBackup.msi` installer from the [Releases](https://github.com/r00ta/FreeWinBackup/releases) page.

## Installation

### Basic Installation (Interactive)

1. Double-click `FreeWinBackup.msi`
2. Follow the installation wizard
3. Click **Install** to complete the setup
4. Launch FreeWinBackup from the Start Menu

### Advanced Installation (Command Line)

Open PowerShell or Command Prompt as Administrator and run:

```powershell
# Basic silent installation (auto-start enabled by default)
msiexec /i FreeWinBackup.msi /qn

# Install without auto-start
msiexec /i FreeWinBackup.msi /qn AUTOSTART=0

# Install with desktop shortcut
msiexec /i FreeWinBackup.msi /qn ADDDESKTOPSHORTCUT=1

# Install with both auto-start and desktop shortcut
msiexec /i FreeWinBackup.msi /qn AUTOSTART=1 ADDDESKTOPSHORTCUT=1
```

### Installation Options

| Property | Description | Default |
|----------|-------------|---------|
| `AUTOSTART=1` | Start automatically at Windows login | On |
| `ADDDESKTOPSHORTCUT=1` | Create desktop shortcut | Off |

## First Launch

After installation:

1. **Find the Application**:
   - Start Menu: Start → FreeWinBackup
   - Desktop: If you enabled desktop shortcut during install
   - System Tray: If auto-start is enabled, check the system tray (notification area)

2. **Configure Settings** (optional):
   - Navigate to the Settings page
   - Disable "Start automatically when I log in to Windows" if you prefer to launch manually
   - Enable "Start minimized to system tray" for quiet background operation

3. **Create Your First Backup**:
   - Go to the Schedules page
   - Click "New" to create a backup schedule
   - Configure source folder, destination folder, and schedule
   - Click "Save"

## Auto-Start Behavior

Auto-start is enabled immediately after installation unless you pass `AUTOSTART=0`.

When **auto-start** is enabled:
- FreeWinBackup launches automatically when you log in to Windows
- A registry entry is created at `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`
- The application starts with the `/minimized` argument

When **start minimized** is also enabled:
- The application runs quietly in the system tray
- No main window appears on startup
- Look for the tray icon (blue circle) in the notification area

## System Tray Icon

The tray icon shows the application status:
- **Blue**: Idle - No backup running
- **Green**: Active - Backup in progress
- **Red**: Error - Last backup failed

**Right-click the tray icon** to:
- Open the main window
- Run a backup immediately
- Exit the application

**Double-click the tray icon** to open the main window.

## Uninstallation

### Using Windows Settings

1. Open **Settings** → **Apps** → **Apps & features**
2. Search for "FreeWinBackup"
3. Click on it and select **Uninstall**

### Using Control Panel

1. Open **Control Panel** → **Programs and Features**
2. Select **FreeWinBackup**
3. Click **Uninstall**

### Using Command Line

```powershell
# Uninstall silently
msiexec /x FreeWinBackup.msi /qn
```

The uninstaller will:
- Remove all program files
- Remove Start Menu shortcuts
- Remove desktop shortcut (if created)
- Remove auto-start registry entry (if enabled)
- Preserve your configuration and backup schedules in `%AppData%\FreeWinBackup`

## Troubleshooting

### Application doesn't start automatically

1. Check Settings → "Start automatically when I log in to Windows" is checked
2. Verify registry entry exists: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\FreeWinBackup`
3. Try logging off and logging back in

### Can't find the tray icon

1. The tray icon might be hidden in the overflow area (click the ^ arrow in the system tray)
2. Check if "Start minimized to system tray" is enabled in Settings
3. Restart the application

### Installation fails

1. Ensure you have .NET Framework 4.8 installed
2. Try running the installer as Administrator
3. Check Windows Event Viewer for error details
4. Uninstall any previous version first

### Backup doesn't run

1. Open the application from Start Menu or system tray
2. Go to Schedules page and verify your schedule is enabled
3. Check the Logs page for error messages
4. Ensure source and destination folders are accessible

## Getting Help

- **Documentation**: See [README.md](../README.md) for detailed usage instructions
- **Installer Documentation**: See [Installer/README.md](README.md) for build instructions
- **Issues**: Report issues on [GitHub Issues](https://github.com/r00ta/FreeWinBackup/issues)

## Next Steps

- Read the full [README.md](../README.md) for detailed feature documentation
- Learn about [Windows Service Mode](../SERVICE_MODE.md) for background operation
- Explore [Versioned Backups](../VERSIONED_BACKUPS.md) documentation
