using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public partial class DarkTextBox : UserControl, IGlowComponent
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public new string Text
        {
            get { return this.textBox.Text; }
            set { this.textBox.Text = value; }
        }

        public bool UseSystemPasswordChar
        {
            get { return this.textBox.UseSystemPasswordChar; }
            set { this.textBox.UseSystemPasswordChar = value; }
        }

        public bool ReadOnly
        {
            get { return this.textBox.ReadOnly; }
            set { this.textBox.ReadOnly = value; }
        }

        public int GlowX => this.Location.X + this.DisplayRectangle.X;

        public int GlowY => this.Location.Y + this.DisplayRectangle.Y;

        public int GlowW => this.DisplayRectangle.Width;

        public int GlowH => this.DisplayRectangle.Height;

        public DarkTextBox()
        {
            this.InitializeComponent();

            this.DoubleBuffered = true;
            
            this.BackColor = DarkColors.Control;
            this.textBox.BackColor = DarkColors.Control;
            this.textBox.ForeColor = DarkColors.Foreground;
            
            this.MinimumSize = new Size(20, 20);

            this.textBox.MouseEnter += (sender, args) => { this.OnMouseEnter(args); };
            this.textBox.MouseLeave += (sender, args) => { this.OnMouseLeave(args); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, DarkColors.Border, ButtonBorderStyle.Solid);
        }
        
        protected override void OnEnter(EventArgs e)
        {
            this.BeginInvoke((Action)delegate
            {
                this.textBox.SelectAll();
            });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.textBox.Size = new Size(this.Width - 8, this.Height - 7);
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
    }
}
