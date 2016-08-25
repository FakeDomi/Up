using System.Windows.Forms;
using domi1819.UpClient.StorageExplorer;

namespace domi1819.UpClient.Forms
{
    partial class StorageExplorerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageExplorerForm));
            this.uiDataGridView = new System.Windows.Forms.DataGridView();
            this.uiBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // uiDataGridView
            // 
            this.uiDataGridView.AllowUserToAddRows = false;
            this.uiDataGridView.AllowUserToDeleteRows = false;
            this.uiDataGridView.AllowUserToResizeColumns = false;
            this.uiDataGridView.AllowUserToResizeRows = false;
            this.uiDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.uiDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.uiDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.uiDataGridView.ColumnHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.uiDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.uiDataGridView.Location = new System.Drawing.Point(-1, -1);
            this.uiDataGridView.Name = "uiDataGridView";
            this.uiDataGridView.ReadOnly = true;
            this.uiDataGridView.RowHeadersVisible = false;
            this.uiDataGridView.RowTemplate.Height = 17;
            this.uiDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uiDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.uiDataGridView.ShowCellToolTips = false;
            this.uiDataGridView.Size = new System.Drawing.Size(586, 364);
            this.uiDataGridView.TabIndex = 0;
            this.uiDataGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.uiDataGridView_MouseClick);
            // 
            // uiBackgroundWorker
            // 
            this.uiBackgroundWorker.WorkerReportsProgress = true;
            this.uiBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.uiBackgroundWorker_DoWork);
            this.uiBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.uiBackgroundWorker_ProgressChanged);
            // 
            // StorageExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.uiDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "StorageExplorerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Storage Explorer";
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView uiDataGridView;
        private System.ComponentModel.BackgroundWorker uiBackgroundWorker;
    }
}