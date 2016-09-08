using System.Drawing;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public class DarkForm : Form
    {
        public bool DisableGlow { get; set; }

        internal IGlowComponent GlowComponent { get; set; }

        public DarkForm()
        {
            this.BackColor = DarkColors.Workspace;
            this.ForeColor = DarkColors.Foreground;

            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!this.DisableGlow && this.GlowComponent != null)
            {
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.GlowComponent.GlowX - 1, this.GlowComponent.GlowY - 1, this.GlowComponent.GlowW + 2, this.GlowComponent.GlowH + 2), DarkColors.StrongColor, ButtonBorderStyle.Solid);

                //e.Graphics.DrawRectangle(new Pen(new SolidBrush(DarkColors.PaleColor), 2.5F), GlowComponent.GlowX - 1, GlowComponent.GlowY - 1, GlowComponent.GlowW + 2, GlowComponent.GlowH + 2);

                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.GlowComponent.GlowX - 2, this.GlowComponent.GlowY, this.GlowComponent.GlowW + 4, this.GlowComponent.GlowH), DarkColors.PaleColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.GlowComponent.GlowX, this.GlowComponent.GlowY - 2, this.GlowComponent.GlowW, this.GlowComponent.GlowH + 4), DarkColors.PaleColor, ButtonBorderStyle.Solid);
            }
        }
    }
}
