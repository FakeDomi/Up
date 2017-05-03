using System.Drawing;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public class DarkForm : Form
    {
        private IGlowComponent focus, hover;

        public bool DisableGlow { get; set; }
        
        public DarkForm()
        {
            this.BackColor = DarkPainting.Workspace;
            this.ForeColor = DarkPainting.Foreground;

            this.DoubleBuffered = true;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DisableGlow)
            {
                return;
            }

            if (this.focus != null)
            {
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.focus.GlowX - 1, this.focus.GlowY - 1, this.focus.GlowW + 2, this.focus.GlowH + 2), DarkPainting.StrongColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.focus.GlowX - 2, this.focus.GlowY, this.focus.GlowW + 4, this.focus.GlowH), DarkPainting.PaleColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.focus.GlowX, this.focus.GlowY - 2, this.focus.GlowW, this.focus.GlowH + 4), DarkPainting.PaleColor, ButtonBorderStyle.Solid);
            }

            if (this.hover != null)
            {
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.hover.GlowX - 1, this.hover.GlowY - 1, this.hover.GlowW + 2, this.hover.GlowH + 2), DarkPainting.StrongColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.hover.GlowX - 2, this.hover.GlowY, this.hover.GlowW + 4, this.hover.GlowH), DarkPainting.PaleColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.hover.GlowX, this.hover.GlowY - 2, this.hover.GlowW, this.hover.GlowH + 4), DarkPainting.PaleColor, ButtonBorderStyle.Solid);
            }
        }

        internal static void UpdateGlow(bool focus, Control control, bool active)
        {
            if (control.Parent is DarkForm parent)
            {
                IGlowComponent glowComponent = (IGlowComponent)(active ? control : null);

                if (focus)
                {
                    parent.focus = glowComponent;
                }
                else
                {
                    parent.hover = glowComponent;
                }

                parent.Invalidate();
            }
        }
    }
}
