// ReSharper disable InconsistentNaming
namespace Domi.UpCore.Windows
{
    public static class WinConsts
    {
        public const int TRUE = 0x01;

        public const int FILE_ATTRIBUTE_NORMAL = 0x80;

        public const int HTCLIENT = 0x01;

        public const int MA_NOACTIVATE = 0x03;

        public const int SC_MOVE = 0xF010;
        public const int SC_MASK = 0xFFF0;

        public const int SHGFI_SMALLICON = 0x0001;
        public const int SHGFI_USEFILEATTRIBUTES = 0x0010;
        public const int SHGFI_ICON = 0x0100;

        public const int WM_ACTIVATE = 0x0006;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_HOTKEY = 0x0312;

        public const int WS_EX_TOPMOST = 0x0008;
    }
}
