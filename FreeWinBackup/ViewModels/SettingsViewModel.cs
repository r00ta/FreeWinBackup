using System.Windows.Input;
using FreeWinBackup.Models;
using FreeWinBackup.Services;

namespace FreeWinBackup.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IStorageService _storageService;
        private ScheduleSettings _settings;

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
        }

        private void SaveSettings()
        {
            _storageService.SaveSettings(Settings);
            System.Windows.MessageBox.Show("Settings saved successfully.", "Settings", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
