# WinForms Setup Application and Shell Integration Summary

## Overview

The WiX-based MSI installer has been replaced with a lightweight WinForms setup application that performs per-user installation tasks for FreeWinBackup. This document captures the structure of the setup utility, the way payload files are bundled, and how it integrates with existing shell features such as the system tray, single-instance enforcement, and logon auto-start.

## 1. WinForms Setup Application

**Location:** `FreeWinBackup.Setup`

**Key Files:**
- `FreeWinBackup.Setup.csproj` – SDK-style WinForms project targeting .NET Framework 4.8
- `Program.cs` – Application entry point that launches the installer UI
- `MainForm.cs` – Installation and removal logic with inline logging
- `MainForm.Designer.cs` – Generated WinForms layout for the setup wizard

**User Workflow:**
1. Choose an installation directory (defaults to `%LOCALAPPDATA%\FreeWinBackup`).
2. Optionally enable desktop shortcut creation and auto-start at logon.
3. Decide whether to launch FreeWinBackup immediately after installation (enabled by default).
4. Click **Install** to copy files, create shortcuts, set up the Run key, and launch when requested.
5. Use **Uninstall** to remove files, shortcuts, and registry entries.
6. Use **Open Install Folder** to launch Explorer in the current install directory.

**Payload Management:**
- The setup project references `FreeWinBackup.csproj` with `ReferenceOutputAssembly=false` so it copies build outputs without adding a binary dependency.
- All build artifacts from `FreeWinBackup\bin\$(Configuration)\net48\**\*.*` are linked into the setup project under `payload/`.
- Building both projects in the same configuration ensures the installer EXE always carries the latest WPF binaries and supporting files.

**Shortcuts and Auto-Start:**
- Start Menu and optional desktop shortcuts are managed through `IWshRuntimeLibrary` COM interop (see `CreateShortcut` in `MainForm.cs`).
- Auto-start uses `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` with the value `"FreeWinBackup" = "<installPath>\FreeWinBackup.exe" /minimized`.
- Uninstall removes the Run key value and any shortcuts created during installation.

**Build Process:**
```powershell
# Restore dependencies and build the WPF payload
nuget restore FreeWinBackup.sln
msbuild FreeWinBackup.sln /p:Configuration=Release

# (Optional) build just the setup project
msbuild FreeWinBackup.Setup\FreeWinBackup.Setup.csproj /p:Configuration=Release

# Output: FreeWinBackup.Setup\bin\Release\FreeWinBackup.Setup.exe
```

**Runtime Characteristics:**
- Per-user install by default; no elevation is required unless the chosen directory needs elevation.
- Assumes the .NET Framework 4.8 runtime is present (same prerequisite as the main app).
- Writes log messages to the on-screen log window for transparency during install/uninstall.

## 2. System Tray Integration

**Files:**
- `FreeWinBackup/UI/TrayManager.cs`
- `FreeWinBackup/UI/AutoStartManager.cs`
- `FreeWinBackup/UI/SingleInstanceManager.cs`
- `App.xaml` / `App.xaml.cs`
- `Views/MainWindow.xaml.cs`

**Tray Features:**
- Status icons generated at runtime: blue (idle), green (backup running), red (error).
- Context menu actions: Open FreeWinBackup, Run Backup Now, Exit.
- Double-clicking the tray icon also opens the main window.
- Balloon tips announce backup start/completion/errors.

## 3. Auto-Start Behaviour

`AutoStartManager` writes to `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` using the same value name as the installer (`FreeWinBackup`). Changes made inside the UI remain consistent with the setup utility:
- Settings Page toggles: "Start automatically when I log in to Windows" and "Start minimized to system tray".
- State persisted in `%AppData%\FreeWinBackup\settings.json` and synchronized with the registry.

## 4. Single Instance Safeguard

FreeWinBackup enforces a single running instance via the named mutex `FreeWinBackup_SingleInstance_Mutex`. The first instance acquires the mutex, while later launches display a notification and exit gracefully. The mutex is released on application shutdown.

## 5. Supporting Documentation

