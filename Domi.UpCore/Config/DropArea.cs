using System.Drawing;

namespace Domi.UpCore.Config
{
    public class DropArea
    {
        public bool Show { get; set; }

        public bool Snap { get; set; }

        public bool Lock { get; set; }

        public Point Location { get; set; }

        public DropArea()
        {
            this.Snap = true;
        }
    }
}
