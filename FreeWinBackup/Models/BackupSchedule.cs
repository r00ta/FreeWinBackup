using System;
using System.Collections.Generic;

namespace FreeWinBackup.Models
{
    public class BackupSchedule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }
        public FrequencyType Frequency { get; set; }
        public TimeSpan RunTime { get; set; }
        public bool IsEnabled { get; set; }
        public List<string> ServicesToStop { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // For weekly schedules
        public DayOfWeek? DayOfWeek { get; set; }
        
        // For monthly schedules
        public int? DayOfMonth { get; set; }

        public BackupSchedule()
        {
            Id = Guid.NewGuid();
            ServicesToStop = new List<string>();
            IsEnabled = true;
            CreatedDate = DateTime.Now;
            RunTime = new TimeSpan(2, 0, 0); // Default to 2 AM
        }
    }
}
