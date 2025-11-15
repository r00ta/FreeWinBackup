using System;
using Microsoft.Win32;

namespace FreeWinBackup.UI
{
    /// <summary>
    /// Manages Windows auto-start functionality via registry
    /// </summary>
    public static class AutoStartManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "FreeWinBackup";

        /// <summary>
        /// Enables auto-start by adding registry entry
        /// </summary>
        public static bool EnableAutoStart()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        // Add /minimized argument so app starts to tray
                        key.SetValue(AppName, $"\"{exePath}\" /minimized");
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Disables auto-start by removing registry entry
        /// </summary>
        public static bool DisableAutoStart()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        // Check if value exists before trying to delete
                        if (key.GetValue(AppName) != null)
                        {
                            key.DeleteValue(AppName, false);
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks if auto-start is currently enabled
        /// </summary>
        public static bool IsAutoStartEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(AppName);
                        return value != null;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}
