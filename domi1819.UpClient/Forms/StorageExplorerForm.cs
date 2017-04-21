using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpClient.StorageExplorer;
using domi1819.UpCore.Network;

namespace domi1819.UpClient.Forms
{
    internal partial class StorageExplorerForm : DarkForm
    {
        private readonly UpClient upClient;

        private readonly FileIconCache icons = new FileIconCache();

        private readonly List<FileItem> files = new List<FileItem>();

        private readonly ToolStripItem itemCopyLink, itemCopyWithNames, itemOpenInBrowser, itemDelete;
        private readonly ContextMenuStrip contextMenu = new ContextMenuStrip();

        private string linkFormat;

        internal StorageExplorerForm(UpClient upClient)
        {
            this.InitializeComponent();

            this.uiDataGridView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.uiDataGridView, new object[] { true });

            this.upClient = upClient;
            
            this.uiDataGridView.DefaultCellStyle.SelectionBackColor = DarkColors.StrongColor;
            this.uiDataGridView.DefaultCellStyle.SelectionForeColor = DarkColors.GetForegroundColor(DarkColors.StrongColor);

            this.upClient.ConfigurationForm.ThemeColorChanged += this.ConfigurationFormOnThemeColorChanged;

            this.itemCopyLink = new ToolStripMenuItem("", null, this.ItemCopyOnClick);
            this.itemCopyWithNames = new ToolStripMenuItem("", null, this.ItemCopyWithNamesOnClick);
            this.itemOpenInBrowser = new ToolStripMenuItem("Open file in browser", null, this.ItemOpenOnClick);
            this.itemDelete = new ToolStripMenuItem("", null, this.ItemDeleteOnClick);

            this.contextMenu.Items.Add(this.itemCopyLink);
            this.contextMenu.Items.Add(this.itemCopyWithNames);
            this.contextMenu.Items.Add(this.itemOpenInBrowser);
            this.contextMenu.Items.Add(this.itemDelete);

            this.uiBackgroundWorker.RunWorkerAsync(upClient.NetClient);
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
            
            GC.Collect(); // TODO
        }

        private void Rebind()
        {
            this.uiDataGridView.DataSource = null;
            this.uiDataGridView.DataSource = this.files;

            this.uiDataGridView.Columns[0].Width = 16;
            this.uiDataGridView.Columns[2].Width = 55;
            this.uiDataGridView.Columns[3].Width = 45;
            this.uiDataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.uiDataGridView.Columns[4].Width = 110;

            this.ResizeColumns();
        }

        private void ResizeColumns()
        {
            int otherColumns = this.uiDataGridView.Columns[0].Width + this.uiDataGridView.Columns[2].Width + this.uiDataGridView.Columns[3].Width + this.uiDataGridView.Columns[4].Width;
            VScrollBar bar = this.uiDataGridView.Controls.OfType<VScrollBar>().First();

            this.uiDataGridView.Columns[1].Width = bar.Visible ? this.uiDataGridView.Width - otherColumns - bar.Width : this.uiDataGridView.Width - otherColumns;
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

                    if (this.uiDataGridView.SelectedRows.Count == 1)
                    {
                        this.itemCopyLink.Text = "Copy download link";
                        this.itemCopyWithNames.Text = "Copy link and file name";
                        this.itemOpenInBrowser.Enabled = true;
                        this.itemDelete.Text = "Delete file";
                    }
                    else
                    {
                        this.itemCopyLink.Text = "Copy download links";
                        this.itemCopyWithNames.Text = "Copy links and file names";
                        this.itemOpenInBrowser.Enabled = false;
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
                column.DefaultCellStyle.SelectionForeColor = DarkColors.GetForegroundColor(DarkColors.StrongColor);
            }

            this.uiDataGridView.Refresh();
        }

        private void uiBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch watch = Stopwatch.StartNew();

            BackgroundWorker worker = (BackgroundWorker)sender;
            NetClient client = (NetClient)e.Argument;

            this.files.Clear();

            Console.WriteLine(watch.Elapsed);

            //try
            //{
                client.ClaimConnectHandle();

            Console.WriteLine(watch.Elapsed);

            this.linkFormat = client.GetLinkFormat();

            Console.WriteLine(watch.Elapsed);
            
            client.Login(this.upClient.Config);

            Console.WriteLine(watch.Elapsed);

            client.ListFiles(this.AddItemCallback, 0, DateTime.MinValue, DateTime.MaxValue, 0, long.MaxValue, "", 0);

            Console.WriteLine(watch.Elapsed);

            worker.ReportProgress(0);

            //}
            //catch (Exception)
            //{
            //    InfoForm.Show("Storage explorer", "Error while fetching file list.", 2500);
            //}
            
            client.ReleaseConnectHandle();
        }

        private bool AddItemCallback(string fileId, string fileName, long fileSize, DateTime updateDate, int downloads)
        {
            this.files.Add(FileItem.Construct(fileId, fileName, fileSize, updateDate, downloads, this.icons));

            return true;
        }
        
        private void uiBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Rebind();
        }

        private void uiBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("complete");
        }

        private void ItemCopyOnClick(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;

            foreach (DataGridViewRow row in this.uiDataGridView.SelectedRows)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.AppendLine();
                }

                builder.Append(string.Format(this.linkFormat, ((FileItem)row.DataBoundItem).Identifier));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void ItemCopyWithNamesOnClick(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;

            foreach (DataGridViewRow row in this.uiDataGridView.SelectedRows)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.AppendLine();
                }

                FileItem item = (FileItem)row.DataBoundItem;

                builder.Append(string.Format(this.linkFormat, item.Identifier));
                builder.Append(" [");
                builder.Append(item.Name);
                builder.Append(']');
            }

            Clipboard.SetText(builder.ToString());
        }

        private void ItemOpenOnClick(object sender, EventArgs e)
        {
            FileItem file = (FileItem)this.uiDataGridView.SelectedRows[0].DataBoundItem;

            Process.Start(string.Format(this.linkFormat, file.Identifier));
        }

        private void ItemDeleteOnClick(object sender, EventArgs e)
        {
        }
    }
}
