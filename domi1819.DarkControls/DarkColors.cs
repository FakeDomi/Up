using System.Drawing;

namespace domi1819.DarkControls
{
    public class DarkColors
    {
        private static Color strongColor;

        public static Color Control { get; set; }
        public static Color Control2 { get; set; }
        public static Color Workspace { get; set; }
        public static Color Border { get; set; }
        public static Color Foreground { get; set; }

        public static Color StrongColor
        {
            get
            {
                return strongColor;
            }
            set
            {
                strongColor = value;
                PaleColor = Color.FromArgb(92, value);
            }
        }
        
        public static Color PaleColor { get; set; }

        static DarkColors()
        {
            Control = Color.FromArgb(37, 37, 38);
            Control2 = Color.FromArgb(42, 42, 44);
            Workspace = Color.FromArgb(45, 45, 48);
            Border = Color.FromArgb(67, 67, 70);
            Foreground = Color.FromArgb(241, 241, 241);

            StrongColor = Color.FromArgb(16, 48, 128);
        }
    }
}
