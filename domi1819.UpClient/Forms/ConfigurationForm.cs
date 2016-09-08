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
            this.RebuildMenu();
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

        internal void RebuildMenu()
        {
            this.contextMenuHandler.Rebuild(this.uiNotifyIcon.ContextMenu.MenuItems);
        }

        private void ResetFields()
        {
            Config settings = this.upClient.Config;

            this.uiServerAddressTextBox.Text = settings.ServerAddress + (settings.ServerPort == 1819 ? string.Empty : ":" + settings.ServerPort);
            this.uiUserIdTextBox.Text = settings.UserId;
            this.uiPasswordTextBox.Text = settings.Password;
            this.uiCryptoKeyTextBox.Text = settings.KeyFile;

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
            Config settings = this.upClient.Config;
            string[] addressSplit = this.uiServerAddressTextBox.Text.Split(':');

            string newAddress;
            int newPort;

            if (addressSplit.Length == 1)
            {
                newAddress = addressSplit[0];
                newPort = Constants.DefaultPort;
            }
            else if (addressSplit.Length == 2)
            {
                newAddress = addressSplit[0];
                
                if (!int.TryParse(addressSplit[1], out newPort))
                {
                    MessageBox.Show("Invalid address!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Invalid address!");
                return;
            }

            bool connectionDirty = newAddress != settings.ServerAddress || newPort != settings.ServerPort;
            
            settings.ServerAddress = newAddress;
            settings.ServerPort = newPort;

            settings.UserId = this.uiUserIdTextBox.Text;
            settings.Password = this.uiPasswordTextBox.Text;
            settings.KeyFile = this.uiCryptoKeyTextBox.Text;

            settings.LocalScreenshotCopy = this.uiLocalCopyCheckBox.Checked;
            settings.DropArea.Snap = this.uiSnapFileAreaCheckBox.Checked;
            settings.PngScreenshots = this.uiPngFormatCheckBox.Checked;
            settings.UpdateBehavior = this.uiIndicateUpdatesCheckBox.Checked ? UpdateBehavior.Indicate : this.uiNeverCheckForUpdatesCheckBox.Checked ? UpdateBehavior.NeverCheck : UpdateBehavior.AutoInstall;
            settings.ThemeColor = WrappedColor.Of(this.uiDarkColorView.Color);

            this.hotkeyForm.SaveToConfig(this.upClient.Config);
            
            this.contextMenuHandler.Rebuild(this.uiNotifyIcon.ContextMenu.MenuItems);

            if (connectionDirty)
            {
                NetClient client = this.upClient.NetClient;

                client.Disconnect();

                client.Address = newAddress;
                client.Port = newPort;
            }
            
            settings.Save();

            this.CancelButtonOnClick(sender, e);
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

        private void CryptoKeyBrowseButtonClick(object sender, EventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            OpenFileDialog openFileDialog = new OpenFileDialog { InitialDirectory = baseDir };

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                if (fileName.StartsWith(baseDir))
                {
                    fileName = fileName.Substring(baseDir.Length);
                }

                this.uiCryptoKeyTextBox.Text = fileName;
            }
        }
        
        private void AccountDetailsButtonClick(object sender, EventArgs e)
        {
            Config testSettings = new Config();

            string[] addressSplit = this.uiServerAddressTextBox.Text.Split(':');

            if (addressSplit.Length == 1)
            {
                testSettings.ServerAddress = addressSplit[0];
                testSettings.ServerPort = 1819;
            }
            else if (addressSplit.Length == 2)
            {
                testSettings.ServerAddress = addressSplit[0];

                int port;

                if (!int.TryParse(addressSplit[1], out port))
                {
                    MessageBox.Show("Invalid address!");
                    return;
                }

                testSettings.ServerPort = port;
            }
            else
            {
                MessageBox.Show("Invalid address!");
                return;
            }

            testSettings.UserId = this.uiUserIdTextBox.Text;
            testSettings.Password = this.uiPasswordTextBox.Text;
            testSettings.KeyFile = this.uiCryptoKeyTextBox.Text;

            new AccountDetailsForm().ShowDetails(testSettings, this);
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
    }
}
