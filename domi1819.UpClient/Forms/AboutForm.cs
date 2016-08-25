using System.Reflection;
using domi1819.DarkControls;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    public partial class AboutForm : DarkForm
    {
        public AboutForm()
        {
            this.InitializeComponent();

            this.uiVersionLabel.Text = @"Version " + Assembly.GetExecutingAssembly().GetName().Version;
            this.uiYearLabel.Text = Constants.BuildYear + @" domi1819";
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
    }
}
