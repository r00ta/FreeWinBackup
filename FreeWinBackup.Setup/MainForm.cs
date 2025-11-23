using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FreeWinBackup.Setup
{
    public partial class MainForm : Form
    {
        private const string ApplicationName = "FreeWinBackup";
        private const string ExecutableName = "FreeWinBackup.exe";
        private readonly string _defaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationName);
        private readonly string _payloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "payload");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtInstallPath.Text = _defaultInstallPath;
            chkAutoStart.Checked = true;
            chkDesktopShortcut.Checked = false;
            chkLaunchAfterInstall.Checked = true;

            if (!Directory.Exists(_payloadPath))
            {
                LogMessage("Payload directory not found. Build the FreeWinBackup project in Release mode before building the setup.");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose an installation folder";
                dialog.SelectedPath = Directory.Exists(txtInstallPath.Text) ? txtInstallPath.Text : _defaultInstallPath;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtInstallPath.Text = dialog.SelectedPath;
                }
            }
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            await RunActionAsync(InstallAsync);
        }

        private async void btnUninstall_Click(object sender, EventArgs e)
        {
            await RunActionAsync(UninstallAsync);
        }

        private void btnOpenInstallFolder_Click(object sender, EventArgs e)
        {
            var path = txtInstallPath.Text.Trim();
            if (!Directory.Exists(path))
            {
                LogMessage("Install folder does not exist yet.");
                return;
            }

            try
            {
                Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                LogMessage($"Unable to open folder: {ex.Message}");
            }
        }

        private async Task RunActionAsync(Func<Task> action)
        {
            ToggleUi(false);
            try
            {
                await action();
                LogMessage("Operation completed.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
            finally
            {
                ToggleUi(true);
            }
        }

        private void ToggleUi(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(ToggleUi), enabled);
                return;
            }

            btnInstall.Enabled = enabled;
            btnUninstall.Enabled = enabled;
            btnBrowse.Enabled = enabled;
            btnOpenInstallFolder.Enabled = enabled;
            chkAutoStart.Enabled = enabled;
            chkDesktopShortcut.Enabled = enabled;
            chkLaunchAfterInstall.Enabled = enabled;
            Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        private Task InstallAsync()
        {
            var installPath = txtInstallPath.Text.Trim();
            var autoStart = chkAutoStart.Checked;
            var desktopShortcut = chkDesktopShortcut.Checked;
            var launchAfterInstall = chkLaunchAfterInstall.Checked;

            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    throw new InvalidOperationException("Install location cannot be empty.");
                }

                if (!Directory.Exists(_payloadPath))
                {
                    throw new InvalidOperationException("Payload directory not found. Build the FreeWinBackup project in Release mode before building the setup.");
                }

                LogMessage($"Installing to {installPath}");
                CopyDirectory(_payloadPath, installPath);
                LogMessage("Files copied.");

                CreateStartMenuShortcut(installPath);
                LogMessage("Start menu shortcut created.");

                if (desktopShortcut)
                {
                    CreateDesktopShortcut(installPath);
                    LogMessage("Desktop shortcut created.");
                }
                else
                {
                    RemoveDesktopShortcut();
                }

                if (autoStart)
                {
                    EnableAutoStart(installPath);
                    LogMessage("Auto-start enabled.");
                }
                else
                {
                    DisableAutoStart();
                    LogMessage("Auto-start disabled.");
                }

                if (launchAfterInstall)
                {
                    LaunchInstalledApplication(installPath);
                }
            });
        }

        private Task UninstallAsync()
        {
            var installPath = txtInstallPath.Text.Trim();

            return Task.Run(() =>
            {
                if (Directory.Exists(installPath))
                {
                    LogMessage($"Removing {installPath}");
                    try
                    {
                        Directory.Delete(installPath, true);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Unable to remove install folder: {ex.Message}");
                    }
                }

                RemoveStartMenuShortcut();
                RemoveDesktopShortcut();
                DisableAutoStart();
                LogMessage("Uninstall complete.");
            });
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var directory in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                var targetDir = directory.Replace(sourceDir, destinationDir);
                Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                var targetFile = file.Replace(sourceDir, destinationDir);
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile) ?? destinationDir);
                File.Copy(file, targetFile, true);
            }
        }

        private void CreateStartMenuShortcut(string installPath)
        {
            var programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            var appFolder = Path.Combine(programsFolder, ApplicationName);
            Directory.CreateDirectory(appFolder);

            var shortcutPath = Path.Combine(appFolder, $"{ApplicationName}.lnk");
            CreateShortcut(shortcutPath, Path.Combine(installPath, ExecutableName), string.Empty, "Launch FreeWinBackup", installPath);
        }

        private void CreateDesktopShortcut(string installPath)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var shortcutPath = Path.Combine(desktopPath, $"{ApplicationName}.lnk");
            CreateShortcut(shortcutPath, Path.Combine(installPath, ExecutableName), string.Empty, "Launch FreeWinBackup", installPath);
        }

        private void RemoveDesktopShortcut()
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var shortcutPath = Path.Combine(desktopPath, $"{ApplicationName}.lnk");
            if (File.Exists(shortcutPath))
            {
                try
                {
                    File.Delete(shortcutPath);
                }
                catch (Exception ex)
                {
                    LogMessage($"Unable to remove desktop shortcut: {ex.Message}");
                }
            }
        }

        private void RemoveStartMenuShortcut()
        {
            var programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            var appFolder = Path.Combine(programsFolder, ApplicationName);
            if (Directory.Exists(appFolder))
            {
                try
                {
                    Directory.Delete(appFolder, true);
                }
                catch (Exception ex)
                {
                    LogMessage($"Unable to remove start menu shortcut: {ex.Message}");
                }
            }
        }

        private void EnableAutoStart(string installPath)
        {
            using (var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
            {
                if (runKey == null)
                {
                    throw new InvalidOperationException("Unable to access the Run registry key.");
                }

                var exePath = Path.Combine(installPath, ExecutableName);
                runKey.SetValue(ApplicationName, $"\"{exePath}\" /minimized");
            }
        }

        private void DisableAutoStart()
        {
            using (var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
            {
                runKey?.DeleteValue(ApplicationName, throwOnMissingValue: false);
            }
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string arguments, string description, string workingDirectory)
        {
            if (!File.Exists(targetPath))
            {
                throw new InvalidOperationException($"Target executable not found: {targetPath}");
            }

            var shortcutDirectory = Path.GetDirectoryName(shortcutPath);
            if (!string.IsNullOrWhiteSpace(shortcutDirectory))
            {
                Directory.CreateDirectory(shortcutDirectory);
            }

            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }

            var shellLink = (IShellLinkW)new ShellLink();
            shellLink.SetPath(targetPath);
            shellLink.SetArguments(arguments ?? string.Empty);
            shellLink.SetDescription(description ?? string.Empty);
            shellLink.SetWorkingDirectory(string.IsNullOrWhiteSpace(workingDirectory)
                ? Path.GetDirectoryName(targetPath) ?? string.Empty
                : workingDirectory);
            shellLink.SetIconLocation(targetPath, 0);

            var persistFile = (IPersistFile)shellLink;
            persistFile.Save(shortcutPath, true);
        }

        private void LaunchInstalledApplication(string installPath)
        {
            var exePath = Path.Combine(installPath, ExecutableName);
            if (!File.Exists(exePath))
            {
                LogMessage("Executable not found after installation; skipping launch.");
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = "/minimized",
                    WorkingDirectory = installPath,
                    UseShellExecute = false
                };

                Process.Start(startInfo);
                LogMessage("FreeWinBackup launched.");
            }
            catch (Exception ex)
            {
                LogMessage($"Unable to launch FreeWinBackup: {ex.Message}");
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, IntPtr pfd, uint fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hWnd, uint fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        private void LogMessage(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
