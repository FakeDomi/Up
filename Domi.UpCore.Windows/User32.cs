using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace Domi.UpCore.Windows
{
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr dc, uint flags);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT value);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum DPI_AWARENESS_CONTEXT
        {
            DPI_AWARENESS_CONTEXT_UNAWARE = -1,
            DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
            DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALE = -5
        }
    }
}
