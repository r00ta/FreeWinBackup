using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FreeWinBackup.Core.Models;

namespace FreeWinBackup.Core.Services
{
    public class SchedulerService
    {
        private readonly IStorageService _storageService;
        private readonly BackupService _backupService;
        private Timer _timer;
        private List<BackupSchedule> _schedules;

        public SchedulerService(IStorageService storageService)
        {
            _storageService = storageService;
            _backupService = new BackupService(storageService);
            LoadSchedules();
        }

        public void Start()
        {
            // Check every minute for schedules to run
            _timer = new Timer(CheckSchedules, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        public void ReloadSchedules()
        {
            LoadSchedules();
        }

        private void LoadSchedules()
        {
            var settings = _storageService.LoadSettings();
            _schedules = settings.Schedules;
        }

        private void CheckSchedules(object state)
        {
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            
            foreach (var schedule in _schedules.Where(s => s.IsEnabled))
            {
                if (ShouldRunNow(schedule, now, currentTime))
                {
                    // Run backup in a separate thread to avoid blocking the timer
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            _backupService.RunBackup(schedule);
                        }
                        catch
                        {
                            // Error already logged in BackupService
                        }
                    });
                }
            }
        }

        private bool ShouldRunNow(BackupSchedule schedule, DateTime now, TimeSpan currentTime)
        {
            // Check if we're within a minute of the scheduled time
            var timeDiff = Math.Abs((schedule.RunTime - currentTime).TotalMinutes);
            if (timeDiff > 1)
                return false;

            // Check if already run today (to avoid running multiple times in the same minute)
            if (schedule.LastRun.HasValue && schedule.LastRun.Value.Date == now.Date)
                return false;

            switch (schedule.Frequency)
            {
                case FrequencyType.Daily:
                    return true;

                case FrequencyType.Weekly:
                    return schedule.DayOfWeek.HasValue && now.DayOfWeek == schedule.DayOfWeek.Value;

                case FrequencyType.Monthly:
                    return schedule.DayOfMonth.HasValue && now.Day == schedule.DayOfMonth.Value;

                default:
                    return false;
            }
        }
    }
}
