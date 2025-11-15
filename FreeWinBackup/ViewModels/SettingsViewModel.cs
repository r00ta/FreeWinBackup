using System.Windows.Input;
using FreeWinBackup.Core.Models;
using FreeWinBackup.Core.Services;
using FreeWinBackup.UI;

namespace FreeWinBackup.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IStorageService _storageService;
        private ScheduleSettings _settings;
        private bool _previousAutoStartState;

        public ScheduleSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel()
        {
            _storageService = new JsonStorageService();
            SaveCommand = new RelayCommand(_ => SaveSettings());
            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings = _storageService.LoadSettings();
            _previousAutoStartState = Settings.AutoStartEnabled;
            
            // Sync with actual registry state
            bool registryAutoStart = AutoStartManager.IsAutoStartEnabled();
            if (Settings.AutoStartEnabled != registryAutoStart)
            {
                Settings.AutoStartEnabled = registryAutoStart;
            }
        }

        private void SaveSettings()
        {
            // Handle auto-start changes
            if (Settings.AutoStartEnabled != _previousAutoStartState)
            {
                if (Settings.AutoStartEnabled)
                {
                    if (!AutoStartManager.EnableAutoStart())
                    {
                        System.Windows.MessageBox.Show(
                            "Failed to enable auto-start. Please check permissions.",
                            "Settings",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Warning);
                        Settings.AutoStartEnabled = false;
                    }
                }
                else
                {
                    if (!AutoStartManager.DisableAutoStart())
                    {
                        System.Windows.MessageBox.Show(
                            "Failed to disable auto-start. Please check permissions.",
                            "Settings",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Warning);
                        Settings.AutoStartEnabled = true;
                    }
                }
                _previousAutoStartState = Settings.AutoStartEnabled;
            }

            _storageService.SaveSettings(Settings);
            System.Windows.MessageBox.Show("Settings saved successfully.", "Settings", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
