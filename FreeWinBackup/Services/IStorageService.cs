using FreeWinBackup.Models;

namespace FreeWinBackup.Services
{
    public interface IStorageService
    {
        ScheduleSettings LoadSettings();
        void SaveSettings(ScheduleSettings settings);
    }
}
