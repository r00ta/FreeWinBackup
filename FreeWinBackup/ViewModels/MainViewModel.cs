using System.Windows.Controls;
using System.Windows.Input;

namespace FreeWinBackup.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Page _currentPage;
        private string _currentPageTitle;

        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set => SetProperty(ref _currentPageTitle, value);
        }

        public ICommand NavigateToSchedulesCommand { get; }
        public ICommand NavigateToLogsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        public MainViewModel()
        {
            NavigateToSchedulesCommand = new RelayCommand(_ => NavigateToSchedules());
            NavigateToLogsCommand = new RelayCommand(_ => NavigateToLogs());
            NavigateToSettingsCommand = new RelayCommand(_ => NavigateToSettings());

            // Navigate to schedules by default
            NavigateToSchedules();
        }

        private void NavigateToSchedules()
        {
            CurrentPage = new Views.SchedulesPage();
            CurrentPageTitle = "Backup Schedules";
        }

        private void NavigateToLogs()
        {
            CurrentPage = new Views.LogsPage();
            CurrentPageTitle = "Backup Logs";
        }

        private void NavigateToSettings()
        {
            CurrentPage = new Views.SettingsPage();
            CurrentPageTitle = "Settings";
        }
    }
}
