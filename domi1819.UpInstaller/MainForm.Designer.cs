namespace domi1819.UpInstaller
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.localVersionTextBox = new domi1819.DarkControls.DarkTextBox();
            this.darkProgressBar1 = new domi1819.DarkControls.DarkProgressBar();
            this.remoteVersionTextBox = new domi1819.DarkControls.DarkTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.installUpdateButton = new domi1819.DarkControls.DarkButton();
            this.SuspendLayout();
            // 
            // localVersionTextBox
            // 
            this.localVersionTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.localVersionTextBox.Location = new System.Drawing.Point(12, 243);
            this.localVersionTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.localVersionTextBox.Name = "localVersionTextBox";
            this.localVersionTextBox.ReadOnly = false;
            this.localVersionTextBox.Size = new System.Drawing.Size(112, 20);
            this.localVersionTextBox.TabIndex = 0;
            this.localVersionTextBox.TextValue = "";
            this.localVersionTextBox.UseSystemPasswordChar = false;
            // 
            // darkProgressBar1
            // 
            this.darkProgressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.darkProgressBar1.BarColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(48)))), ((int)(((byte)(128)))));
            this.darkProgressBar1.Location = new System.Drawing.Point(12, 286);
            this.darkProgressBar1.Name = "darkProgressBar1";
            this.darkProgressBar1.Size = new System.Drawing.Size(232, 24);
            this.darkProgressBar1.TabIndex = 1;
            this.darkProgressBar1.Value = 0F;
            this.darkProgressBar1.ValueInt = 0;
            // 
            // remoteVersionTextBox
            // 
            this.remoteVersionTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.remoteVersionTextBox.Location = new System.Drawing.Point(132, 243);
            this.remoteVersionTextBox.MinimumSize = new System.Drawing.Size(20, 20);
            this.remoteVersionTextBox.Name = "remoteVersionTextBox";
            this.remoteVersionTextBox.ReadOnly = false;
            this.remoteVersionTextBox.Size = new System.Drawing.Size(112, 20);
            this.remoteVersionTextBox.TabIndex = 2;
            this.remoteVersionTextBox.TextValue = "";
            this.remoteVersionTextBox.UseSystemPasswordChar = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local Version";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Remote Version";
            // 
            // installUpdateButton
            // 
            this.installUpdateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.installUpdateButton.FlatAppearance.BorderSize = 0;
            this.installUpdateButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.installUpdateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.installUpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.installUpdateButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.installUpdateButton.Location = new System.Drawing.Point(12, 286);
            this.installUpdateButton.Name = "installUpdateButton";
            this.installUpdateButton.Size = new System.Drawing.Size(232, 24);
            this.installUpdateButton.TabIndex = 4;
            this.installUpdateButton.Text = "Install / Update";
            this.installUpdateButton.UseVisualStyleBackColor = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(256, 322);
            this.Controls.Add(this.installUpdateButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.remoteVersionTextBox);
            this.Controls.Add(this.darkProgressBar1);
            this.Controls.Add(this.localVersionTextBox);
            this.Name = "MainForm";
            this.Text = "Setup / Update";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkControls.DarkTextBox localVersionTextBox;
        private DarkControls.DarkProgressBar darkProgressBar1;
        private DarkControls.DarkTextBox remoteVersionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DarkControls.DarkButton installUpdateButton;
    }
}

