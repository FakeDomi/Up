using System;
using System.Drawing;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public partial class DarkProgressBar : UserControl, IGlowComponent
    {
        private const int BarPadding = 2;

        private static readonly Brush ForegroundBrush = new SolidBrush(DarkColors.Foreground);
        private static readonly StringFormat StringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        private static readonly Font TextFont = new Font(FontFamily.GenericSansSerif, 8.25F);

        private SolidBrush textOverlayBrush = new SolidBrush(DarkColors.GetForegroundColor(DarkColors.StrongColor));

        private string valueText;
        private float value;

        public float Value
        {
            get => this.value;
            set
            {
                this.value = value < 0 ? 0 : value > 1 ? 1 : value;
                this.valueText = $"{this.ValueInt} %";
                this.Invalidate();
            }
        }

        public int ValueInt
        {
            get => (int)(this.Value * 100);
            set => this.Value = value / 100F;
        }

        public SolidBrush Brush { get; set; }

        public Color BarColor
        {
            get => this.Brush.Color;
            set
            {
                this.Brush = new SolidBrush(value);
                this.textOverlayBrush = new SolidBrush(DarkColors.GetForegroundColor(DarkColors.StrongColor));
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
            
            this.MouseEnter += this.LabelMouseEnter;
            this.MouseLeave += this.LabelMouseLeave;

            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            ControlPaint.DrawBorder(g, this.DisplayRectangle, DarkColors.Border, ButtonBorderStyle.Solid);

            if (this.value >= 0)
            {
                RectangleF fullArea = new RectangleF(0, 0, this.Width, this.Height);
                Rectangle barArea = new Rectangle(BarPadding, BarPadding, (int)((this.Width - 2 * BarPadding) * this.value), this.Height - 2 * BarPadding);

                g.DrawString(this.valueText, TextFont, ForegroundBrush, fullArea, StringFormat);
                g.FillRectangle(this.Brush, barArea);
                g.Clip = new Region(barArea);
                g.DrawString(this.valueText, TextFont, this.textOverlayBrush, fullArea, StringFormat);
            }
        }

        private void LabelMouseEnter(object sender, EventArgs e)
        {
            if (this.Parent is DarkForm parent)
            {
                parent.GlowComponent = this;
                parent.Invalidate();
            }
        }

        private void LabelMouseLeave(object sender, EventArgs e)
        {
            if (this.Parent is DarkForm parent)
            {
                parent.GlowComponent = null;
                parent.Invalidate();
            }
        }
    }
}
