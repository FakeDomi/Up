namespace domi1819.UpClient.Forms
{
    partial class KeyAcceptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyAcceptForm));
            this.uiAcceptButton = new domi1819.DarkControls.DarkButton();
            this.uiDenyButton = new domi1819.DarkControls.DarkButton();
            this.uiInfoLabel = new System.Windows.Forms.Label();
            this.uiRemoteFingerprintTextBox = new domi1819.DarkControls.DarkTextBox();
            this.uiRemoteLabel = new System.Windows.Forms.Label();
            this.uiServerAddressTextBox = new domi1819.DarkControls.DarkTextBox();
            this.uiAddressLabel = new System.Windows.Forms.Label();
            this.uiLocalFingerprintTextBox = new domi1819.DarkControls.DarkTextBox();
            this.uiLocalLabel = new System.Windows.Forms.Label();
            this.uiCopyRemoteButton = new domi1819.DarkControls.DarkButton();
            this.uiCopyLocalButton = new domi1819.DarkControls.DarkButton();
            this.SuspendLayout();
            // 
            // uiAcceptButton
            // 
            this.uiAcceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uiAcceptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiAcceptButton.FlatAppearance.BorderSize = 0;
            this.uiAcceptButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiAcceptButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.uiAcceptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiAcceptButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiAcceptButton.Location = new System.Drawing.Point(247, 202);
            this.uiAcceptButton.Name = "uiAcceptButton";
            this.uiAcceptButton.Size = new System.Drawing.Size(100, 23);
            this.uiAcceptButton.TabIndex = 0;
            this.uiAcceptButton.Text = "Accept";
            this.uiAcceptButton.UseVisualStyleBackColor = false;
            this.uiAcceptButton.Click += new System.EventHandler(this.AcceptButtonOnClick);
            // 
            // uiDenyButton
            // 
            this.uiDenyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uiDenyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiDenyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uiDenyButton.FlatAppearance.BorderSize = 0;
            this.uiDenyButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiDenyButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.uiDenyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiDenyButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiDenyButton.Location = new System.Drawing.Point(141, 202);
            this.uiDenyButton.Name = "uiDenyButton";
            this.uiDenyButton.Size = new System.Drawing.Size(100, 23);
            this.uiDenyButton.TabIndex = 1;
            this.uiDenyButton.Text = "Deny";
            this.uiDenyButton.UseVisualStyleBackColor = false;
            this.uiDenyButton.Click += new System.EventHandler(this.DenyButtonOnClick);
            // 
            // uiInfoLabel
            // 
            this.uiInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiInfoLabel.Location = new System.Drawing.Point(12, 9);
            this.uiInfoLabel.Name = "uiInfoLabel";
            this.uiInfoLabel.Size = new System.Drawing.Size(335, 59);
            this.uiInfoLabel.TabIndex = 2;
            this.uiInfoLabel.Text = "You don\'t have this server\'s key saved in your Trust Store.\r\nThere is no guarante" +
    "e that it\'s the server you think it is.\r\nAccept and store this key for this addr" +
    "ess?";
            this.uiInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiRemoteFingerprintTextBox
            // 
            this.uiRemoteFingerprintTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiRemoteFingerprintTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiRemoteFingerprintTextBox.Location = new System.Drawing.Point(15, 128);
            this.uiRemoteFingerprintTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.uiRemoteFingerprintTextBox.Name = "uiRemoteFingerprintTextBox";
            this.uiRemoteFingerprintTextBox.ReadOnly = true;
            this.uiRemoteFingerprintTextBox.Size = new System.Drawing.Size(284, 20);
            this.uiRemoteFingerprintTextBox.TabIndex = 3;
            this.uiRemoteFingerprintTextBox.Text = "001122-334455-667788-99AABB-CCDDEE-FFFFFF";
            this.uiRemoteFingerprintTextBox.UseSystemPasswordChar = false;
            // 
            // uiRemoteLabel
            // 
            this.uiRemoteLabel.AutoSize = true;
            this.uiRemoteLabel.Location = new System.Drawing.Point(12, 112);
            this.uiRemoteLabel.Name = "uiRemoteLabel";
            this.uiRemoteLabel.Size = new System.Drawing.Size(93, 13);
            this.uiRemoteLabel.TabIndex = 5;
            this.uiRemoteLabel.Text = "Remote fingerprint";
            // 
            // uiServerAddressTextBox
            // 
            this.uiServerAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiServerAddressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiServerAddressTextBox.Location = new System.Drawing.Point(15, 89);
            this.uiServerAddressTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.uiServerAddressTextBox.Name = "uiServerAddressTextBox";
            this.uiServerAddressTextBox.ReadOnly = true;
            this.uiServerAddressTextBox.Size = new System.Drawing.Size(332, 20);
            this.uiServerAddressTextBox.TabIndex = 3;
            this.uiServerAddressTextBox.Text = "abc.xyz:1819";
            this.uiServerAddressTextBox.UseSystemPasswordChar = false;
            // 
            // uiAddressLabel
            // 
            this.uiAddressLabel.AutoSize = true;
            this.uiAddressLabel.Location = new System.Drawing.Point(12, 73);
            this.uiAddressLabel.Name = "uiAddressLabel";
            this.uiAddressLabel.Size = new System.Drawing.Size(78, 13);
            this.uiAddressLabel.TabIndex = 5;
            this.uiAddressLabel.Text = "Server address";
            // 
            // uiLocalFingerprintTextBox
            // 
            this.uiLocalFingerprintTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiLocalFingerprintTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiLocalFingerprintTextBox.Location = new System.Drawing.Point(15, 167);
            this.uiLocalFingerprintTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.uiLocalFingerprintTextBox.Name = "uiLocalFingerprintTextBox";
            this.uiLocalFingerprintTextBox.ReadOnly = true;
            this.uiLocalFingerprintTextBox.Size = new System.Drawing.Size(284, 20);
            this.uiLocalFingerprintTextBox.TabIndex = 3;
            this.uiLocalFingerprintTextBox.UseSystemPasswordChar = false;
            // 
            // uiLocalLabel
            // 
            this.uiLocalLabel.AutoSize = true;
            this.uiLocalLabel.Location = new System.Drawing.Point(12, 151);
            this.uiLocalLabel.Name = "uiLocalLabel";
            this.uiLocalLabel.Size = new System.Drawing.Size(82, 13);
            this.uiLocalLabel.TabIndex = 5;
            this.uiLocalLabel.Text = "Local fingerprint";
            // 
            // uiCopyRemoteButton
            // 
            this.uiCopyRemoteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiCopyRemoteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiCopyRemoteButton.FlatAppearance.BorderSize = 0;
            this.uiCopyRemoteButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiCopyRemoteButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.uiCopyRemoteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiCopyRemoteButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiCopyRemoteButton.Location = new System.Drawing.Point(305, 125);
            this.uiCopyRemoteButton.Name = "uiCopyRemoteButton";
            this.uiCopyRemoteButton.Size = new System.Drawing.Size(42, 23);
            this.uiCopyRemoteButton.TabIndex = 6;
            this.uiCopyRemoteButton.Text = "Copy";
            this.uiCopyRemoteButton.UseVisualStyleBackColor = false;
            this.uiCopyRemoteButton.Click += new System.EventHandler(this.CopyRemoteButtonOnClick);
            // 
            // uiCopyLocalButton
            // 
            this.uiCopyLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiCopyLocalButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiCopyLocalButton.FlatAppearance.BorderSize = 0;
            this.uiCopyLocalButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiCopyLocalButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.uiCopyLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiCopyLocalButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiCopyLocalButton.Location = new System.Drawing.Point(305, 164);
            this.uiCopyLocalButton.Name = "uiCopyLocalButton";
            this.uiCopyLocalButton.Size = new System.Drawing.Size(42, 23);
            this.uiCopyLocalButton.TabIndex = 6;
            this.uiCopyLocalButton.Text = "Copy";
            this.uiCopyLocalButton.UseVisualStyleBackColor = false;
            this.uiCopyLocalButton.Click += new System.EventHandler(this.CopyLocalButtonOnClick);
            // 
            // KeyAcceptForm
            // 
            this.AcceptButton = this.uiAcceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uiDenyButton;
            this.ClientSize = new System.Drawing.Size(359, 237);
            this.Controls.Add(this.uiCopyLocalButton);
            this.Controls.Add(this.uiCopyRemoteButton);
            this.Controls.Add(this.uiAddressLabel);
            this.Controls.Add(this.uiServerAddressTextBox);
            this.Controls.Add(this.uiLocalLabel);
            this.Controls.Add(this.uiLocalFingerprintTextBox);
            this.Controls.Add(this.uiRemoteLabel);
            this.Controls.Add(this.uiRemoteFingerprintTextBox);
            this.Controls.Add(this.uiInfoLabel);
            this.Controls.Add(this.uiDenyButton);
            this.Controls.Add(this.uiAcceptButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyAcceptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Verification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkControls.DarkButton uiAcceptButton;
        private DarkControls.DarkButton uiDenyButton;
        private System.Windows.Forms.Label uiInfoLabel;
        private DarkControls.DarkTextBox uiRemoteFingerprintTextBox;
        private System.Windows.Forms.Label uiRemoteLabel;
        private DarkControls.DarkTextBox uiServerAddressTextBox;
        private System.Windows.Forms.Label uiAddressLabel;
        private DarkControls.DarkTextBox uiLocalFingerprintTextBox;
        private System.Windows.Forms.Label uiLocalLabel;
        private DarkControls.DarkButton uiCopyRemoteButton;
        private DarkControls.DarkButton uiCopyLocalButton;
    }
}