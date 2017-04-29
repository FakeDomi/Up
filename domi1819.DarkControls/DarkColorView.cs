using System;
using System.Drawing;
using System.Windows.Forms;
using domi1819.UpCore.Utilities;

namespace domi1819.DarkControls
{
    public sealed partial class DarkColorView : UserControl, IGlowComponent
    {
        private Color color;
        private Brush brush;
        private string text;

        private readonly ColorDialog colorDialog = new ColorDialog();
        private readonly Rectangle colorRectangle = new Rectangle(4, 4, 15, 15);

        public Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.brush = new SolidBrush(value);
                this.colorDialog.Color = value;
                this.RefreshLabelText();
                this.Invalidate();
            }
        }

        public bool AllowEdit { get; set; }

        public string CustomText
        {
            get => this.text;
            set
            {
                this.text = value;
                this.RefreshLabelText();
            }
        }
        public int GlowX => this.Location.X + this.DisplayRectangle.X;

        public int GlowY => this.Location.Y + this.DisplayRectangle.Y;

        public int GlowW => this.DisplayRectangle.Width;

        public int GlowH => this.DisplayRectangle.Height;


        public event EventHandler ColorSelected;

        public DarkColorView()
        {
            this.InitializeComponent();

            this.DoubleBuffered = true;

            this.ForeColor = DarkColors.Foreground;
            this.BackColor = DarkColors.Control;

            this.uiColorLabel.ForeColor = DarkColors.Foreground;
            this.uiColorLabel.BackColor = DarkColors.Control;

            this.uiColorLabel.Click += delegate { this.OnClick(null); };
            this.uiColorLabel.MouseEnter += delegate { this.OnMouseEnter(null); };
            this.uiColorLabel.MouseLeave += delegate { this.OnMouseLeave(null); };

            this.Color = DarkColors.StrongColor;

            this.colorDialog.CustomColors[1] = 0x1818C0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, DarkColors.Border, ButtonBorderStyle.Solid);

            e.Graphics.FillRectangle(this.brush, this.colorRectangle);
            ControlPaint.DrawBorder(e.Graphics, this.colorRectangle, DarkColors.Border, ButtonBorderStyle.Solid);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            DarkForm.UpdateGlowComponent(this, true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            DarkForm.UpdateGlowComponent(this, false);
        }

        protected override void OnClick(EventArgs e)
        {
            if (e != null)
            {
                base.OnClick(e);
            }

            if (this.AllowEdit && this.colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Color = this.colorDialog.Color;

                this.ColorSelected?.Invoke(this, new EventArgs());
            }
        }

        private void RefreshLabelText()
        {
            this.uiColorLabel.Text = (this.CustomText ?? "") + this.color.ToHexString();
        }
    }
}
