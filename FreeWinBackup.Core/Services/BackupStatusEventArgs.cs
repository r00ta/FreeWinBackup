using System;

namespace FreeWinBackup.Core.Services
{
    /// <summary>
    /// Event arguments for backup status changes
    /// </summary>
    public class BackupStatusEventArgs : EventArgs
    {
        public string ScheduleName { get; set; }
        public BackupStatus Status { get; set; }
        public string Message { get; set; }
        public Exception Error { get; set; }
    }

    /// <summary>
    /// Backup status enumeration
    /// </summary>
    public enum BackupStatus
    {
        Started,
        InProgress,
        Completed,
        Failed
    }
}
