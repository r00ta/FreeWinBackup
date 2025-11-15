using System;
using System.IO;
using System.Linq;
using FreeWinBackup.Core.Models;

namespace FreeWinBackup.Core.Services
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
                var deletedBackupSets = 0;
                var deletedSize = 0L;

                // Get all backup subdirectories (folders starting with "backup_")
                var backupDirs = Directory.GetDirectories(schedule.DestinationFolder, "backup_*", SearchOption.TopDirectoryOnly);

                foreach (var backupDir in backupDirs)
                {
                    try
                    {
                        var dirInfo = new DirectoryInfo(backupDir);
                        if (dirInfo.CreationTime < cutoffDate)
                        {
                            // Calculate size before deletion
                            var size = CalculateDirectorySize(backupDir);
                            
                            // Delete the entire backup set
                            Directory.Delete(backupDir, true);
                            deletedBackupSets++;
                            deletedSize += size;
                            
                            _loggingService.Log(new LogEntry
                            {
                                ScheduleId = schedule.Id,
                                ScheduleName = schedule.Name,
                                Message = $"Deleted old backup set: {dirInfo.Name}",
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
                            Message = $"Failed to delete backup directory {backupDir}: {ex.Message}",
                            Level = LogLevel.Warning,
                            IsSuccess = false
                        });
                    }
                }

                if (deletedBackupSets > 0)
                {
                    _loggingService.Log(new LogEntry
                    {
                        ScheduleId = schedule.Id,
                        ScheduleName = schedule.Name,
                        Message = $"Retention policy applied: deleted {deletedBackupSets} backup set(s) ({FormatBytes(deletedSize)}) older than {schedule.RetentionDays} days",
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

        private long CalculateDirectorySize(string path)
        {
            long size = 0;
            try
            {
                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch { }
                }
            }
            catch { }
            return size;
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
