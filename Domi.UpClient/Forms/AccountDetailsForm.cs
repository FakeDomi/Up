using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Domi.DarkControls;
using Domi.UpCore.Config;
using Domi.UpCore.Network;
using Domi.UpCore.Utilities;

namespace Domi.UpClient.Forms
{
    public partial class AccountDetailsForm : DarkForm
    {
        private Config config;
        private RsaCache rsaCache;
        private Form parent;
        private bool fetchKey;

        public AccountDetailsForm()
        {
            this.InitializeComponent();
        }

        internal void ShowDetails(Config config, RsaCache rsaCache, Form parentForm, bool fetchKey, EventHandler<ColorSelectedEventArgs> themeColorChanged)
        {
            this.config = config;
            this.rsaCache = rsaCache;
            this.parent = parentForm;
            this.fetchKey = fetchKey;
            this.uiBackgroundWorker.RunWorkerAsync();

            this.uiProgressBar.BarColor = DarkPainting.StrongColor;
            themeColorChanged += (sender, args) => { this.uiProgressBar.BarColor = args.NewColor; };
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            NetClient client = new NetClient(this.config.ServerAddress, this.config.ServerPort, this.rsaCache);

            try
            {
                if (this.fetchKey)
                {
                    e.Result = client.FetchKey();
                }
                else if (client.Connect())
                {
                    if (client.Login(this.config.UserId, this.config.Password))
                    {
                        e.Result = client.GetStorageInfo();
                    }
                    else
                    {
                        e.Result = new InfoForm("Login failed", "Invalid username / password", 5000);
                    }
                }
                else
                {
                    e.Result = new InfoForm("Connection failed!", "Server key not trusted.", 3000);
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
            if (e.Result is StorageInfo)
            {
                StorageInfo storageInfo = (StorageInfo)e.Result;

                this.uiRightLabel.Text = string.Format("{1}{0}{0}{2}{0}{3} Byte{0}{0}{4}{0}{5} Byte", Environment.NewLine, storageInfo.FileCount, Util.GetByteSizeText(storageInfo.MaxCapacity), storageInfo.MaxCapacity, Util.GetByteSizeText(storageInfo.UsedCapacity), storageInfo.UsedCapacity);
                this.uiProgressBar.Value = (float)storageInfo.UsedCapacity / storageInfo.MaxCapacity;

                this.ShowDialog(this.parent);
            }
            else if (e.Result is InfoForm)
            {
                InfoForm infoForm = (InfoForm)e.Result;

                infoForm.Show();

                this.Close();
                this.Dispose();
            }
            else if (e.Result is RsaKey)
            {
                RsaKey rsaKey = (RsaKey)e.Result;
                string serverAddress = $"{this.config.ServerAddress}:{this.config.ServerPort}";

                RsaCache tempRsaCache = new RsaCache(this.rsaCache.Path);

                tempRsaCache.LoadKey(serverAddress);

                if (tempRsaCache.Key?.Fingerprint.SequenceEqual(rsaKey.Fingerprint) ?? false)
                {
                    InfoForm.Show("Key is trusted", "You are already trusting this key.", 3000);
                }
                else
                {
                    KeyAcceptForm feedbackForm = new KeyAcceptForm(serverAddress, rsaKey.Fingerprint, tempRsaCache.Key?.Fingerprint);

                    feedbackForm.ShowDialog(this.parent);

                    if (feedbackForm.Accept)
                    {
                        this.rsaCache.Key = rsaKey;
                        this.rsaCache.ServerAddress = serverAddress;
                        this.rsaCache.SaveKey();

                        InfoForm.Show("Key accepted", "Saved!", 3000);
                    }
                    else
                    {
                        InfoForm.Show("Key denied", "Rip.", 3000);
                    }
                }

                tempRsaCache.Dispose();
            }
        }
    }
}
