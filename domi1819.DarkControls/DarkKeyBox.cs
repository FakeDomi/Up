using System.Windows.Forms;

namespace domi1819.DarkControls
{
    public partial class DarkKeyBox : DarkTextBox
    {
        public Keys Modifiers { get; private set; }

        public Keys Key { get; private set; }

        public DarkKeyBox()
        {
            this.InitializeComponent();

            this.textBox.ReadOnly = true;
            this.textBox.PreviewKeyDown += this.TextBoxOnPreviewKeyDown;
        }

        public void Set(Keys modifiers, Keys key)
        {
            this.Modifiers = modifiers;
            this.Key = key;

            this.RefreshText();
        }

        private void TextBoxOnPreviewKeyDown(object o, PreviewKeyDownEventArgs e)
        {
            if (e.Modifiers == 0)
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    this.Modifiers = 0;
                    this.Key = 0;

                    this.RefreshText();
                    this.Parent.SelectNextControl(this, true, true, true, true);
                }
            }
            else if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                this.Modifiers = e.Modifiers;
                this.Key = e.KeyCode;

                this.RefreshText();
                this.Parent.SelectNextControl(this, true, true, true, true);
            }
        }

        private void RefreshText()
        {
            if (this.Modifiers > 0 && this.Key > 0)
            {
                this.textBox.Text = $"{this.Modifiers} + {this.Key}";
                this.textBox.ForeColor = DarkColors.Foreground;
            }
            else
            {
                this.textBox.Text = "No Hotkey";
                this.textBox.ForeColor = DarkColors.ForegroundInactive;
            }
        }
    }
}
