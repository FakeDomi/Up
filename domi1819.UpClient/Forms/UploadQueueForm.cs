using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpClient.Uploads;
using domi1819.UpCore.Utilities;
using domi1819.UpCore.Windows;

namespace domi1819.UpClient.Forms
{
    internal partial class UploadQueueForm : DarkForm
    {
        internal bool KeepVisible { get; set; }

        private int totalItemCount;
        private bool isVisible;

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

        public UploadQueueForm(UpClient upClient)
        {
            this.InitializeComponent();

            Screen screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);

            upClient.ConfigurationForm.ThemeColorChanged += this.ConfigurationFormOnThemeColorChanged;

            this.uiProgressBar.BarColor = DarkPainting.StrongColor;
        }

        internal new void Show()
        {
            this.uiHideTimer.Stop();

            if (!this.isVisible)
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

                this.isVisible = true;
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
            this.isVisible = false;
        }

        internal int RefreshList(IList<UploadItem> items)
        {
            int maxItemCount = Constants.Client.UploadQueueMaxItemCount;
            ListBox.ObjectCollection visualItems = this.uiUploadItemsListBox.Items;

            visualItems.Clear();

            for (int i = 0; i < maxItemCount; i++)
            {
                if (items.Count > maxItemCount && i == maxItemCount - 1)
                {
                    visualItems.Add($"{items.Count - (maxItemCount - 1)} more files");
                }
                else if (i < items.Count)
                {
                    visualItems.Add($"{items[i].FileName}{items[i].FileExtension}");
                }
            }

            this.totalItemCount = items.Count;

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
                case WinConsts.WM_NCHITTEST:
                    m.Result = new IntPtr(WinConsts.HTCLIENT);
                    return;

                case WinConsts.WM_NCACTIVATE:
                    m.WParam = new IntPtr(WinConsts.TRUE);
                    break;

                case WinConsts.WM_SYSCOMMAND:
                    if ((m.WParam.ToInt32() & WinConsts.SC_MASK) == WinConsts.SC_MOVE)
                    {
                        return;
                    }

                    break;
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
            this.uiProgressBar.Value = e.ProgressPercentage / 100F;
            this.uiSpeedLabel.Text = $"{Util.GetByteSizeText((long)e.UserState)}/s";
        }

        private void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            int maxItemCount = Constants.Client.UploadQueueMaxItemCount;

            if (e.Index < 0 || e.Index >= this.totalItemCount || e.Index >= maxItemCount)
            {
                return;
            }

            Font font = e.Font;

            if (e.Index == 0)
            {
                font = new Font(e.Font, FontStyle.Bold);
            }
            else if (this.totalItemCount > maxItemCount && e.Index == maxItemCount - 1)
            {
                font = new Font(e.Font, FontStyle.Italic);
            }

            e.DrawBackground();
            e.Graphics.DrawString(this.uiUploadItemsListBox.Items[e.Index].ToString(), font, new SolidBrush(e.ForeColor), e.Bounds);
            e.DrawFocusRectangle();
        }

        private void HideTimerTick(object sender, EventArgs e)
        {
            this.uiHideTimer.Stop();
            this.Hide();
        }

        private void ConfigurationFormOnThemeColorChanged(object sender, ColorSelectedEventArgs colorChangedEventArgs)
        {
            this.uiProgressBar.BarColor = colorChangedEventArgs.NewColor;
        }
    }
}
