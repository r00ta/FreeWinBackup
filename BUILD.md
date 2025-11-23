# Build Instructions for FreeWinBackup

## Quick Start

For the fastest way to get started building FreeWinBackup:

```powershell
# 1. Clone the repository
git clone https://github.com/r00ta/FreeWinBackup.git
cd FreeWinBackup

# 2. Open Visual Studio and open FreeWinBackup.sln
# 3. Press F5 to build and run

# Or use command line:
nuget restore FreeWinBackup.sln
msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"
```

## Prerequisites

- **Windows Operating System**: Windows 7 or later / Windows Server 2008 R2 or later
- **Visual Studio**: Visual Studio 2019 or later (Community, Professional, or Enterprise)
  - Download from: https://visualstudio.microsoft.com/downloads/
- **.NET Framework 4.8 SDK**: Included with Visual Studio or downloadable from Microsoft
  - Download standalone: https://dotnet.microsoft.com/download/dotnet-framework/net48
- **NuGet Package Manager**: Included with Visual Studio
- **WiX Toolset v3.11 or later** (optional, only for building the MSI installer)
  - Download from: https://wixtoolset.org/releases/

## Build Configurations

The solution supports two build configurations:
- **Debug**: Includes debugging symbols and assertions for development
- **Release**: Optimized for production use

## Building from Source

### Option 1: Using Visual Studio (Recommended)

1. **Clone or Download the Repository**
   ```cmd
   git clone https://github.com/r00ta/FreeWinBackup.git
   cd FreeWinBackup
   ```

2. **Open the Solution**
   - Double-click `FreeWinBackup.sln` or
   - Open Visual Studio and use File > Open > Project/Solution

3. **Restore NuGet Packages** (Automatic in VS 2017+)
   - Visual Studio will automatically restore packages on solution load
   - Manual restore: Right-click solution → "Restore NuGet Packages"
   - Or: Tools > NuGet Package Manager > Package Manager Console, then run:
     ```powershell
     Update-Package -reinstall
     ```

