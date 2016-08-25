using System;
using System.ComponentModel;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Config;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    public partial class AccountDetailsForm : DarkForm
    {
        private Form parent;

        public AccountDetailsForm()
        {
            this.InitializeComponent();
        }

        internal void ShowDetails(Config settings, Form parentForm)
        {
            this.parent = parentForm;
            this.uiBackgroundWorker.RunWorkerAsync(settings);
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Config settings = (Config)e.Argument;

            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    e.Result = client.GetStorageInfo();
                }
                else
                {
                    e.Result = new InfoForm("Login failed", "Invalid username / password", 5000);
                }
            }
            catch (Exception ex)
            {
                e.Result = new InfoForm("Error", ex.Message, 5000);
            }

            try
            {
                client.Disconnect();
            }
            catch (Exception)
            {
                // We don't care...
            }
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StorageInfo storageInfo = e.Result as StorageInfo;

            if (storageInfo != null)
            {
                this.uiRightLabel.Text = string.Format("{1}{0}{0}{2}{0}{3} Byte{0}{0}{4}{0}{5} Byte", Environment.NewLine, storageInfo.FileCount, Util.GetByteSizeText(storageInfo.MaxCapacity), storageInfo.MaxCapacity, Util.GetByteSizeText(storageInfo.UsedCapacity), storageInfo.UsedCapacity);
                this.uiProgressBar.Value = (float)storageInfo.UsedCapacity / storageInfo.MaxCapacity;

                this.ShowDialog(this.parent);
            }
            else
            {
                InfoForm infoForm = e.Result as InfoForm;

                if (infoForm != null)
                {
                    infoForm.Show();

                    this.Close();
                    this.Dispose();
                }
            }
        }
    }
}
