using System;

namespace FreeWinBackup.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string ScheduleName { get; set; }
        public Guid ScheduleId { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public bool IsSuccess { get; set; }
        public long? FilesCount { get; set; }
        public long? BytesCopied { get; set; }
        public TimeSpan? Duration { get; set; }

        public LogEntry()
        {
            Timestamp = DateTime.Now;
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Success
    }
}
