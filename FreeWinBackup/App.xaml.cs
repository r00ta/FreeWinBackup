using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using FreeWinBackup.Core.Services;
using FreeWinBackup.UI;
using FreeWinBackup.Views;

namespace FreeWinBackup
{
    public partial class App : System.Windows.Application
    {
        private TrayManager _trayManager;
        private SingleInstanceManager _singleInstanceManager;
        private MainWindow _mainWindow;
        private IStorageService _storageService;
        private bool _startMinimized;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check for single instance
            _singleInstanceManager = new SingleInstanceManager();
            if (!_singleInstanceManager.IsFirstInstance())
            {
                // Another instance is already running
                System.Windows.MessageBox.Show(
                    "FreeWinBackup is already running. Check the system tray.",
                    "FreeWinBackup",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Shutdown();
                return;
            }

            // Initialize storage service
            _storageService = new JsonStorageService();

            // Check if started with /minimized argument
            _startMinimized = e.Args.Contains("/minimized", StringComparer.OrdinalIgnoreCase);
            
            // Load settings to check StartMinimized preference
            var settings = _storageService.LoadSettings();
            if (settings.StartMinimized)
            {
                _startMinimized = true;
            }

            // Create main window but don't show it yet
            _mainWindow = new MainWindow();

            // Initialize tray manager
            _trayManager = new TrayManager(
                _storageService,
                ShowMainWindow,
                ExitApplication,
                RunBackupNow);

            // Show balloon tip if started minimized
            if (_startMinimized)
            {
                _trayManager.ShowBalloonTip(
                    "FreeWinBackup",
                    "Application started in system tray",
                    ToolTipIcon.Info);
            }
            else
            {
                ShowMainWindow();
            }
        }

        private void ShowMainWindow()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Activate();
            }
        }

        private void RunBackupNow()
        {
            // This will be implemented to trigger backup - placeholder for now
            _trayManager?.ShowBalloonTip(
                "FreeWinBackup",
                "Backup functionality will be integrated here",
                ToolTipIcon.Info);
        }

        private void ExitApplication()
        {
            // Allow the main window to close
            if (_mainWindow != null)
            {
                _mainWindow.AllowClose = true;
            }
            
            // Clean up and exit
            Shutdown();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Handled in OnStartup
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Cleanup
            _trayManager?.Dispose();
            _singleInstanceManager?.Dispose();
        }
    }
}
