using System.Drawing;

namespace domi1819.UpCore.Config
{
    public class WrappedColor
    {
        private Color color;

        public int Red
        {
            get { return this.color.R; }
            set { this.color = Color.FromArgb(value, this.color.G, this.color.B); }
        }

        public int Green
        {
            get { return this.color.G; }
            set { this.color = Color.FromArgb(this.color.R, value, this.color.B); }
        }

        public int Blue
        {
            get { return this.color.B; }
            set { this.color = Color.FromArgb(this.color.R, this.color.G, value); }
        }

        public static WrappedColor Of(Color color)
        {
            return new WrappedColor { color = color };
        }

        public Color GetColor()
        {
            return this.color;
        }
    }
}
