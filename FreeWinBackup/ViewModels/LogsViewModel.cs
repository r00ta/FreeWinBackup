using System.Collections.ObjectModel;
using System.Linq;
using FreeWinBackup.Core.Models;
using FreeWinBackup.Core.Services;

namespace FreeWinBackup.ViewModels
{
    public class LogsViewModel : BaseViewModel
    {
        private readonly LoggingService _loggingService;
        private ObservableCollection<LogEntry> _logs;
        private LogEntry _selectedLog;

        public ObservableCollection<LogEntry> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        public LogEntry SelectedLog
        {
            get => _selectedLog;
            set => SetProperty(ref _selectedLog, value);
        }

        public LogsViewModel()
        {
            _loggingService = new LoggingService();
            LoadLogs();
        }

        private void LoadLogs()
        {
            var allLogs = _loggingService.GetLogs();
            Logs = new ObservableCollection<LogEntry>(allLogs.OrderByDescending(l => l.Timestamp));
        }

        public void Refresh()
        {
            LoadLogs();
        }
    }
}
