using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Windows;

namespace domi1819.UpClient.Forms
{
    internal partial class InfoForm : DarkForm
    {
        internal static void Show(string title, string text, int timeout)
        {
            new InfoForm(title, text, timeout).Show();
        }

        private bool showing;
        private readonly int timeout;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WinConsts.WS_EX_TOPMOST;

                return createParams;
            }
        }

        internal InfoForm(string title, string text, int timeout)
        {
            this.InitializeComponent();

            this.uiTitleLabel.Text = title;
            this.uiTextLabel.Text = text;

            this.uiInfoTimer.Interval = 10;
            this.timeout = timeout;
        }

        internal new void Show()
        {
            this.uiInfoTimer.Start();
        }

        protected override void WndProc(ref Message m)
        {
            switch ((uint)m.Msg)
            {
                case WinConsts.WM_NCHITTEST:
                    m.Result = new IntPtr((int)WinConsts.HTCLIENT);
                    return;

                case WinConsts.WM_NCACTIVATE:
                    m.WParam = new IntPtr((int)WinConsts.TRUE);
                    break;
            }

            base.WndProc(ref m);
        }

        private void InfoTimerTick(object sender, EventArgs e)
        {
            if (!this.showing)
            {
                Message m = new Message { HWnd = this.Handle, Msg = (int)WinConsts.WM_NCACTIVATE };
                
                this.WndProc(ref m);

                this.uiInfoTimer.Stop();

                Screen screen = Screen.FromPoint(this.Location);

                base.Show();
                this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height + 55);

                this.Opacity = 0D;
                this.Refresh();

                for (int c = 10; c > 0; c--)
                {
                    this.Location = new Point(this.Location.X, this.Location.Y - c);
                    this.Opacity += 0.1D;
                    //this.Refresh();
                    Thread.Sleep(10);
                }

                this.showing = true;

                this.uiInfoTimer.Interval = this.timeout;
                this.uiInfoTimer.Start();
            }
            else
            {
                this.uiInfoTimer.Stop();

                for (int c = 1; c <= 10; c++)
                {
                    this.Location = new Point(this.Location.X, this.Location.Y + c);
                    this.Opacity -= 0.1D;
                    //this.Refresh();
                    Thread.Sleep(10);
                }

                this.Close();
                this.Dispose();
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            this.InfoTimerTick(null, null);
        }
    }
}
