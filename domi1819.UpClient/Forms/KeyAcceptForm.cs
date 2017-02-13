using System;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    public partial class KeyAcceptForm : DarkForm
    {
        private readonly string fullRemoteFingerprint, fullLocalFingerprint = "";

        public bool Accept { get; private set; }

        public KeyAcceptForm(string serverAddress, byte[] remoteFingerprint, byte[] localFingerprint = null)
        {
            this.InitializeComponent();

            this.uiServerAddressTextBox.TextValue = serverAddress;

            this.fullRemoteFingerprint = remoteFingerprint.ToHexString(6);
            this.uiRemoteFingerprintTextBox.TextValue = this.fullRemoteFingerprint.Substring(0, 41);

            if (localFingerprint != null)
            {
                this.uiInfoLabel.Text = @"This server's key mismatches the key you have previously accepted.
That could mean that the server has changed its identiy, 
or your connection is being intercepted by an attacker.
Do you want to accept and store the new key?";

                this.fullLocalFingerprint = localFingerprint.ToHexString(6);
                this.uiLocalFingerprintTextBox.TextValue = this.fullLocalFingerprint.Substring(0, 41);
            }
        }

        private void AcceptButtonOnClick(object sender, EventArgs e)
        {
            this.Accept = true;
            this.Close();
            this.Dispose();
        }

        private void DenyButtonOnClick(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void CopyRemoteButtonOnClick(object sender, EventArgs e)
        {
            Clipboard.SetText(this.fullRemoteFingerprint);
        }

        private void CopyLocalButtonOnClick(object sender, EventArgs e)
        {
            Clipboard.SetText(this.fullLocalFingerprint);
        }

    }
}
