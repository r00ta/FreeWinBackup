# FreeWinBackup Installer

This directory contains the WiX Toolset installer project for FreeWinBackup.

## Prerequisites

To build the installer, you need:

1. **WiX Toolset v3.11 or later**
   - Download from: https://wixtoolset.org/releases/
   - Install the WiX Toolset build tools
   - Add WiX bin directory to PATH (usually `C:\Program Files (x86)\WiX Toolset v3.11\bin`)

2. **.NET Framework 4.8 Developer Pack**
   - Download from: https://dotnet.microsoft.com/download/dotnet-framework/net48
   - Required to build the main application

3. **MSBuild** (included with Visual Studio 2019 or later)
   - Alternatively, install Build Tools for Visual Studio

## Building the Installer

### Option 1: Using Visual Studio

1. Open `FreeWinBackup.sln` in the root directory
2. Set the solution configuration to **Release**
3. Right-click on the `FreeWinBackup.Installer` project and select **Build**
4. The MSI will be created in `Installer\bin\Release\FreeWinBackup.msi`

### Option 2: Using Command Line (MSBuild)

```powershell
# From the repository root directory

# Build the main application first
msbuild FreeWinBackup.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build the installer
msbuild Installer\FreeWinBackup.Installer.wixproj /p:Configuration=Release
```

The installer MSI will be created at: `Installer\bin\Release\FreeWinBackup.msi`

### Option 3: Using the Build Script

```powershell
# From the Installer directory
.\build-installer.ps1
```

## Installation Options

The installer supports several command-line properties:

### Basic Installation

```powershell
# Silent installation
msiexec /i FreeWinBackup.msi /qn

# Interactive installation
msiexec /i FreeWinBackup.msi
```

### Installation with Auto-Start

Enable automatic startup at user logon:

```powershell
msiexec /i FreeWinBackup.msi /qn AUTOSTART=1
```

This adds a registry entry to `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` that launches FreeWinBackup minimized when the user logs in.

### Installation with Desktop Shortcut

Add a desktop shortcut:

```powershell
msiexec /i FreeWinBackup.msi /qn ADDDESKTOPSHORTCUT=1
```

### Combined Options

```powershell
msiexec /i FreeWinBackup.msi /qn AUTOSTART=1 ADDDESKTOPSHORTCUT=1
```

## Uninstallation

### Using Control Panel

1. Open **Control Panel** > **Programs and Features**
2. Select **FreeWinBackup** from the list
3. Click **Uninstall**

### Using Command Line

```powershell
# Find the product code
wmic product where "name='FreeWinBackup'" get IdentifyingNumber

# Uninstall using the product code
msiexec /x {PRODUCT-CODE-GUID} /qn

# Or uninstall using the MSI file
msiexec /x FreeWinBackup.msi /qn
```

## Versioning

The installer version is defined in `Product.wxs`:

```xml
<?define Version="1.0.0.0" ?>
```

**Important:** When releasing a new version:

1. Update the version number in `Product.wxs`
2. Update the version in `AssemblyInfo.cs` files to match
3. Do NOT change the `UpgradeCode` GUID (this ensures upgrade functionality works)
4. The `Product Id` is set to `*` which generates a new GUID for each build

## Code Signing (Optional)

To sign the MSI for distribution:

1. **Obtain a code signing certificate**
   - From a Certificate Authority (CA) like DigiCert, Sectigo, etc.
   - Or create a self-signed certificate for internal use

2. **Sign the MSI using SignTool**

```powershell
# Using a PFX file
signtool sign /f "path\to\certificate.pfx" /p "password" /t http://timestamp.digicert.com "FreeWinBackup.msi"

# Using a certificate from the certificate store
signtool sign /n "Certificate Subject Name" /t http://timestamp.digicert.com "FreeWinBackup.msi"
```

3. **Verify the signature**

```powershell
signtool verify /pa "FreeWinBackup.msi"
```

## Installation Directory

By default, the application is installed to:
```
%LocalAppData%\FreeWinBackup
```

This is a per-user installation that doesn't require administrator privileges.

## Installed Files

The installer deploys:
- `FreeWinBackup.exe` - Main application executable
- `FreeWinBackup.Core.dll` - Core library
- `FreeWinBackup.exe.config` - Application configuration
- `Newtonsoft.Json.dll` - JSON serialization library
- Start Menu shortcut in the FreeWinBackup folder
- Optional desktop shortcut (if ADDDESKTOPSHORTCUT=1)
- Optional auto-start registry entry (if AUTOSTART=1)

## Troubleshooting

### WiX Toolset Not Found

**Error:** "The WiX Toolset v3.11 build tools must be installed"

**Solution:** 
- Install WiX Toolset from https://wixtoolset.org/releases/
- Ensure WiX is added to your PATH
- Restart Visual Studio or command prompt after installation

### Build Failed - Missing Source Files

**Error:** "Cannot find file specified in Source attribute"

**Solution:**
- Build the main FreeWinBackup application in Release mode first
- Ensure output files exist in `FreeWinBackup\bin\Release\`
- Check that file names in `Product.wxs` match actual output files

### MSI Install Error 2503/2502

**Error:** "Error 2503/2502 - Called RunScript when not marked in progress"

**Solution:**
- Run the installation with administrator privileges
- Or use: `msiexec /i FreeWinBackup.msi /qn` (silent mode often bypasses this)

### Changes Not Reflected After Upgrade

**Issue:** New version doesn't replace old files

**Solution:**
- Ensure version number was incremented in `Product.wxs`
- Verify UpgradeCode remains unchanged
- Uninstall old version first if upgrade detection fails

## Testing the Installer

Use the automated test script:

```powershell
.\test-install.ps1
```

This script:
1. Builds the solution and installer
2. Installs the MSI with AUTOSTART=1
3. Verifies the registry Run key exists
4. Checks that files are installed correctly
5. Provides uninstall instructions

## Additional Resources

- [WiX Toolset Documentation](https://wixtoolset.org/documentation/)
- [WiX Tutorial](https://www.firegiant.com/wix/tutorial/)
- [MSI Command Line Options](https://docs.microsoft.com/en-us/windows/win32/msi/command-line-options)
