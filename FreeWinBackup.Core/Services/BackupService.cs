using System;
using System.Diagnostics;
using System.IO;
using FreeWinBackup.Core.Models;

namespace FreeWinBackup.Core.Services
{
    public class BackupService
    {
        private readonly IStorageService _storageService;
        private readonly LoggingService _loggingService;
        private readonly ServiceControlService _serviceControl;
        private readonly RetentionService _retentionService;

        public BackupService(IStorageService storageService)
        {
            _storageService = storageService;
            _loggingService = new LoggingService();
            _serviceControl = new ServiceControlService();
            _retentionService = new RetentionService();
        }

        public void RunBackup(BackupSchedule schedule)
        {
            var startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _loggingService.Log(new LogEntry
                {
                    ScheduleId = schedule.Id,
                    ScheduleName = schedule.Name,
                    Message = $"Starting backup: {schedule.Name}",
                    Level = LogLevel.Info
                });

                // Stop services before backup
                _serviceControl.StopServices(schedule.ServicesToStop, schedule.Id, schedule.Name);

                // Create versioned backup subfolder
                var backupFolderName = $"backup_{startTime:yyyyMMdd_HHmmss}";
                var versionedDestination = Path.Combine(schedule.DestinationFolder, backupFolderName);

                // Perform the backup to the versioned folder
                var result = CopyDirectory(schedule.SourceFolder, versionedDestination);

                // Start services after backup
                _serviceControl.StartServices(schedule.ServicesToStop, schedule.Id, schedule.Name);

                // Apply retention policy if enabled
                _retentionService.ApplyRetentionPolicy(schedule);

                stopwatch.Stop();

                // Update last run time
                var settings = _storageService.LoadSettings();
                var savedSchedule = settings.Schedules.Find(s => s.Id == schedule.Id);
                if (savedSchedule != null)
                {
                    savedSchedule.LastRun = startTime;
                    _storageService.SaveSettings(settings);
                }

                _loggingService.Log(new LogEntry
                {
                    ScheduleId = schedule.Id,
                    ScheduleName = schedule.Name,
                    Message = $"Backup completed: {schedule.Name} to {backupFolderName}",
                    Level = LogLevel.Success,
                    IsSuccess = true,
                    FilesCount = result.FilesCount,
                    BytesCopied = result.BytesCopied,
                    Duration = stopwatch.Elapsed
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                // Try to restart services even if backup failed
                try
                {
                    _serviceControl.StartServices(schedule.ServicesToStop, schedule.Id, schedule.Name);
                }
                catch { }

                _loggingService.Log(new LogEntry
                {
                    ScheduleId = schedule.Id,
                    ScheduleName = schedule.Name,
                    Message = $"Backup failed: {ex.Message}",
                    Level = LogLevel.Error,
                    IsSuccess = false,
                    Duration = stopwatch.Elapsed
                });

                throw;
            }
        }

        private (long FilesCount, long BytesCopied) CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }

            Directory.CreateDirectory(destDir);

            long filesCount = 0;
            long bytesCopied = 0;

            // Copy files
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
                filesCount++;
                bytesCopied += new FileInfo(file).Length;
            }

            // Copy subdirectories
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, dirName);
                var result = CopyDirectory(subDir, destSubDir);
                filesCount += result.FilesCount;
                bytesCopied += result.BytesCopied;
            }

            return (filesCount, bytesCopied);
        }
    }
}
