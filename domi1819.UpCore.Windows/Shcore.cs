using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace domi1819.UpCore.Windows
{
    public static class Shcore
    {
        [DllImport("Shcore.dll")]
        public static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);

        public enum PROCESS_DPI_AWARENESS
        {
            PROCESS_DPI_UNAWARE = 0,
            PROCESS_SYSTEM_DPI_AWARE = 1,
            PROCESS_PER_MONITOR_DPI_AWARE = 2
        }
    }
}
