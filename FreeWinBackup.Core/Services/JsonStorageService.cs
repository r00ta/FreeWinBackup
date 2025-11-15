using System;
using System.IO;
using FreeWinBackup.Core.Models;
using Newtonsoft.Json;

namespace FreeWinBackup.Core.Services
{
    public class JsonStorageService : IStorageService
    {
        private readonly string _settingsPath;

        public JsonStorageService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FreeWinBackup");
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _settingsPath = Path.Combine(appDataPath, "settings.json");
        }

        public ScheduleSettings LoadSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                var defaultSettings = new ScheduleSettings();
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            try
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonConvert.DeserializeObject<ScheduleSettings>(json) ?? new ScheduleSettings();
            }
            catch
            {
                return new ScheduleSettings();
            }
        }

        public void SaveSettings(ScheduleSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
