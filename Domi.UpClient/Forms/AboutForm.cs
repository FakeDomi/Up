using System.Reflection;
using System.Windows.Forms;
using Domi.DarkControls;

namespace Domi.UpClient.Forms
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
