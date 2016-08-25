using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpClient.StorageExplorer;

namespace domi1819.UpClient.Forms
{
    internal partial class StorageExplorerForm : DarkForm
    {
        private readonly UpClient upClient;

        private readonly FileIconCache icons = new FileIconCache();

        private readonly BindingList<FileItem> files = new BindingList<FileItem>();

        private readonly ToolStripItem itemCopyLink, itemCopyWithNames, itemOpenInBrowser, itemDelete;
        private readonly ContextMenuStrip contextMenu = new ContextMenuStrip();

        internal StorageExplorerForm(UpClient upClient)
        {
            this.InitializeComponent();

            this.uiDataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.uiDataGridView, new object[] { true });

            this.upClient = upClient;

            this.files.Add(new FileItem("aabbccdd", this.icons[".exe"], "runme.exe", "2.4 KB", "2016-08-22 13:37"));

            this.uiDataGridView.DataSource = this.files;

            this.uiDataGridView.Columns[0].Width = 16;
            this.uiDataGridView.Columns[2].Width = 55;
            this.uiDataGridView.Columns[3].Width = 110;
            
            this.ResizeColumns();

            this.uiDataGridView.DefaultCellStyle.SelectionBackColor = DarkColors.StrongColor;

            this.upClient.ConfigurationForm.ThemeColorChanged += this.ConfigurationFormOnThemeColorChanged;

            this.itemCopyLink = new ToolStripMenuItem("", null, this.ItemCopyOnClick);
            this.itemCopyWithNames = new ToolStripMenuItem("", null, this.ItemCopyWithNamesOnClick);
            this.itemOpenInBrowser = new ToolStripMenuItem("Open file in browser", null, this.ItemOpenOnClick);
            this.itemDelete = new ToolStripMenuItem("", null, this.ItemDeleteOnClick);

            this.contextMenu.Items.Add(this.itemCopyLink);
            this.contextMenu.Items.Add(this.itemCopyWithNames);
            this.contextMenu.Items.Add(this.itemOpenInBrowser);
            this.contextMenu.Items.Add(this.itemDelete);
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.uiDataGridView.Columns.Count > 1)
            {
                this.ResizeColumns();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            this.upClient.ConfigurationForm.ThemeColorChanged -= this.ConfigurationFormOnThemeColorChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            GC.Collect();
        }

        private void ResizeColumns()
        {
            int otherColumns = this.uiDataGridView.Columns[0].Width + this.uiDataGridView.Columns[2].Width + this.uiDataGridView.Columns[3].Width;
            VScrollBar bar = this.uiDataGridView.Controls.OfType<VScrollBar>().First();

            if (bar.Visible)
            {
                this.uiDataGridView.Columns[1].Width = this.uiDataGridView.Width - otherColumns - bar.Width;
            }
            else
            {
                this.uiDataGridView.Columns[1].Width = this.uiDataGridView.Width - otherColumns;
            }
        }

        private void uiDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hit = this.uiDataGridView.HitTest(e.X, e.Y);

                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0)
                {
                    DataGridViewRow row = this.uiDataGridView.Rows[hit.RowIndex];

                    if (!row.Selected)
                    {
                        this.uiDataGridView.ClearSelection();
                        row.Selected = true;
                    }

                    if (this.uiDataGridView.SelectedRows.Count > 1)
                    {
                        this.itemCopyLink.Text = "Copy download link";
                        this.itemCopyWithNames.Text = "Copy link and file name";
                        this.itemOpenInBrowser.Enabled = false;
                        this.itemDelete.Text = "Delete file";
                    }
                    else
                    {
                        this.itemCopyLink.Text = "Copy download links";
                        this.itemCopyWithNames.Text = "Copy links and file names";
                        this.itemOpenInBrowser.Enabled = true;
                        this.itemDelete.Text = "Delete files";
                    }

                    this.contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void ConfigurationFormOnThemeColorChanged(object sender, ColorChangedEventArgs colorChangedEventArgs)
        {
            foreach (DataGridViewColumn column in this.uiDataGridView.Columns)
            {
                column.DefaultCellStyle.SelectionBackColor = DarkColors.StrongColor;
            }

            this.uiDataGridView.Refresh();
        }

        private void ItemCopyOnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void uiBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void uiBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void ItemCopyWithNamesOnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ItemOpenOnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ItemDeleteOnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
