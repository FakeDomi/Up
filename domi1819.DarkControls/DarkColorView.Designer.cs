namespace domi1819.DarkControls
{
    sealed partial class DarkColorView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiColorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uiColorLabel
            // 
            this.uiColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiColorLabel.Location = new System.Drawing.Point(21, 1);
            this.uiColorLabel.Name = "uiColorLabel";
            this.uiColorLabel.Size = new System.Drawing.Size(112, 21);
            this.uiColorLabel.TabIndex = 0;
            this.uiColorLabel.Text = "uiColorLabel";
            this.uiColorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DarkColorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uiColorLabel);
            this.MaximumSize = new System.Drawing.Size(2000000000, 23);
            this.MinimumSize = new System.Drawing.Size(100, 23);
            this.Name = "DarkColorView";
            this.Size = new System.Drawing.Size(150, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label uiColorLabel;
    }
}
