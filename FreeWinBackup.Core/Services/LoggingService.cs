using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FreeWinBackup.Core.Models;
using Newtonsoft.Json;

namespace FreeWinBackup.Core.Services
{
    public class LoggingService
    {
        private readonly string _logFilePath;

        public LoggingService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FreeWinBackup");
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _logFilePath = Path.Combine(appDataPath, "logs.json");
        }

        public void Log(LogEntry entry)
        {
            var logs = GetLogs();
            logs.Add(entry);
            SaveLogs(logs);
        }

        public List<LogEntry> GetLogs()
        {
            if (!File.Exists(_logFilePath))
            {
                return new List<LogEntry>();
            }

            try
            {
                var json = File.ReadAllText(_logFilePath);
                return JsonConvert.DeserializeObject<List<LogEntry>>(json) ?? new List<LogEntry>();
            }
            catch
            {
                return new List<LogEntry>();
            }
        }

        private void SaveLogs(List<LogEntry> logs)
        {
            // Keep only last 1000 entries
            if (logs.Count > 1000)
            {
                logs = logs.OrderByDescending(l => l.Timestamp).Take(1000).ToList();
            }

            var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(_logFilePath, json);
        }

        public void CleanOldLogs(int daysToKeep)
        {
            var logs = GetLogs();
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            var filteredLogs = logs.Where(l => l.Timestamp >= cutoffDate).ToList();
            SaveLogs(filteredLogs);
        }
    }
}
