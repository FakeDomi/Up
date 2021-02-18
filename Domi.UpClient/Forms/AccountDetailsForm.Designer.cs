using Domi.DarkControls;

namespace Domi.UpClient.Forms
{
    partial class AccountDetailsForm
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountDetailsForm));
            this.uiProgressBar = new DarkProgressBar();
            this.uiLeftLabel = new System.Windows.Forms.Label();
            this.uiRightLabel = new System.Windows.Forms.Label();
            this.uiBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // uiProgressBar
            // 
            this.uiProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiProgressBar.BarColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(255)))));
            this.uiProgressBar.Location = new System.Drawing.Point(12, 116);
            this.uiProgressBar.Name = "uiProgressBar";
            this.uiProgressBar.Size = new System.Drawing.Size(260, 24);
            this.uiProgressBar.TabIndex = 0;
            this.uiProgressBar.Value = 0F;
            // 
            // uiLeftLabel
            // 
            this.uiLeftLabel.Location = new System.Drawing.Point(12, 9);
            this.uiLeftLabel.Name = "uiLeftLabel";
            this.uiLeftLabel.Size = new System.Drawing.Size(110, 104);
            this.uiLeftLabel.TabIndex = 1;
            this.uiLeftLabel.Text = "Number of files stored\r\n\r\nStorage capacity\r\n\r\n\r\nOccupied storage";
            // 
            // uiRightLabel
            // 
            this.uiRightLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiRightLabel.Location = new System.Drawing.Point(128, 9);
            this.uiRightLabel.Name = "uiRightLabel";
            this.uiRightLabel.Size = new System.Drawing.Size(144, 104);
            this.uiRightLabel.TabIndex = 1;
            this.uiRightLabel.Text = "\r\n\r\n\r\nPlease wait, \r\nfetching data...";
            this.uiRightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // uiBackgroundWorker
            // 
            this.uiBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.uiBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
            // 
            // AccountDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 152);
            this.Controls.Add(this.uiRightLabel);
            this.Controls.Add(this.uiLeftLabel);
            this.Controls.Add(this.uiProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Account Details";
            this.ResumeLayout(false);

        }

        #endregion

        private DarkProgressBar uiProgressBar;
        private System.Windows.Forms.Label uiLeftLabel;
        private System.Windows.Forms.Label uiRightLabel;
        private System.ComponentModel.BackgroundWorker uiBackgroundWorker;
    }
}