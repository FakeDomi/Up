using System.Drawing;

namespace domi1819.UpCore.Config
{
    public class WrappedColor
    {
        private Color color;
        
        // ReSharper disable once UnusedMember.Global
        public int Red
        {
            get => this.color.R;
            set => this.color = Color.FromArgb(value, this.color.G, this.color.B);
        }
        
        // ReSharper disable once UnusedMember.Global
        public int Green
        {
            get => this.color.G;
            set => this.color = Color.FromArgb(this.color.R, value, this.color.B);
        }
        
        // ReSharper disable once UnusedMember.Global
        public int Blue
        {
            get => this.color.B;
            set => this.color = Color.FromArgb(this.color.R, this.color.G, value);
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
