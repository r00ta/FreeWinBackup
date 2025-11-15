using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FreeWinBackup.Models;
using FreeWinBackup.Services;

namespace FreeWinBackup.ViewModels
{
    public class SchedulesViewModel : BaseViewModel
    {
        private readonly IStorageService _storageService;
        private readonly SchedulerService _schedulerService;
        private ObservableCollection<BackupSchedule> _schedules;
        private BackupSchedule _selectedSchedule;
        private bool _isEditing;
        private BackupSchedule _editingSchedule;

        public ObservableCollection<BackupSchedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        public BackupSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set => SetProperty(ref _selectedSchedule, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public BackupSchedule EditingSchedule
        {
            get => _editingSchedule;
            set => SetProperty(ref _editingSchedule, value);
        }

        public ICommand NewScheduleCommand { get; }
        public ICommand EditScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }
        public ICommand SaveScheduleCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ToggleEnabledCommand { get; }
        public ICommand RunNowCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand BrowseDestinationCommand { get; }

        public SchedulesViewModel()
        {
            _storageService = new JsonStorageService();
            _schedulerService = new SchedulerService(_storageService);
            
            NewScheduleCommand = new RelayCommand(_ => NewSchedule());
            EditScheduleCommand = new RelayCommand(_ => EditSchedule(), _ => SelectedSchedule != null);
            DeleteScheduleCommand = new RelayCommand(_ => DeleteSchedule(), _ => SelectedSchedule != null);
            SaveScheduleCommand = new RelayCommand(_ => SaveSchedule(), _ => ValidateSchedule());
            CancelEditCommand = new RelayCommand(_ => CancelEdit());
            ToggleEnabledCommand = new RelayCommand(_ => ToggleEnabled(), _ => SelectedSchedule != null);
            RunNowCommand = new RelayCommand(_ => RunNow(), _ => SelectedSchedule != null);
            BrowseSourceCommand = new RelayCommand(_ => BrowseSource());
            BrowseDestinationCommand = new RelayCommand(_ => BrowseDestination());

            LoadSchedules();
            _schedulerService.Start();
        }

        private void LoadSchedules()
        {
            var settings = _storageService.LoadSettings();
            Schedules = new ObservableCollection<BackupSchedule>(settings.Schedules);
        }

        private void NewSchedule()
        {
            EditingSchedule = new BackupSchedule
            {
                Name = "New Backup Schedule"
            };
            IsEditing = true;
        }

        private void EditSchedule()
        {
            if (SelectedSchedule == null) return;

            EditingSchedule = new BackupSchedule
            {
                Id = SelectedSchedule.Id,
                Name = SelectedSchedule.Name,
                SourceFolder = SelectedSchedule.SourceFolder,
                DestinationFolder = SelectedSchedule.DestinationFolder,
                Frequency = SelectedSchedule.Frequency,
                RunTime = SelectedSchedule.RunTime,
                IsEnabled = SelectedSchedule.IsEnabled,
                ServicesToStop = new System.Collections.Generic.List<string>(SelectedSchedule.ServicesToStop),
                DayOfWeek = SelectedSchedule.DayOfWeek,
                DayOfMonth = SelectedSchedule.DayOfMonth,
                LastRun = SelectedSchedule.LastRun,
                CreatedDate = SelectedSchedule.CreatedDate
            };
            IsEditing = true;
        }

        private void DeleteSchedule()
        {
            if (SelectedSchedule == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the schedule '{SelectedSchedule.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var settings = _storageService.LoadSettings();
                settings.Schedules.RemoveAll(s => s.Id == SelectedSchedule.Id);
                _storageService.SaveSettings(settings);
                LoadSchedules();
                _schedulerService.ReloadSchedules();
            }
        }

        private void SaveSchedule()
        {
            if (EditingSchedule == null) return;

            var settings = _storageService.LoadSettings();
            var existing = settings.Schedules.FirstOrDefault(s => s.Id == EditingSchedule.Id);
            
            if (existing != null)
            {
                settings.Schedules.Remove(existing);
            }
            
            settings.Schedules.Add(EditingSchedule);
            _storageService.SaveSettings(settings);
            
            LoadSchedules();
            _schedulerService.ReloadSchedules();
            IsEditing = false;
            EditingSchedule = null;
        }

        private void CancelEdit()
        {
            IsEditing = false;
            EditingSchedule = null;
        }

        private bool ValidateSchedule()
        {
            if (EditingSchedule == null) return false;
            if (string.IsNullOrWhiteSpace(EditingSchedule.Name)) return false;
            if (string.IsNullOrWhiteSpace(EditingSchedule.SourceFolder)) return false;
            if (string.IsNullOrWhiteSpace(EditingSchedule.DestinationFolder)) return false;
            
            if (EditingSchedule.Frequency == FrequencyType.Weekly && !EditingSchedule.DayOfWeek.HasValue)
                return false;
            if (EditingSchedule.Frequency == FrequencyType.Monthly && !EditingSchedule.DayOfMonth.HasValue)
                return false;
                
            return true;
        }

        private void ToggleEnabled()
        {
            if (SelectedSchedule == null) return;

            var settings = _storageService.LoadSettings();
            var schedule = settings.Schedules.FirstOrDefault(s => s.Id == SelectedSchedule.Id);
            if (schedule != null)
            {
                schedule.IsEnabled = !schedule.IsEnabled;
                _storageService.SaveSettings(settings);
                LoadSchedules();
                _schedulerService.ReloadSchedules();
            }
        }

        private void RunNow()
        {
            if (SelectedSchedule == null) return;

            try
            {
                var backupService = new BackupService(_storageService);
                backupService.RunBackup(SelectedSchedule);
                MessageBox.Show($"Backup '{SelectedSchedule.Name}' completed successfully.", "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSchedules();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseSource()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                EditingSchedule.SourceFolder = dialog.SelectedPath;
                OnPropertyChanged(nameof(EditingSchedule));
            }
        }

        private void BrowseDestination()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                EditingSchedule.DestinationFolder = dialog.SelectedPath;
                OnPropertyChanged(nameof(EditingSchedule));
            }
        }
    }
}
