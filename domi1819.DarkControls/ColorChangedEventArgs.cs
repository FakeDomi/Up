using System;
using System.Drawing;

namespace domi1819.DarkControls
{
    public class ColorChangedEventArgs : EventArgs
    {
        public Color NewColor { get; set; }

        public ColorChangedEventArgs(Color newColor)
        {
            this.NewColor = newColor;
        }
    }
}