4. **Build the Solution**
   - Select Build > Build Solution (or press F6 or Ctrl+Shift+B)
   - Choose "Release" configuration for production builds
   - Choose "Debug" configuration for development
   - The build output will be in:
     - `FreeWinBackup\bin\Debug\` or `FreeWinBackup\bin\Release\`
     - `FreeWinBackup.Core\bin\Debug\` or `FreeWinBackup.Core\bin\Release\`
     - `FreeWinBackup.ServiceHost\bin\Debug\` or `FreeWinBackup.ServiceHost\bin\Release\`

5. **Run the Application**
   - Press F5 to run with debugging
   - Press Ctrl+F5 to run without debugging
   - Or find the executable in `FreeWinBackup\bin\Debug\FreeWinBackup.exe` or `FreeWinBackup\bin\Release\FreeWinBackup.exe`

**Expected Build Output:**
- ✅ FreeWinBackup.exe (WPF desktop application)
- ✅ FreeWinBackup.Core.dll (shared class library)
- ✅ FreeWinBackup.ServiceHost.exe (Windows Service)
- ✅ Newtonsoft.Json.dll (NuGet dependency)

### Option 2: Using MSBuild (Command Line)

1. **Open Developer Command Prompt for Visual Studio**
   - Start Menu > Visual Studio 2019/2022 > Developer Command Prompt
   - Or open PowerShell/CMD and run (adjust path for your VS version and edition):
     ```powershell
     # For VS 2019 Community
     & "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat"
     
     # For VS 2022 Community
     & "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
     
     # For VS 2019 Professional
     & "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsDevCmd.bat"
     ```

2. **Navigate to the Solution Directory**
   ```cmd
   cd path\to\FreeWinBackup
   ```

3. **Restore NuGet Packages**
   ```cmd
   nuget restore FreeWinBackup.sln
   ```
   
   If `nuget.exe` is not found, download it from https://www.nuget.org/downloads or use:
   ```cmd
   msbuild /t:restore FreeWinBackup.sln
   ```

4. **Build the Solution**
   ```cmd
   # Build all projects (Release configuration)
   msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"
   
   # Build Debug configuration
   msbuild FreeWinBackup.sln /p:Configuration=Debug /p:Platform="Any CPU"
   
   # Build with detailed output for diagnostics
   msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU" /v:detailed
   
   # Build specific project only
   msbuild FreeWinBackup.Core\FreeWinBackup.Core.csproj /p:Configuration=Release
   ```

5. **Build Output Locations**
   - WPF Application: `FreeWinBackup\bin\Release\FreeWinBackup.exe`
   - Core Library: `FreeWinBackup.Core\bin\Release\FreeWinBackup.Core.dll`
   - Service Host: `FreeWinBackup.ServiceHost\bin\Release\FreeWinBackup.ServiceHost.exe`

**Build Success Indicators:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Building the Windows Service

The solution includes a Windows Service host for automated background backups.

### Using Visual Studio

1. **Open the Solution**
   - Open `FreeWinBackup.sln`

2. **Select Release Configuration**
   - Configuration dropdown > Release

3. **Build the Solution**
   - Build > Build Solution (F6)
   - This builds all three projects:
     - FreeWinBackup.Core (shared library)
     - FreeWinBackup (WPF app)
     - FreeWinBackup.ServiceHost (Windows Service)

4. **Locate Service Executable**
   ```
   FreeWinBackup.ServiceHost\bin\Release\
   ├── FreeWinBackup.ServiceHost.exe
   ├── FreeWinBackup.ServiceHost.exe.config
   ├── FreeWinBackup.Core.dll
   └── Newtonsoft.Json.dll
   ```

### Using MSBuild

```cmd
cd path\to\FreeWinBackup
nuget restore FreeWinBackup.sln
msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"
```

### Building Individual Projects

To build only the service:
```cmd
msbuild FreeWinBackup.Core\FreeWinBackup.Core.csproj /p:Configuration=Release
msbuild FreeWinBackup.ServiceHost\FreeWinBackup.ServiceHost.csproj /p:Configuration=Release
```

## Running the Application

### First Run

When you first run the application:
1. The app will create configuration folders in `%AppData%\FreeWinBackup\`
2. A default configuration file will be created
3. You'll see the main window with the Schedules page

### Administrator Privileges

**Important**: To control Windows services, the application should be run with Administrator privileges:
1. Right-click on `FreeWinBackup.exe`
2. Select "Run as administrator"

Alternatively, you can configure the executable to always run as administrator:
1. Right-click on `FreeWinBackup.exe`
2. Select "Properties"
3. Go to "Compatibility" tab
4. Check "Run this program as an administrator"
5. Click "OK"

## Continuous Integration

The project includes a GitHub Actions workflow that automatically builds the application on every push and pull request.

### CI/CD Pipeline

The `.github/workflows/build.yml` workflow:
- ✅ Builds on Windows runners (required for .NET Framework 4.8)
- ✅ Tests both Debug and Release configurations
- ✅ Restores NuGet packages automatically
- ✅ Verifies all build outputs are created correctly
- ✅ Uploads build artifacts for Release builds
- ✅ Can optionally build the WiX installer if WiX Toolset is installed

### Viewing Build Status

Check the build status at: https://github.com/r00ta/FreeWinBackup/actions

Green checkmarks ✓ indicate successful builds. Click on any workflow run to view detailed logs.

## Dependencies

The project has minimal external dependencies:

| Package | Version | Purpose |
|---------|---------|---------|
| Newtonsoft.Json | 13.0.3 | JSON serialization for configuration files |
| .NET Framework | 4.8 | Runtime framework |

All dependencies are declared in `packages.config` files and are automatically restored by NuGet.

## Project Structure

```
FreeWinBackup/
├── FreeWinBackup.sln           # Visual Studio solution file
├── FreeWinBackup/              # Main WPF application project
│   ├── FreeWinBackup.csproj
│   ├── App.xaml / App.xaml.cs  # Application entry point
│   ├── ViewModels/             # MVVM view models
│   ├── Views/                  # XAML views and UI
│   └── UI/                     # UI helpers (tray, auto-start)
├── FreeWinBackup.Core/         # Shared class library
│   ├── FreeWinBackup.Core.csproj
│   ├── Models/                 # Data models
│   └── Services/               # Business logic services
├── FreeWinBackup.ServiceHost/  # Windows Service executable
│   ├── FreeWinBackup.ServiceHost.csproj
│   ├── Program.cs              # Service entry point
│   └── FreeWinBackupWindowsService.cs
└── Installer/                  # WiX installer project (optional)
    └── FreeWinBackup.Installer.wixproj
