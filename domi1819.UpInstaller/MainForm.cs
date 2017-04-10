using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Windows.Forms;
using domi1819.DarkControls;

namespace domi1819.UpInstaller
{
    public partial class MainForm : DarkForm
    {
        private WebClient webClient = new WebClient { CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore)};
        private string downloadPath;

        public MainForm()
        {
            this.InitializeComponent();

            
        }

        private void label1_Click(object sender, System.EventArgs e)
        {

        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (File.Exists("%appdata%\\up\\version"))
                {
                    
                }

                string[] versionInfo = this.webClient.DownloadString("https://up.domi1819.xyz/version").Split(';');

                if (versionInfo.Length == 2)
                {
                    this.remoteVersionTextBox.TextValue = versionInfo[0];
                    this.downloadPath = versionInfo[1];

                    return;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            MessageBox.Show(@"Error while looking for releases!");
            Application.Exit();
        }
    }
}
