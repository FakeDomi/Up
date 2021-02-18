using Domi.DarkControls;

namespace Domi.UpClient.Forms
{
    partial class UploadQueueForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadQueueForm));
            this.uiUploadItemsListBox = new System.Windows.Forms.ListBox();
            this.coloredBox1 = new ColoredBox();
            this.uiProgressBar = new DarkProgressBar();
            this.uiSpeedLabel = new System.Windows.Forms.Label();
            this.uiBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.uiHideTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // uiUploadItemsListBox
            // 
            this.uiUploadItemsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiUploadItemsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiUploadItemsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.uiUploadItemsListBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiUploadItemsListBox.FormattingEnabled = true;
            this.uiUploadItemsListBox.Location = new System.Drawing.Point(12, 89);
            this.uiUploadItemsListBox.Name = "uiUploadItemsListBox";
            this.uiUploadItemsListBox.Size = new System.Drawing.Size(190, 91);
            this.uiUploadItemsListBox.TabIndex = 0;
            this.uiUploadItemsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxDrawItem);
            // 
            // coloredBox1
            // 
            this.coloredBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(71)))));
            this.coloredBox1.BorderColor = System.Drawing.Color.Empty;
            this.coloredBox1.Location = new System.Drawing.Point(12, 82);
            this.coloredBox1.Name = "coloredBox1";
            this.coloredBox1.Size = new System.Drawing.Size(190, 1);
            this.coloredBox1.TabIndex = 1;
            // 
            // uiProgressBar
            // 
            this.uiProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiProgressBar.BarColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(255)))));
            this.uiProgressBar.Location = new System.Drawing.Point(12, 25);
            this.uiProgressBar.Name = "uiProgressBar";
            this.uiProgressBar.Size = new System.Drawing.Size(190, 26);
            this.uiProgressBar.TabIndex = 3;
            this.uiProgressBar.Value = 0F;
            // 
            // uiSpeedLabel
            // 
            this.uiSpeedLabel.Location = new System.Drawing.Point(108, 54);
            this.uiSpeedLabel.Name = "uiSpeedLabel";
            this.uiSpeedLabel.Size = new System.Drawing.Size(94, 17);
            this.uiSpeedLabel.TabIndex = 4;
            this.uiSpeedLabel.Text = "▲  0 KB/s";
            this.uiSpeedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // uiBackgroundWorker
            // 
            this.uiBackgroundWorker.WorkerReportsProgress = true;
            this.uiBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            // 
            // uiHideTimer
            // 
            this.uiHideTimer.Interval = 2000;
            this.uiHideTimer.Tick += new System.EventHandler(this.HideTimerTick);
            // 
            // UploadQueueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 192);
            this.Controls.Add(this.uiSpeedLabel);
            this.Controls.Add(this.uiProgressBar);
            this.Controls.Add(this.coloredBox1);
            this.Controls.Add(this.uiUploadItemsListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UploadQueueForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Uploads";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox uiUploadItemsListBox;
        private ColoredBox coloredBox1;
        private DarkProgressBar uiProgressBar;
        private System.Windows.Forms.Label uiSpeedLabel;
        private System.ComponentModel.BackgroundWorker uiBackgroundWorker;
        private System.Windows.Forms.Timer uiHideTimer;

    }
}