using System;
using System.Drawing;

namespace domi1819.DarkControls
{
    public static class DarkColors
    {
        private static Color strongColor;

        public static Color Control { get; }
        public static Color Control2 { get; }
        public static Color Workspace { get; }
        public static Color Border { get; }
        public static Color Foreground { get; }

        public static Color StrongColor
        {
            get => strongColor;
            set
            {
                strongColor = value;
                PaleColor = Color.FromArgb(92, value);
            }
        }

        public static Color PaleColor { get; private set; }

        static DarkColors()
        {
            Control = Color.FromArgb(37, 37, 38);
            Control2 = Color.FromArgb(42, 42, 44);
            Workspace = Color.FromArgb(45, 45, 48);
            Border = Color.FromArgb(67, 67, 70);
            Foreground = Color.FromArgb(241, 241, 241);

            StrongColor = Color.FromArgb(16, 48, 128);
        }

        public static Color GetForegroundColor(Color background)
        {
            return (int)Math.Sqrt(background.R * background.R * 0.299 + background.G * background.G * 0.587 + background.B * background.B * 0.114) > 127 ? Color.Black : Foreground;
        }
    }
}
