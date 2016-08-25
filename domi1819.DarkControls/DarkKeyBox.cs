using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public partial class DarkKeyBox : DarkTextBox
    {
        public Keys Modifiers { get; set; }
        public Keys Key { get; set; }
        
        public DarkKeyBox()
        {
            this.InitializeComponent();

            this.textBox.ReadOnly = true;
            this.textBox.PreviewKeyDown += this.TextBoxOnPreviewKeyDown;
        }

        public void RefreshText()
        {
            if (this.Modifiers > 0 && this.Key > 0)
            {
                this.textBox.Text = $"{this.Modifiers} + {this.Key}";
            }
            else
            {
                this.textBox.Text = "";
            }
        }

        private void TextBoxOnPreviewKeyDown(object o, PreviewKeyDownEventArgs e)
        {
            if (e.Modifiers == 0)
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    this.Modifiers = 0;
                    this.Key = 0;

                    this.Parent.SelectNextControl(this, true, true, true, true);
                }
            }
            else if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                this.Modifiers = e.Modifiers;
                this.Key = e.KeyCode;

                this.Parent.SelectNextControl(this, true, true, true, true);
            }

            this.RefreshText();
        }
    }
}
