using System;
using System.Drawing;
using System.Windows.Forms;
using FreeWinBackup.Core.Models;
using FreeWinBackup.Core.Services;

namespace FreeWinBackup.UI
{
    /// <summary>
    /// Manages the system tray icon and its behavior
    /// </summary>
    public class TrayManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private readonly IStorageService _storageService;
        private readonly Action _showMainWindow;
        private readonly Action _exitApplication;
        private readonly Action _runBackupNow;
        
        public enum TrayIconState
        {
            Idle,
            Active,
            Error
        }

        private TrayIconState _currentState;

        public TrayManager(IStorageService storageService, Action showMainWindow, Action exitApplication, Action runBackupNow = null)
        {
            _storageService = storageService;
            _showMainWindow = showMainWindow;
            _exitApplication = exitApplication;
            _runBackupNow = runBackupNow;
            _currentState = TrayIconState.Idle;
            
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "FreeWinBackup - Idle"
            };

            // Set initial icon
            UpdateIcon(TrayIconState.Idle);

            // Create context menu
            var contextMenu = new ContextMenuStrip();
            
            contextMenu.Items.Add("Open FreeWinBackup", null, (s, e) => _showMainWindow?.Invoke());
            
            if (_runBackupNow != null)
            {
                contextMenu.Items.Add("Run Backup Now", null, (s, e) => _runBackupNow?.Invoke());
            }
            
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (s, e) => _exitApplication?.Invoke());

            _notifyIcon.ContextMenuStrip = contextMenu;
            
            // Double-click opens main window
            _notifyIcon.DoubleClick += (s, e) => _showMainWindow?.Invoke();
        }

        public void UpdateIcon(TrayIconState state)
        {
            _currentState = state;
            
            Icon icon;
            string tooltip;

            switch (state)
            {
                case TrayIconState.Active:
                    icon = CreateColoredIcon(Color.Green);
                    tooltip = "FreeWinBackup - Backup in Progress";
                    break;
                case TrayIconState.Error:
                    icon = CreateColoredIcon(Color.Red);
                    tooltip = "FreeWinBackup - Error";
                    break;
                case TrayIconState.Idle:
                default:
                    icon = CreateColoredIcon(Color.Blue);
                    tooltip = "FreeWinBackup - Idle";
                    break;
            }

            if (_notifyIcon != null)
            {
                _notifyIcon.Icon = icon;
                _notifyIcon.Text = tooltip;
            }
        }

        /// <summary>
        /// Creates a simple colored icon (placeholder until actual icon files are provided)
        /// </summary>
        private Icon CreateColoredIcon(Color color)
        {
            // Create a simple 16x16 bitmap with a colored circle
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, 12, 12);
                }
                using (var pen = new Pen(Color.White, 1))
                {
                    g.DrawEllipse(pen, 2, 2, 12, 12);
                }
            }
            
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return icon;
        }

        public void ShowBalloonTip(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.ShowBalloonTip(3000, title, message, icon);
            }
        }

        public void Hide()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
            }
        }

        public void Show()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = true;
            }
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
    }
}
