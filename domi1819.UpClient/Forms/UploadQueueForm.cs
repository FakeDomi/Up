using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    internal partial class UploadQueueForm : DarkForm
    {
        private readonly UpClient upClient;
        
        internal bool KeepVisible { get; set; }
        internal bool IsVisible { get; set; }
        
        // ReSharper disable once ConvertToAutoProperty
        internal BackgroundWorker BackgroundWorker => this.uiBackgroundWorker;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x0008; // WS_EX_TOPMOST

                return createParams;
            }
        }
        
        public UploadQueueForm(UpClient upClient, UploadManager manager)
        {
            this.InitializeComponent();

            this.upClient = upClient;

            this.BackgroundWorker.DoWork += manager.StartUpload;
            this.BackgroundWorker.RunWorkerCompleted += manager.UploadCompleted;

            Screen screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
            
            this.upClient.ConfigurationForm.ThemeColorChanged += this.ConfigurationFormOnThemeColorChanged;
        }

        internal new void Show()
        {
            this.uiHideTimer.Stop();

            if (!this.IsVisible)
            {
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Width, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height + 55);

                Message m = new Message { HWnd = this.Handle, Msg = 0x0086 };

                this.WndProc(ref m);

                base.Show();
                this.Opacity = 0D;

                for (int c = 10; c > 0; c--)
                {
                    this.Location = new Point(this.Location.X, this.Location.Y - c);
                    this.Opacity += 0.1D;
                    this.Refresh();
                    Thread.Sleep(10);
                }

                this.IsVisible = true;
            }

            if (!this.KeepVisible)
            {
                this.uiHideTimer.Start();
            }
        }

        internal new void Hide()
        {
            this.uiHideTimer.Stop();

            for (int c = 1; c <= 10; c++)
            {
                this.Location = new Point(this.Location.X, this.Location.Y + c);
                this.Opacity -= 0.1D;
                this.Refresh();
                Thread.Sleep(10);
            }

            base.Hide();
            this.IsVisible = false;
        }

        internal int RefreshList(IList<UploadItem> items)
        {
            ListBox.ObjectCollection visualItems = this.listBox1.Items;

            visualItems.Clear();

            for (int i = 0; i < 7; i++)
            {
                if (i == 6 && items.Count > 7)
                {
                    visualItems.Add(items.Count - 6 + " more files");
                }
                else if (i < items.Count)
                {
                    visualItems.Add($"{items[i].FileName}{items[i].FileExtension}");
                }
            }

            return visualItems.Count;
        }

        internal void FitSize(int itemCount)
        {
            this.Height = 139 + itemCount * 13;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Width, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
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
                case 0x0112: // WM_SYSCOMMAND
                {
                    if ((m.WParam.ToInt32() & 0xfff0) == 0xF010) // SC_MOVE
                    {
                        return;
                    }

                    break;
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                this.KeepVisible = false;
                this.Hide();
            }
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.uiProgressBar.ValueInt = e.ProgressPercentage;
            this.uiSpeedLabel.Text = $"{Util.GetByteSizeText((long)e.UserState)}/s";

            if (e.ProgressPercentage == 100 && (long)e.UserState == 0L)
            {
                this.FitSize(this.RefreshList(this.upClient.UploadManager.UploadItems));
            }
        }
        
        private void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            int uploadItemCount = this.upClient.UploadManager.UploadItems.Count;

            if (e.Index < 0 || e.Index >= uploadItemCount || e.Index >= 7)
            {
                return;
            }

            Font font = e.Font;

            if (e.Index == 0)
            {
                font = new Font(e.Font, FontStyle.Bold);
            }
            else if (e.Index == 6 && uploadItemCount > 7)
            {
                font = new Font(e.Font, FontStyle.Italic);
            }

            e.DrawBackground();
            e.Graphics.DrawString(((ListBox)(sender)).Items[e.Index].ToString(), font, new SolidBrush(e.ForeColor), e.Bounds);
            e.DrawFocusRectangle();
        }

        private void HideTimerTick(object sender, EventArgs e)
        {
            this.uiHideTimer.Stop();
            this.Hide();
        }
        private void ConfigurationFormOnThemeColorChanged(object sender, ColorChangedEventArgs colorChangedEventArgs)
        {
            this.uiProgressBar.BarColor = colorChangedEventArgs.NewColor;
        }
    }
}
