using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using domi1819.DarkControls;
using domi1819.UpCore.Config;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.Forms
{
    internal partial class ScreenshotForm : Form
    {
        private readonly Brush brush = new SolidBrush(Color.FromArgb(80, 128, 128, 128));

        private readonly UpClient upClient;

        private int startX, startY;
        private int drawStartX, drawStartY;
        private int drawEndX, drawEndY;

        private int lastX, lastY;
        private bool drawSelection;
        private bool drawBackground = true;

        private bool finalized;
        private bool localScreenshot;

        private Bitmap backgroundImage;

        //protected override bool ShowWithoutActivation => true;

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams createParams = base.CreateParams;
        //        createParams.ExStyle |= 0x0008; // WS_EX_TOPMOST

        //        return createParams;
        //    }
        //}

        internal ScreenshotForm(UpClient upClient)
        {
            this.upClient = upClient;

            this.InitializeComponent();
        }

        internal void TakeScreenshot(bool fullscreen, bool local, int sleepTime = 0)
        {
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }

            if (this.Visible)
            {
                return;
            }
            
            this.Width = SystemInformation.VirtualScreen.Width;
            this.Height = SystemInformation.VirtualScreen.Height;

            this.Opacity = 1;

            this.drawBackground = !fullscreen;
            this.finalized = false;

            Bitmap bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(this.Width, this.Height));
            }

            this.backgroundImage = bitmap;

            this.Show();
            this.Location = new Point(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top);
            this.Activate();
            
            this.localScreenshot = local;

            if (fullscreen)
            {
                this.Invoke((MethodInvoker)this.SelectFullscreen);
            }
            else
            {
                this.Cursor = Cursors.Cross;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0021) // WM_MOUSEACTIVATE
            {
                m.Result = new IntPtr(0x03); // MA_NOACTIVATE
                return;
            }

            if (m.Msg == 0x0006)
            {
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.drawSelection)
            {
                e.Graphics.FillRectangle(this.brush, 0, 0, this.Width, this.drawStartY);
                e.Graphics.FillRectangle(this.brush, 0, this.drawEndY, this.Width, this.Height - this.drawEndY);

                e.Graphics.FillRectangle(this.brush, 0, this.drawStartY, this.drawStartX, this.drawEndY - this.drawStartY);
                e.Graphics.FillRectangle(this.brush, this.drawEndX, this.drawStartY, this.Width - this.drawEndX, this.drawEndY - this.drawStartY);

                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.drawStartX - 1, this.drawStartY - 1, this.drawEndX - this.drawStartX + 2, this.drawEndY - this.drawStartY + 2), DarkColors.PaleColor, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(this.drawStartX, this.drawStartY, this.drawEndX - this.drawStartX, this.drawEndY - this.drawStartY), DarkColors.StrongColor, ButtonBorderStyle.Solid);
            }
            else if (this.drawBackground)
            {
                e.Graphics.FillRectangle(this.brush, 0, 0, this.Width, this.Height);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right)
            {
                this.FinalizeScreenshot(true);
            }
            else if (e.Button == MouseButtons.Left)
            {
                this.startX = Cursor.Position.X;
                this.startY = Cursor.Position.Y;

                this.drawSelection = true;
                this.uiTimer.Start();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.backgroundImage, 0, 0, this.backgroundImage.Width, this.backgroundImage.Height);
        }

        private void SelectFullscreen()
        {
            this.drawStartX = 0;
            this.drawStartY = 0;
            this.drawEndX = this.Width;
            this.drawEndY = this.Height;

            this.FinalizeScreenshot(false);
        }

        private void FinalizeScreenshot(bool cancel)
        {
            if (this.finalized)
            {
                return;
            }

            this.uiTimer.Stop();
            this.drawSelection = false;
            this.drawBackground = false;

            int areaWidth = this.drawEndX - this.drawStartX;
            int areaHeight = this.drawEndY - this.drawStartY;
            
            if (!cancel && areaWidth > 1 && areaHeight > 1)
            {
                Config settings = this.upClient.Config;

                Bitmap oldBackground = this.backgroundImage;
                Bitmap destImage = oldBackground.Clone(new Rectangle(this.drawStartX, this.drawStartY, areaWidth, areaHeight), oldBackground.PixelFormat);

                this.backgroundImage = destImage;

                this.Refresh();

                this.Location = new Point(this.drawStartX, this.drawStartY);
                this.Width = areaWidth;
                this.Height = areaHeight;

                oldBackground.Dispose();

                for (int c = 1; c <= 10; c++)
                {
                    this.Location = new Point(this.Location.X, this.Location.Y - c);
                    this.Opacity -= 0.1D;
                    Thread.Sleep(12);
                }

                string tempFolderPath = Util.CreateTempFolder();

                DateTime now = DateTime.Now;

                string fileExtension = (settings.PngScreenshots ? ".png" : ".jpeg");
                string fileName = $"ss_{now.Year}-{now.Month.Pad(2)}-{now.Day.Pad(2)}_{now.Hour.Pad(2)}-{now.Minute.Pad(2)}-{now.Second.Pad(2)}";
                string fileFullPath = Path.Combine(tempFolderPath, $"{fileName}{fileExtension}");

                if (settings.PngScreenshots)
                {
                    destImage.Save(fileFullPath, ImageFormat.Png);
                }
                else
                {
                    destImage.Save(fileFullPath, ImageCodecInfo.GetImageEncoders().First(x => x.MimeType.Equals("image/jpeg")), new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, 90L) } });
                }

                if (this.localScreenshot)
                {
                    this.upClient.UploadManager.CleanupTempFile(tempFolderPath, fileName, fileExtension, true);
                }
                else
                {
                    this.upClient.UploadManager.AddItem(new UploadItem { FolderPath = tempFolderPath, FileName = fileName, FileExtension = fileExtension, TemporaryFile = true });
                }
            }
            else
            {
                for (int c = 1; c <= 10; c++)
                {
                    this.Opacity -= 0.1D;
                    Thread.Sleep(12);
                }
            }

            this.Hide();
            this.finalized = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                this.FinalizeScreenshot(false);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                this.FinalizeScreenshot(true);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            int curX = Cursor.Position.X, curY = Cursor.Position.Y;

            if (this.lastX != curX || this.lastY != curY)
            {
                if (this.startX <= curX)
                {
                    this.drawStartX = this.startX;
                    this.drawEndX = curX + 1;
                }
                else
                {
                    this.drawStartX = curX;
                    this.drawEndX = this.startX + 1;
                }

                if (this.startY <= curY)
                {
                    this.drawStartY = this.startY;
                    this.drawEndY = curY + 1;
                }
                else
                {
                    this.drawStartY = curY;
                    this.drawEndY = this.startY + 1;
                }

                this.lastX = curX;
                this.lastY = curY;

                this.Invalidate();
            }
        }
    }
}
