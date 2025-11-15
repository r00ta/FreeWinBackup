using System;
using System.IO;
using System.Linq;
using FreeWinBackup.Models;

namespace FreeWinBackup.Services
{
    public class RetentionService
    {
        private readonly LoggingService _loggingService;

        public RetentionService()
        {
            _loggingService = new LoggingService();
        }

        public void ApplyRetentionPolicy(BackupSchedule schedule)
        {
            if (!schedule.EnableRetentionPolicy || schedule.RetentionDays <= 0)
                return;

            if (!Directory.Exists(schedule.DestinationFolder))
                return;

            try
            {
                var cutoffDate = DateTime.Now.AddDays(-schedule.RetentionDays);
                var deletedCount = 0;
                var deletedSize = 0L;

                // Get all files in destination directory and subdirectories
                var files = Directory.GetFiles(schedule.DestinationFolder, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.LastWriteTime < cutoffDate)
                        {
                            var size = fileInfo.Length;
                            File.Delete(file);
                            deletedCount++;
                            deletedSize += size;
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Log(new LogEntry
                        {
                            ScheduleId = schedule.Id,
                            ScheduleName = schedule.Name,
                            Message = $"Failed to delete file {file}: {ex.Message}",
                            Level = LogLevel.Warning,
                            IsSuccess = false
                        });
                    }
                }

                // Clean up empty directories
                CleanEmptyDirectories(schedule.DestinationFolder);

                if (deletedCount > 0)
                {
                    _loggingService.Log(new LogEntry
                    {
                        ScheduleId = schedule.Id,
                        ScheduleName = schedule.Name,
                        Message = $"Retention policy applied: deleted {deletedCount} files ({FormatBytes(deletedSize)}) older than {schedule.RetentionDays} days",
                        Level = LogLevel.Info,
                        IsSuccess = true
                    });
                }
            }
            catch (Exception ex)
            {
                _loggingService.Log(new LogEntry
                {
                    ScheduleId = schedule.Id,
                    ScheduleName = schedule.Name,
                    Message = $"Failed to apply retention policy: {ex.Message}",
                    Level = LogLevel.Error,
                    IsSuccess = false
                });
            }
        }

        private void CleanEmptyDirectories(string path)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    CleanEmptyDirectories(directory);
                    
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        try
                        {
                            Directory.Delete(directory);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
