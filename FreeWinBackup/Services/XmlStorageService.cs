using System;
using System.IO;
using System.Xml.Serialization;
using FreeWinBackup.Models;

namespace FreeWinBackup.Services
{
    public class XmlStorageService : IStorageService
    {
        private readonly string _settingsPath;

        public XmlStorageService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FreeWinBackup");
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _settingsPath = Path.Combine(appDataPath, "settings.xml");
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
                var serializer = new XmlSerializer(typeof(ScheduleSettings));
                using (var reader = new StreamReader(_settingsPath))
                {
                    return (ScheduleSettings)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return new ScheduleSettings();
            }
        }

        public void SaveSettings(ScheduleSettings settings)
        {
            var serializer = new XmlSerializer(typeof(ScheduleSettings));
            using (var writer = new StreamWriter(_settingsPath))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
