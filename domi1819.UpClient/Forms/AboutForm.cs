using System.Reflection;
using System.Windows.Forms;
using domi1819.DarkControls;

namespace domi1819.UpClient.Forms
{
    public partial class AboutForm : DarkForm
    {
        public AboutForm()
        {
            this.InitializeComponent();

            this.uiVersionLabel.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
        }

        internal void Restore()
        {
            if (!this.Visible)
            {
                this.Show();
            }
            else
            {
                this.BringToFront();
                this.Activate();
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
