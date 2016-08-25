using System;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public class DarkButton : Button, IGlowComponent
    {
        public DarkButton()
        {
            this.ForeColor = DarkColors.Foreground;
            this.BackColor = DarkColors.Control;
            this.FlatStyle = FlatStyle.Flat;

            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.MouseOverBackColor = DarkColors.Control2;
            this.FlatAppearance.MouseDownBackColor = DarkColors.Workspace;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            
            ControlPaint.DrawBorder(pevent.Graphics, this.DisplayRectangle, DarkColors.Border, ButtonBorderStyle.Solid);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            DarkForm parent = this.Parent as DarkForm;

            if (parent != null)
            {
                parent.GlowComponent = this;
                parent.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            DarkForm parent = this.Parent as DarkForm;

            if (parent != null)
            {
                parent.GlowComponent = null;
                parent.Invalidate();
            }
        }

        public int GlowX
        {
            get { return this.Location.X + this.DisplayRectangle.X; }
        }

        public int GlowY
        {
            get { return this.Location.Y + this.DisplayRectangle.Y; }
        }

        public int GlowW
        {
            get { return this.DisplayRectangle.Width; }
        }

        public int GlowH
        {
            get { return this.DisplayRectangle.Height; }
        }
    }
}