```

## Troubleshooting

### Build Errors

**"Could not load file or assembly 'Newtonsoft.Json'"**
- Solution: Restore NuGet packages
  ```cmd
  nuget restore FreeWinBackup.sln
  ```

**".NET Framework 4.8 is not installed"**
- Solution: Install .NET Framework 4.8 Developer Pack from https://dotnet.microsoft.com/download/dotnet-framework/net48

**"Project targets .NET Framework 4.8"**
- Solution: Ensure .NET Framework 4.8 Developer Pack is installed with Visual Studio or download separately

**"MSBuild is not recognized as an internal or external command"**
- Solution: Run from Developer Command Prompt or add MSBuild to PATH
  ```powershell
  # Add to PATH (replace with your VS version/edition):
  $env:PATH += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin"
  ```

**"NuGet restore failed"**
- Solution: Check internet connection and try:
  ```cmd
  # Clear NuGet cache
  nuget locals all -clear
  # Restore again
  nuget restore FreeWinBackup.sln -Force
  ```

### Runtime Errors

**"Access Denied" when controlling services**
- Solution: Run the application as Administrator
  - Right-click FreeWinBackup.exe → Run as administrator

**"Path not found" errors**
- Solution: Ensure source and destination folders exist and are accessible

**Services not stopping/starting**
- Solution: 
  1. Verify service names are correct
  2. Run application with Administrator privileges
  3. Check Windows Event Viewer for service errors

## Deployment

### Deploying the WPF Application

1. Build in Release mode
2. Copy the following files from `FreeWinBackup\bin\Release\`:
   - `FreeWinBackup.exe`
   - `FreeWinBackup.exe.config`
   - `FreeWinBackup.Core.dll`
   - `Newtonsoft.Json.dll`

3. Create a folder structure:
   ```
   FreeWinBackup\
   ├── FreeWinBackup.exe
   ├── FreeWinBackup.exe.config
   ├── FreeWinBackup.Core.dll
   └── Newtonsoft.Json.dll
   ```

### Deploying the Windows Service

1. Build in Release mode
2. Copy the following files from `FreeWinBackup.ServiceHost\bin\Release\`:
   - `FreeWinBackup.ServiceHost.exe`
   - `FreeWinBackup.ServiceHost.exe.config`
   - `FreeWinBackup.Core.dll`
   - `Newtonsoft.Json.dll`

3. Create a folder structure:
   ```
   FreeWinBackupService\
   ├── FreeWinBackup.ServiceHost.exe
   ├── FreeWinBackup.ServiceHost.exe.config
   ├── FreeWinBackup.Core.dll
   └── Newtonsoft.Json.dll
   ```

4. Install the service using PowerShell (as Administrator):
   ```powershell
   cd scripts
   .\Install-FreeWinBackupService.ps1 -BinaryPath "C:\Path\To\FreeWinBackupService\FreeWinBackup.ServiceHost.exe"
   ```

### Complete Deployment Package

For a complete deployment that includes both the WPF app and service:

```
FreeWinBackup-Package\
├── WPF\
│   ├── FreeWinBackup.exe
│   ├── FreeWinBackup.exe.config
│   ├── FreeWinBackup.Core.dll
│   └── Newtonsoft.Json.dll
├── Service\
│   ├── FreeWinBackup.ServiceHost.exe
│   ├── FreeWinBackup.ServiceHost.exe.config
│   ├── FreeWinBackup.Core.dll
│   └── Newtonsoft.Json.dll
└── scripts\
    ├── Install-FreeWinBackupService.ps1
    └── Uninstall-FreeWinBackupService.ps1
```

4. Distribute this folder or create an installer using tools like:
   - WiX Toolset
   - Inno Setup
   - Advanced Installer

### System Requirements for Deployment

- Windows 7 or later / Windows Server 2008 R2 or later
- .NET Framework 4.8 Runtime (can be included with installer)
- Administrator privileges for service control features

## Development

### Project Structure

```
FreeWinBackup/
├── Models/             # Data models
├── ViewModels/         # MVVM ViewModels
├── Views/              # XAML views
├── Services/           # Business logic services
├── Properties/         # Assembly info and resources
├── App.xaml           # Application definition
└── App.config         # Application configuration
```

### Adding New Features

1. Create model classes in `Models/` folder
2. Create service classes in `Services/` folder
3. Create ViewModel in `ViewModels/` folder
4. Create XAML view in `Views/` folder
5. Update navigation in `MainViewModel.cs` if adding a new page

### Code Style

- Follow C# naming conventions
- Use MVVM pattern for UI logic
- Keep services loosely coupled
- Add XML documentation comments for public APIs
- Handle exceptions appropriately

## License

This project is licensed under the Apache License 2.0. See LICENSE file for details.

## Quick Build Verification Checklist

After building, verify your build was successful:

- [ ] `FreeWinBackup\bin\Release\FreeWinBackup.exe` exists
- [ ] `FreeWinBackup.Core\bin\Release\FreeWinBackup.Core.dll` exists  
- [ ] `FreeWinBackup.ServiceHost\bin\Release\FreeWinBackup.ServiceHost.exe` exists
- [ ] `packages\Newtonsoft.Json.*\lib\net45\Newtonsoft.Json.dll` exists (or in bin folders)
- [ ] Build output shows "0 Error(s)"
- [ ] Application launches without errors (F5 or Ctrl+F5)

## Getting Help

If you encounter build issues:

1. Check the [Troubleshooting](#troubleshooting) section above
2. Review the [GitHub Actions build logs](https://github.com/r00ta/FreeWinBackup/actions) for reference
3. Open an issue on GitHub with:
   - Full error message
   - Build output
   - Visual Studio version
   - .NET Framework version installed
