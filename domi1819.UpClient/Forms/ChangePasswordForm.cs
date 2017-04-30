using System;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    public partial class ChangePasswordForm : DarkForm
    {
        private readonly RsaCache rsaCache;

        internal string NewPassword { get; private set; }

        public ChangePasswordForm(string serverAddress, string userId, string currentPassword, RsaCache rsaCache)
        {
            this.InitializeComponent();

            this.serverAddressTextBox.Text = serverAddress;
            this.userIdTextBox.Text = userId;
            this.currentPasswordTextBox.Text = currentPassword;

            this.rsaCache = rsaCache;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            Address address = Address.Parse(this.serverAddressTextBox.Text, Constants.Server.DefaultPort);

            if (address.Equals(Address.Invalid))
            {
                MessageBox.Show("Invalid address");
                return;
            }

            NetClient client = new NetClient(address.Host, address.Port, this.rsaCache);
            InfoForm result;

            try
            {
                if (client.Connect())
                {
                    if (client.Login(this.userIdTextBox.Text, this.currentPasswordTextBox.Text))
                    {
                        if (client.SetPassword(this.newPasswordTextBox.Text))
                        {
                            this.NewPassword = this.newPasswordTextBox.Text;
                            result = InfoFormDefaults.PasswordChanged;
                        }
                        else
                        {
                            result = InfoFormDefaults.NewPasswordRejected;
                        }
                    }
                    else
                    {
                        result = InfoFormDefaults.LoginFailed;
                    }
                }
                else
                {
                    result = InfoFormDefaults.ServerNotTrusted;
                }
            }
            catch (Exception exception)
            {
                result = new InfoForm("Network error!", exception.Message, Constants.Client.InfoErrorTimeout);
            }

            result.Show();

            if (this.NewPassword != null)
            {
                this.Close();
            }

            client.Disconnect();
        }

        private void HidePasswordCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.newPasswordTextBox.UseSystemPasswordChar = this.hidePasswordCheckBox.Checked;
        }
    }
}
