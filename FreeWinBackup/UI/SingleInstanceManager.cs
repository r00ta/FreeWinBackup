using System;
using System.Threading;

namespace FreeWinBackup.UI
{
    /// <summary>
    /// Manages single instance application behavior using Mutex
    /// </summary>
    public class SingleInstanceManager : IDisposable
    {
        private Mutex _mutex;
        private bool _owned;
        private const string MutexName = "FreeWinBackup_SingleInstance_Mutex";

        public bool IsFirstInstance()
        {
            try
            {
                _mutex = new Mutex(true, MutexName, out _owned);
                return _owned;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_mutex != null && _owned)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
        }
    }
}
