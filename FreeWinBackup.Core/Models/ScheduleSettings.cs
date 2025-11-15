using System.Collections.Generic;

namespace FreeWinBackup.Core.Models
{
    public class ScheduleSettings
    {
        public List<BackupSchedule> Schedules { get; set; }
        public string LogFilePath { get; set; }
        public int MaxLogDays { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public string SmtpServer { get; set; }
        public string EmailTo { get; set; }
        public bool AutoStartEnabled { get; set; }
        public bool StartMinimized { get; set; }

        public ScheduleSettings()
        {
            Schedules = new List<BackupSchedule>();
            MaxLogDays = 30;
            EnableEmailNotifications = false;
            AutoStartEnabled = false;
            StartMinimized = true;
        }
    }
}
