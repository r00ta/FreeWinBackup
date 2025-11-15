using FreeWinBackup.Core.Models;

namespace FreeWinBackup.Core.Services
{
    public interface IStorageService
    {
        ScheduleSettings LoadSettings();
        void SaveSettings(ScheduleSettings settings);
    }
}
