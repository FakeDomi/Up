namespace domi1819.UpClient.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.uiTitleLabel = new System.Windows.Forms.Label();
            this.uiVersionLabel = new System.Windows.Forms.Label();
            this.uiWebAddressLabel = new System.Windows.Forms.Label();
            this.uiYearLabel = new System.Windows.Forms.Label();
            this.uiAllRightsReservedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uiTitleLabel
            // 
            this.uiTitleLabel.AutoSize = true;
            this.uiTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTitleLabel.Location = new System.Drawing.Point(148, 22);
            this.uiTitleLabel.Name = "uiTitleLabel";
            this.uiTitleLabel.Size = new System.Drawing.Size(131, 15);
            this.uiTitleLabel.TabIndex = 0;
            this.uiTitleLabel.Text = "Up quick sharing client";
            // 
            // uiVersionLabel
            // 
            this.uiVersionLabel.AutoSize = true;
            this.uiVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiVersionLabel.Location = new System.Drawing.Point(148, 37);
            this.uiVersionLabel.Name = "uiVersionLabel";
            this.uiVersionLabel.Size = new System.Drawing.Size(92, 15);
            this.uiVersionLabel.TabIndex = 0;
            this.uiVersionLabel.Text = "Version X.X.X.X";
            // 
            // uiWebAddressLabel
            // 
            this.uiWebAddressLabel.AutoSize = true;
            this.uiWebAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiWebAddressLabel.Location = new System.Drawing.Point(148, 52);
            this.uiWebAddressLabel.Name = "uiWebAddressLabel";
            this.uiWebAddressLabel.Size = new System.Drawing.Size(101, 15);
            this.uiWebAddressLabel.TabIndex = 0;
            this.uiWebAddressLabel.Text = "https://up.domi.re";
            // 
            // uiYearLabel
            // 
            this.uiYearLabel.AutoSize = true;
            this.uiYearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiYearLabel.Location = new System.Drawing.Point(148, 67);
            this.uiYearLabel.Name = "uiYearLabel";
            this.uiYearLabel.Size = new System.Drawing.Size(132, 15);
            this.uiYearLabel.TabIndex = 0;
            this.uiYearLabel.Text = "2015 - 2019 domi1819";
            // 
            // uiAllRightsReservedLabel
            // 
            this.uiAllRightsReservedLabel.AutoSize = true;
            this.uiAllRightsReservedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiAllRightsReservedLabel.Location = new System.Drawing.Point(148, 82);
            this.uiAllRightsReservedLabel.Name = "uiAllRightsReservedLabel";
            this.uiAllRightsReservedLabel.Size = new System.Drawing.Size(116, 15);
            this.uiAllRightsReservedLabel.TabIndex = 0;
            this.uiAllRightsReservedLabel.Text = "Licensed under MIT";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(309, 122);
            this.Controls.Add(this.uiAllRightsReservedLabel);
            this.Controls.Add(this.uiYearLabel);
            this.Controls.Add(this.uiWebAddressLabel);
            this.Controls.Add(this.uiVersionLabel);
            this.Controls.Add(this.uiTitleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label uiTitleLabel;
        private System.Windows.Forms.Label uiVersionLabel;
        private System.Windows.Forms.Label uiWebAddressLabel;
        private System.Windows.Forms.Label uiYearLabel;
        private System.Windows.Forms.Label uiAllRightsReservedLabel;
    }
}