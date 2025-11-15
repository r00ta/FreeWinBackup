using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using FreeWinBackup.Core.Models;
using FreeWinBackup.Core.Services;

namespace FreeWinBackup.ServiceHost
{
    public partial class FreeWinBackupWindowsService : ServiceBase
    {
        private SchedulerService _schedulerService;
        private IStorageService _storageService;
        private LoggingService _loggingService;
        private string _logDirectory;
        private const string EventLogSource = "FreeWinBackup";
        private const string EventLogName = "Application";

        public FreeWinBackupWindowsService()
        {
            this.ServiceName = "FreeWinBackup";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;

            // Initialize event log for critical errors
            try
            {
                if (!EventLog.SourceExists(EventLogSource))
                {
                    EventLog.CreateEventSource(EventLogSource, EventLogName);
                }
            }
            catch
            {
                // If we can't create event log source, continue anyway
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Read configuration
                _logDirectory = ConfigurationManager.AppSettings["LogDirectory"];
                
                // If log directory is not configured, use default
                if (string.IsNullOrWhiteSpace(_logDirectory))
                {
                    _logDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "FreeWinBackup");
                }

                // Ensure log directory exists
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }

                // Initialize services
                _storageService = new JsonStorageService();
                _loggingService = new LoggingService();
                _schedulerService = new SchedulerService(_storageService);

                // Start the scheduler
                _schedulerService.Start();

                // Log startup
                _loggingService.Log(new LogEntry
                {
                    Message = "FreeWinBackup Windows Service started successfully",
                    Level = LogLevel.Info,
                    IsSuccess = true
                });

                LogToEventLog("FreeWinBackup service started successfully", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to start FreeWinBackup service: {ex.Message}";
                LogToEventLog(errorMessage, EventLogEntryType.Error);
                
                // Try to log to file if possible
                try
                {
                    _loggingService?.Log(new LogEntry
                    {
                        Message = errorMessage,
                        Level = LogLevel.Error,
                        IsSuccess = false
                    });
                }
                catch { }

                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                // Stop the scheduler
                _schedulerService?.Stop();

                // Log shutdown
                _loggingService?.Log(new LogEntry
                {
                    Message = "FreeWinBackup Windows Service stopped",
                    Level = LogLevel.Info,
                    IsSuccess = true
                });

                LogToEventLog("FreeWinBackup service stopped", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error stopping FreeWinBackup service: {ex.Message}";
                LogToEventLog(errorMessage, EventLogEntryType.Warning);

                try
                {
                    _loggingService?.Log(new LogEntry
                    {
                        Message = errorMessage,
                        Level = LogLevel.Warning,
                        IsSuccess = false
                    });
                }
                catch { }
            }
        }

        private void LogToEventLog(string message, EventLogEntryType type)
        {
            try
            {
                using (EventLog eventLog = new EventLog(EventLogName))
                {
                    eventLog.Source = EventLogSource;
                    eventLog.WriteEntry(message, type);
                }
            }
            catch
            {
                // Silently fail if we can't write to event log
            }
        }
    }
}
