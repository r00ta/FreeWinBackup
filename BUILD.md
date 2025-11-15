# Build Instructions for FreeWinBackup

## Prerequisites

- **Windows Operating System**: Windows 7 or later / Windows Server 2008 R2 or later
- **Visual Studio**: Visual Studio 2019 or later (Community, Professional, or Enterprise)
- **.NET Framework 4.8 SDK**: Included with Visual Studio or downloadable from Microsoft
- **NuGet Package Manager**: Included with Visual Studio

## Building from Source

### Option 1: Using Visual Studio (Recommended)

1. **Clone or Download the Repository**
   ```
   git clone https://github.com/r00ta/FreeWinBackup.git
   cd FreeWinBackup
   ```

2. **Open the Solution**
   - Double-click `FreeWinBackup.sln` or
   - Open Visual Studio and use File > Open > Project/Solution

3. **Restore NuGet Packages**
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"
   - Or: Tools > NuGet Package Manager > Restore NuGet Packages

4. **Build the Solution**
   - Select Build > Build Solution (or press F6)
   - Choose "Release" configuration for production builds
   - Choose "Debug" configuration for development

5. **Run the Application**
   - Press F5 to run with debugging
   - Press Ctrl+F5 to run without debugging
   - Or find the executable in `FreeWinBackup\bin\Debug\FreeWinBackup.exe` or `FreeWinBackup\bin\Release\FreeWinBackup.exe`

### Option 2: Using MSBuild (Command Line)

1. **Open Developer Command Prompt for Visual Studio**
   - Start Menu > Visual Studio 2019 > Developer Command Prompt for VS 2019

2. **Navigate to the Solution Directory**
   ```
   cd path\to\FreeWinBackup
   ```

3. **Restore NuGet Packages**
   ```
   nuget restore FreeWinBackup.sln
   ```

4. **Build the Solution**
   ```
   msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"
   ```

5. **Find the Executable**
   - Output will be in `FreeWinBackup\bin\Release\FreeWinBackup.exe`

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

## Troubleshooting

### Build Errors

**"Could not load file or assembly 'Newtonsoft.Json'"**
- Solution: Restore NuGet packages (right-click solution > Restore NuGet Packages)

**".NET Framework 4.8 is not installed"**
- Solution: Install .NET Framework 4.8 from Microsoft's website

**"Project targets .NET Framework 4.8"**
- Solution: Ensure .NET Framework 4.8 Developer Pack is installed

### Runtime Errors

**"Access Denied" when controlling services**
- Solution: Run the application as Administrator

**"Path not found" errors**
- Solution: Ensure source and destination folders exist and are accessible

**Services not stopping/starting**
- Solution: Verify service names are correct and you have Administrator privileges

## Deployment

### Creating a Standalone Package

1. Build in Release mode
2. Copy the following files from `FreeWinBackup\bin\Release\`:
   - `FreeWinBackup.exe`
   - `FreeWinBackup.exe.config`
   - `Newtonsoft.Json.dll`
   - Any other DLL files

3. Create a folder structure:
   ```
   FreeWinBackup\
   ├── FreeWinBackup.exe
   ├── FreeWinBackup.exe.config
   └── Newtonsoft.Json.dll
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
