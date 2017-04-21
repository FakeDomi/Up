using System;
using System.Drawing;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public partial class DarkProgressBar : UserControl, IGlowComponent
    {
        private float value;

        public float Value
        {
            get { return this.value; }
            set
            {
                this.value = value < 0 ? 0 : value > 1 ? 1 : value;
                this.label.Text = (int)(value * 100) + " %";
            }
        }

        public int ValueInt
        {
            get { return (int)(this.Value * 100); }
            set { this.Value = value / 100F; }
        }

        public SolidBrush Brush { get; set; }

        public Color BarColor
        {
            get { return this.Brush.Color; }
            set
            {
                this.Brush = new SolidBrush(value);
                this.label.ForeColor = DarkColors.GetForegroundColor(value);
                this.Invalidate();
            }
        }

        public int GlowX => this.Location.X + this.DisplayRectangle.X;

        public int GlowY => this.Location.Y + this.DisplayRectangle.Y;

        public int GlowW => this.DisplayRectangle.Width;

        public int GlowH => this.DisplayRectangle.Height;

        public DarkProgressBar()
        {
            this.InitializeComponent();
            this.BackColor = DarkColors.Control;
            this.BarColor = DarkColors.StrongColor;

            this.label.BackColor = Color.Transparent;
            this.label.ForeColor = DarkColors.GetForegroundColor(this.BarColor);

            this.label.MouseEnter += this.LabelMouseEnter;
            this.label.MouseLeave += this.LabelMouseLeave;

            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, DarkColors.Border, ButtonBorderStyle.Solid);

            if (this.value > 0)
            {
                e.Graphics.FillRectangle(this.Brush, 2, 2, (this.Width - 4) * this.value, this.Height - 4);
            }
        }

        private void LabelMouseEnter(object sender, EventArgs e)
        {
            DarkForm parent = this.Parent as DarkForm;

            if (parent != null)
            {
                parent.GlowComponent = this;
                parent.Invalidate();
            }
        }

        private void LabelMouseLeave(object sender, EventArgs e)
        {
            DarkForm parent = this.Parent as DarkForm;

            if (parent != null)
            {
                parent.GlowComponent = null;
                parent.Invalidate();
            }
        }
    }
}
