using System.ServiceProcess;

namespace FreeWinBackup.ServiceHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FreeWinBackupWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