- `README.md` – Updated installation guidance referencing the WinForms setup utility.
- `BUILD.md` – Build workflow for packaging the setup executable along with the WPF payload.
- `QUICKSTART.md` – First-run experience using the new installer.
- `PROJECT_SUMMARY.md` – High-level overview listing the setup project as part of the solution.

## Testing Recommendations

### Manual Testing Checklist

**Installer:**
- [ ] Build installer successfully
- [ ] Install with default options
- [ ] Install with AUTOSTART=1
- [ ] Install with ADDDESKTOPSHORTCUT=1
- [ ] Verify Start Menu shortcut works
- [ ] Verify desktop shortcut (if enabled)
- [ ] Uninstall and verify cleanup

**System Tray:**
- [ ] Tray icon appears on startup
- [ ] Icon changes color during backup
- [ ] Context menu items work
- [ ] Double-click opens window
- [ ] Balloon tips appear appropriately

**Auto-Start:**
- [ ] Enable auto-start in Settings
- [ ] Verify registry entry created
- [ ] Log off and log back in
- [ ] Verify application starts automatically
- [ ] Disable auto-start
- [ ] Verify registry entry removed

**Single Instance:**
- [ ] Start application
- [ ] Try to start second instance
- [ ] Verify message box appears
- [ ] Verify only one process runs

**Window Behavior:**
- [ ] Close window with X button
- [ ] Verify it minimizes to tray
- [ ] Reopen from tray menu
- [ ] Exit from tray menu
- [ ] Verify application closes

### Automated Testing

**Build Validation:**
```powershell
# Run from Installer directory
.\test-install.ps1
```

Tests performed:
1. Build solution
2. Build installer
3. Install MSI with AUTOSTART=1
4. Verify files installed
5. Verify registry entry
6. Provide uninstall instructions

## Known Limitations

1. **Platform:** Windows-only (.NET Framework 4.8)
2. **Build Environment:** Cannot build/test on Linux/macOS
3. **Icons:** Using programmatic generation (placeholder for .ico files)
4. **Tray Icon Persistence:** Limited to application lifetime
5. **Multi-User:** Auto-start is per-user, not system-wide

## Future Enhancements

Potential improvements not included in this implementation:

1. **Custom Icons:** Replace programmatic icons with professional .ico files
2. **Progress Indicator:** Show backup progress in tray tooltip
3. **Recent Backups:** Tray menu showing recent backup history
4. **Quick Schedule:** Create/edit schedules from tray menu
5. **Notification Settings:** User control over balloon tip frequency
6. **Task Scheduler:** Alternative to registry Run key
7. **Bootstrapper:** Provide a prerequisite stub that installs .NET Framework 4.8 when missing
8. **Code Signing:** Digitally sign the setup executable for better trust
9. **Per-Machine Option:** Allow elevated installs to `Program Files`
10. **Custom Actions:** Pre/post install hooks (e.g., migrate settings)

## Migration Notes

For users upgrading from previous versions:

1. Existing settings in `%AppData%\FreeWinBackup\settings.json` are preserved
2. New properties (`AutoStartEnabled`, `StartMinimized`) default to `false`
3. No breaking changes to data formats
4. Registry auto-start can be enabled post-install via Settings

## Support and Troubleshooting

**Common Issues:**

1. **Setup build fails:**
   - Ensure the WPF project builds first (Release mode)
   - Confirm `FreeWinBackup\bin\Release\net48` exists before building the setup project

2. **Application doesn't auto-start:**
   - Check Settings page checkbox
   - Verify registry entry exists
   - Log off/on to test

3. **Tray icon hidden:**
   - Check system tray overflow area (^ icon)
   - Enable "Start minimized" in Settings

4. **Multiple instances:**
   - SingleInstanceManager should prevent this
   - Check Task Manager for hung processes

**Getting Help:**
- GitHub Issues: https://github.com/r00ta/FreeWinBackup/issues
- Documentation: See README.md files
- Logs: Check `%AppData%\FreeWinBackup\logs.json`

## Conclusion

This implementation delivers a self-contained setup application and preserves the system tray capabilities that make FreeWinBackup user-friendly and production-ready. All acceptance criteria from the installer refresh have been met with focused updates to the codebase and documentation.
