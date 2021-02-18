using Domi.DarkControls;

namespace Domi.UpClient.Forms
{
    partial class InfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoForm));
            this.uiTitleLabel = new System.Windows.Forms.Label();
            this.uiTextLabel = new System.Windows.Forms.Label();
            this.uiInfoTimer = new System.Windows.Forms.Timer(this.components);
            this.uiCloseButton = new DarkButton();
            this.SuspendLayout();
            // 
            // uiTitleLabel
            // 
            this.uiTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTitleLabel.Location = new System.Drawing.Point(79, 7);
            this.uiTitleLabel.Name = "uiTitleLabel";
            this.uiTitleLabel.Size = new System.Drawing.Size(195, 18);
            this.uiTitleLabel.TabIndex = 0;
            this.uiTitleLabel.Text = "Title";
            // 
            // uiTextLabel
            // 
            this.uiTextLabel.Location = new System.Drawing.Point(80, 25);
            this.uiTextLabel.Name = "uiTextLabel";
            this.uiTextLabel.Size = new System.Drawing.Size(217, 43);
            this.uiTextLabel.TabIndex = 1;
            this.uiTextLabel.Text = "Blah.";
            this.uiTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiInfoTimer
            // 
            this.uiInfoTimer.Tick += new System.EventHandler(this.InfoTimerTick);
            // 
            // uiCloseButton
            // 
            this.uiCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.uiCloseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiCloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.uiCloseButton.Location = new System.Drawing.Point(285, 3);
            this.uiCloseButton.Name = "uiCloseButton";
            this.uiCloseButton.Size = new System.Drawing.Size(16, 16);
            this.uiCloseButton.TabIndex = 2;
            this.uiCloseButton.Text = "✖";
            this.uiCloseButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // InfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(304, 74);
            this.ControlBox = false;
            this.Controls.Add(this.uiCloseButton);
            this.Controls.Add(this.uiTextLabel);
            this.Controls.Add(this.uiTitleLabel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "InfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label uiTitleLabel;
        private System.Windows.Forms.Label uiTextLabel;
        private System.Windows.Forms.Timer uiInfoTimer;
        private DarkButton uiCloseButton;
    }
}