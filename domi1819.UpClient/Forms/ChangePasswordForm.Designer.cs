namespace domi1819.UpClient.Forms
{
    partial class ChangePasswordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
            this.currentPasswordTextBox = new domi1819.DarkControls.DarkTextBox();
            this.currentPasswordLabel = new System.Windows.Forms.Label();
            this.userIdTextBox = new domi1819.DarkControls.DarkTextBox();
            this.userIdLabel = new System.Windows.Forms.Label();
            this.serverAddressTextBox = new domi1819.DarkControls.DarkTextBox();
            this.serverAddressLabel = new System.Windows.Forms.Label();
            this.newPasswordTextBox = new domi1819.DarkControls.DarkTextBox();
            this.newPasswordLabel = new System.Windows.Forms.Label();
            this.separator = new domi1819.DarkControls.ColoredBox();
            this.saveButton = new domi1819.DarkControls.DarkButton();
            this.hidePasswordCheckBox = new domi1819.DarkControls.DarkCheckBox();
            this.SuspendLayout();
            // 
            // currentPasswordTextBox
            // 
            this.currentPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.currentPasswordTextBox.Location = new System.Drawing.Point(107, 64);
            this.currentPasswordTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.currentPasswordTextBox.Name = "currentPasswordTextBox";
            this.currentPasswordTextBox.ReadOnly = false;
            this.currentPasswordTextBox.Size = new System.Drawing.Size(154, 20);
            this.currentPasswordTextBox.TabIndex = 5;
            this.currentPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // currentPasswordLabel
            // 
            this.currentPasswordLabel.AutoSize = true;
            this.currentPasswordLabel.Location = new System.Drawing.Point(12, 68);
            this.currentPasswordLabel.Name = "currentPasswordLabel";
            this.currentPasswordLabel.Size = new System.Drawing.Size(89, 13);
            this.currentPasswordLabel.TabIndex = 10;
            this.currentPasswordLabel.Text = "Current password";
            // 
            // userIdTextBox
            // 
            this.userIdTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.userIdTextBox.Location = new System.Drawing.Point(107, 38);
            this.userIdTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.userIdTextBox.Name = "userIdTextBox";
            this.userIdTextBox.ReadOnly = true;
            this.userIdTextBox.Size = new System.Drawing.Size(154, 20);
            this.userIdTextBox.TabIndex = 4;
            this.userIdTextBox.UseSystemPasswordChar = false;
            // 
            // userIdLabel
            // 
            this.userIdLabel.AutoSize = true;
            this.userIdLabel.Location = new System.Drawing.Point(12, 42);
            this.userIdLabel.Name = "userIdLabel";
            this.userIdLabel.Size = new System.Drawing.Size(43, 13);
            this.userIdLabel.TabIndex = 8;
            this.userIdLabel.Text = "User ID";
            // 
            // serverAddressTextBox
            // 
            this.serverAddressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.serverAddressTextBox.Location = new System.Drawing.Point(107, 12);
            this.serverAddressTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.serverAddressTextBox.Name = "serverAddressTextBox";
            this.serverAddressTextBox.ReadOnly = true;
            this.serverAddressTextBox.Size = new System.Drawing.Size(154, 20);
            this.serverAddressTextBox.TabIndex = 3;
            this.serverAddressTextBox.UseSystemPasswordChar = false;
            // 
            // serverAddressLabel
            // 
            this.serverAddressLabel.AutoSize = true;
            this.serverAddressLabel.Location = new System.Drawing.Point(12, 16);
            this.serverAddressLabel.Name = "serverAddressLabel";
            this.serverAddressLabel.Size = new System.Drawing.Size(38, 13);
            this.serverAddressLabel.TabIndex = 5;
            this.serverAddressLabel.Text = "Server";
            // 
            // newPasswordTextBox
            // 
            this.newPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.newPasswordTextBox.Location = new System.Drawing.Point(107, 97);
            this.newPasswordTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.newPasswordTextBox.Name = "newPasswordTextBox";
            this.newPasswordTextBox.ReadOnly = false;
            this.newPasswordTextBox.Size = new System.Drawing.Size(154, 20);
            this.newPasswordTextBox.TabIndex = 0;
            this.newPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // newPasswordLabel
            // 
            this.newPasswordLabel.AutoSize = true;
            this.newPasswordLabel.Location = new System.Drawing.Point(12, 101);
            this.newPasswordLabel.Name = "newPasswordLabel";
            this.newPasswordLabel.Size = new System.Drawing.Size(77, 13);
            this.newPasswordLabel.TabIndex = 12;
            this.newPasswordLabel.Text = "New password";
            // 
            // separator
            // 
            this.separator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(71)))));
            this.separator.Location = new System.Drawing.Point(12, 90);
            this.separator.Name = "separator";
            this.separator.Size = new System.Drawing.Size(249, 1);
            this.separator.TabIndex = 13;
            this.separator.TabStop = false;
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.saveButton.Location = new System.Drawing.Point(12, 154);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(249, 23);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Change password";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonClick);
            // 
            // hidePasswordCheckBox
            // 
            this.hidePasswordCheckBox.AutoSize = true;
            this.hidePasswordCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.hidePasswordCheckBox.Checked = true;
            this.hidePasswordCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hidePasswordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hidePasswordCheckBox.Location = new System.Drawing.Point(107, 121);
            this.hidePasswordCheckBox.Name = "hidePasswordCheckBox";
            this.hidePasswordCheckBox.RadioStyle = false;
            this.hidePasswordCheckBox.Size = new System.Drawing.Size(93, 17);
            this.hidePasswordCheckBox.TabIndex = 1;
            this.hidePasswordCheckBox.Text = "Hide password";
            this.hidePasswordCheckBox.UseVisualStyleBackColor = false;
            this.hidePasswordCheckBox.CheckedChanged += new System.EventHandler(this.HidePasswordCheckBoxCheckedChanged);
            // 
            // ChangePasswordForm
            // 
            this.AcceptButton = this.saveButton;
            this.ClientSize = new System.Drawing.Size(273, 189);
            this.Controls.Add(this.hidePasswordCheckBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.separator);
            this.Controls.Add(this.newPasswordTextBox);
            this.Controls.Add(this.newPasswordLabel);
            this.Controls.Add(this.currentPasswordTextBox);
            this.Controls.Add(this.currentPasswordLabel);
            this.Controls.Add(this.userIdTextBox);
            this.Controls.Add(this.userIdLabel);
            this.Controls.Add(this.serverAddressTextBox);
            this.Controls.Add(this.serverAddressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ChangePasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkControls.DarkTextBox currentPasswordTextBox;
        private System.Windows.Forms.Label currentPasswordLabel;
        private DarkControls.DarkTextBox userIdTextBox;
        private System.Windows.Forms.Label userIdLabel;
        private DarkControls.DarkTextBox serverAddressTextBox;
        private System.Windows.Forms.Label serverAddressLabel;
        private DarkControls.DarkTextBox newPasswordTextBox;
        private System.Windows.Forms.Label newPasswordLabel;
        private DarkControls.ColoredBox separator;
        private DarkControls.DarkButton saveButton;
        private DarkControls.DarkCheckBox hidePasswordCheckBox;
    }
}
