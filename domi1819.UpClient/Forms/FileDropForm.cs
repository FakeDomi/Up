using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Config;

namespace domi1819.UpClient.Forms
{
    internal partial class FileDropForm : DarkForm
    {
        private readonly UpClient upClient;
        private Point mouseOffset;

        private readonly ToolStripMenuItem menuItemLock;
        
        internal FileDropForm(UpClient upClient)
        {
            this.InitializeComponent();

            this.upClient = upClient;
            
            this.ContextMenuStrip.Items.Add(this.menuItemLock = new ToolStripMenuItem(upClient.Config.DropArea.Lock ? "Unlock" : "Lock", null, this.LockOnClick));
            this.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Hide", null, this.HideOnClick));
        }

        internal void Reload()
        {
            Config config = this.upClient.Config;

            if (config.DropArea.Show)
            {
                this.Location = config.DropArea.Location;

                if (config.DropArea.Snap)
                {
                    this.Resnap();
                }

                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
        
        internal void Resnap(Point? p = null)
        {
            if (p == null)
            {
                p = new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
            }

            Screen screen = Screen.FromPoint(p.Value);

            if (this.Location.X + this.Width / 2 < screen.WorkingArea.Left + screen.WorkingArea.Width * 0.125F + this.Width * 0.375F)
            {
                this.Location = new Point(screen.WorkingArea.Left, this.Location.Y);
            }
            else if (this.Location.X + this.Width / 2 < screen.WorkingArea.Left + screen.WorkingArea.Width * 0.375F + this.Width * 0.125F)
            {
                this.Location = new Point(screen.WorkingArea.Left - this.Width / 4 + screen.WorkingArea.Width / 4, this.Location.Y);
            }
            else if (this.Location.X + this.Width / 2 < screen.WorkingArea.Left + screen.WorkingArea.Width * 0.625F - this.Width * 0.125F)
            {
                this.Location = new Point(screen.WorkingArea.Left - this.Width / 2 + screen.WorkingArea.Width / 2, this.Location.Y);
            }
            else if (this.Location.X + this.Width / 2 < screen.WorkingArea.Left + screen.WorkingArea.Width * 0.875F - this.Width * 0.375F)
            {
                this.Location = new Point(screen.WorkingArea.Left - this.Width * 3 / 4 + screen.WorkingArea.Width * 3 / 4, this.Location.Y);
            }
            else
            {
                this.Location = new Point(screen.WorkingArea.Right - this.Width, this.Location.Y);
            }

            this.Location = this.Location.Y + this.Height / 2 < screen.WorkingArea.Top + screen.WorkingArea.Height / 2 ? new Point(this.Location.X, screen.WorkingArea.Top) : new Point(this.Location.X, screen.WorkingArea.Bottom - this.Height);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084: // WM_NCHITTEST
                {
                    m.Result = new IntPtr(0x01); // HTCLIENT
                    return;
                }
                case 0x0086: // WM_NCACTIVATE
                {
                    m.WParam = new IntPtr(0x01); // TRUE
                    break;
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.upClient.Config.DropArea.Lock)
            {
                this.mouseOffset = e.Location;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.upClient.Config.DropArea.Lock)
            {
                this.Left = e.X + this.Left - this.mouseOffset.X;
                this.Top = e.Y + this.Top - this.mouseOffset.Y;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.upClient.Config.DropArea.Lock)
            {
                Config config = this.upClient.Config;

                if (config.DropArea.Snap)
                {
                    this.Resnap();
                }

                config.DropArea.Location = this.Location;
                config.SaveFile();
            }

            base.OnMouseUp(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            string[] files = drgevent.Data.GetData(DataFormats.FileDrop) as string[];
            
            if (files == null || files.Any(file => File.GetAttributes(file).HasFlag(FileAttributes.Directory)))
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            drgevent.Effect = DragDropEffects.Copy;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            
            this.upClient.UploadManager.AddItems((string[])drgevent.Data.GetData(DataFormats.FileDrop));
        }

        private void LockOnClick(object o, EventArgs e)
        {
            this.upClient.Config.DropArea.Lock = !this.upClient.Config.DropArea.Lock;
            this.upClient.Config.SaveFile();

            this.menuItemLock.Text = this.upClient.Config.DropArea.Lock ? "Unlock" : "Lock";
        }

        private void HideOnClick(object o, EventArgs e)
        {
            this.upClient.ActionManager.ToggleFileDropArea();
        }
    }
}
