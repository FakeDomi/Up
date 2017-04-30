using System;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Config;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    internal partial class ConfigurationForm : DarkForm
    {
        private readonly UpClient upClient;

        private readonly HotkeyForm hotkeyForm = new HotkeyForm();
        private readonly HotkeyManager hotkeyManager;
        private readonly ContextMenuHandler contextMenuHandler;

        private bool suppressUpdateStyleCheckedChanged;
        private bool suppressScreenshotFormatCheckedChanged;

        public event EventHandler<ColorChangedEventArgs> ThemeColorChanged;

        public ConfigurationForm(UpClient upClient)
        {
            this.InitializeComponent();

            this.upClient = upClient;
            this.hotkeyManager = new HotkeyManager(this, upClient.ActionManager);

            this.ResetFields();

            this.contextMenuHandler = new ContextMenuHandler(upClient);

            this.uiNotifyIcon.ContextMenu = new ContextMenu();
            this.RebuildContextMenu();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312) // WM_HOTKEY
            {
                this.hotkeyManager.ProcessHotkey(m.LParam.ToInt32());
            }

            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                this.ResetFields();
            }
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

        internal void HideTrayIcon()
        {
            this.uiNotifyIcon.Visible = false;
        }

        internal void RebuildContextMenu()
        {
            this.contextMenuHandler.Rebuild(this.uiNotifyIcon.ContextMenu.MenuItems);
        }

        private void ResetFields()
        {
            Config settings = this.upClient.Config;

            this.uiServerAddressTextBox.Text = settings.ServerAddress + (settings.ServerPort == 1819 ? string.Empty : ":" + settings.ServerPort);
            this.uiUserIdTextBox.Text = settings.UserId;
            this.uiPasswordTextBox.Text = settings.Password;

            this.uiLocalCopyCheckBox.Checked = settings.LocalScreenshotCopy;
            this.uiSnapFileAreaCheckBox.Checked = settings.DropArea.Snap;

            if (settings.PngScreenshots)
            {
                this.uiPngFormatCheckBox.Checked = true;
            }
            else
            {
                this.uiJpgFormatCheckBox.Checked = true;
            }

            switch (settings.UpdateBehavior)
            {
                case UpdateBehavior.Indicate:
                    {
                        this.uiIndicateUpdatesCheckBox.Checked = true;
                        break;
                    }
                case UpdateBehavior.NeverCheck:
                    {
                        this.uiNeverCheckForUpdatesCheckBox.Checked = true;
                        break;
                    }
                default:
                    {
                        this.uiAutoInstallUpdatesCheckBox.Checked = true;
                        break;
                    }
            }

            DarkColors.StrongColor = this.upClient.Config.ThemeColor.GetColor();

            this.uiDarkColorView.Color = this.upClient.Config.ThemeColor.GetColor();
            this.DarkColorViewColorSelected(null, null);

            this.hotkeyForm.ResetFields(settings);
            this.hotkeyManager.ReloadHotkeys(settings);
            this.upClient.FileDropForm.Reload();
        }

        private void SaveButtonOnClick(object sender, EventArgs e)
        {
            Config config = this.upClient.Config;

            string oldAddress = config.ServerAddress;
            int oldPort = config.ServerPort;

            if (this.FillConfig(config, true))
            {
                this.contextMenuHandler.Rebuild(this.uiNotifyIcon.ContextMenu.MenuItems);

                if (oldAddress != config.ServerAddress || oldPort != config.ServerPort)
                {
                    NetClient client = this.upClient.NetClient;

                    client.Disconnect();

                    client.Host = config.ServerAddress;
                    client.Port = config.ServerPort;
                }

                config.SaveFile();

                this.CancelButtonOnClick(sender, e);
            }
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            this.Hide();
            this.ResetFields();
        }

        private void NotifyIconOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Restore();
        }

        private void FormatRadioCheckBoxesOnCheckedChanged(object sender, EventArgs e)
        {
            if (this.suppressScreenshotFormatCheckedChanged)
            {
                return;
            }

            this.suppressScreenshotFormatCheckedChanged = true;

            CheckBox senderBox = (CheckBox)sender;

            this.uiPngFormatCheckBox.Checked = false;
            this.uiJpgFormatCheckBox.Checked = false;

            senderBox.Checked = true;

            this.suppressScreenshotFormatCheckedChanged = false;
        }

        private void RadioCheckBoxesOnCheckedChanged(object sender, EventArgs e)
        {
            if (this.suppressUpdateStyleCheckedChanged)
            {
                return;
            }

            this.suppressUpdateStyleCheckedChanged = true;

            CheckBox senderBox = (CheckBox)sender;

            this.uiAutoInstallUpdatesCheckBox.Checked = false;
            this.uiIndicateUpdatesCheckBox.Checked = false;
            this.uiNeverCheckForUpdatesCheckBox.Checked = false;

            senderBox.Checked = true;

            this.suppressUpdateStyleCheckedChanged = false;
        }

        private void VerifyKeyButtonClicked(object sender, EventArgs e)
        {
            Config testConfig = new Config();

            this.FillConfig(testConfig);

            this.AccountDetailsButtonClick(null, e);
        }

        private void AccountDetailsButtonClick(object sender, EventArgs e)
        {
            Config testConfig = new Config();

            this.FillConfig(testConfig);

            new AccountDetailsForm().ShowDetails(testConfig, this.upClient.RsaCache, this, sender == null, this.ThemeColorChanged);
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            this.hotkeyManager.SuspendHotkeys();
            this.hotkeyForm.ShowDialog(this);
            this.hotkeyManager.ActivateHotkeys(this.upClient.Config);
        }

        private void DarkColorViewColorSelected(object sender, EventArgs e)
        {
            DarkColors.StrongColor = this.uiDarkColorView.Color;
            this.ThemeColorChanged?.Invoke(this, new ColorChangedEventArgs(this.uiDarkColorView.Color));
        }

        private bool FillConfig(Config config, bool fillHotkeys = false)
        {
            Address address = Address.Parse(this.uiServerAddressTextBox.Text, Constants.Server.DefaultPort);

            if (address.Equals(Address.Invalid))
            {
                MessageBox.Show("Invalid address!");
                return false;
            }

            config.ServerAddress = address.Host;
            config.ServerPort = address.Port;

            config.UserId = this.uiUserIdTextBox.Text;
            config.Password = this.uiPasswordTextBox.Text;

            config.LocalScreenshotCopy = this.uiLocalCopyCheckBox.Checked;
            config.DropArea.Snap = this.uiSnapFileAreaCheckBox.Checked;
            config.PngScreenshots = this.uiPngFormatCheckBox.Checked;
            config.UpdateBehavior = this.uiIndicateUpdatesCheckBox.Checked ? UpdateBehavior.Indicate : this.uiNeverCheckForUpdatesCheckBox.Checked ? UpdateBehavior.NeverCheck : UpdateBehavior.AutoInstall;
            config.ThemeColor = WrappedColor.Of(this.uiDarkColorView.Color);

            if (fillHotkeys)
            {
                this.hotkeyForm.FillConfig(this.upClient.Config);
            }

            return true;
        }

        private void ChangePasswordButtonClick(object sender, EventArgs e)
        {
            using (ChangePasswordForm changePasswordForm = new ChangePasswordForm(this.uiServerAddressTextBox.Text, this.uiUserIdTextBox.Text, this.uiPasswordTextBox.Text, this.upClient.RsaCache))
            {
                changePasswordForm.ShowDialog(this);

                if (changePasswordForm.NewPassword != null)
                {
                    this.uiPasswordTextBox.Text = changePasswordForm.NewPassword;
                }
            }
        }
    }
}
