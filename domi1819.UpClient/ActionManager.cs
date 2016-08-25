using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient
{
    internal class ActionManager
    {
        private readonly UpClient upClient;
        private readonly OpenFileDialog openFileDialog = new OpenFileDialog { Multiselect = true };

        internal ActionManager(UpClient upClient)
        {
            this.upClient = upClient;
        }

        internal void Update()
        {
        }

        internal void UploadFile()
        {
            this.openFileDialog.FileName = "";

            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.upClient.UploadManager.AddItems(this.openFileDialog.FileNames);
            }
        }

        internal void UploadScreenshot(bool fullscreen, bool local, int timeout = 0)
        {
            this.upClient.ScreenshotForm.TakeScreenshot(fullscreen, local, timeout);
        }

        internal void UploadClipboard(bool local = false)
        {
            if (Clipboard.ContainsImage() || Clipboard.ContainsText())
            {
                DateTime now = DateTime.Now;

                string tempFolderPath = Util.CreateTempFolder();
                string fileName = $"clip_{now.Year}-{now.Month.Pad(2)}-{now.Day.Pad(2)}_{now.Hour.Pad(2)}-{now.Minute.Pad(2)}-{now.Second.Pad(2)}";
                string fileExt = Clipboard.ContainsImage() ? ".png" : ".txt";
                string fileFullPath = Path.Combine(tempFolderPath, $"{fileName}{fileExt}");

                if (Clipboard.ContainsImage())
                {
                    Image image = Clipboard.GetImage();

                    if (image == null)
                    {
                        Directory.Delete(tempFolderPath);

                        return;
                    }

                    image.Save(fileFullPath);
                }
                else if (Clipboard.ContainsText())
                {
                    using (StreamWriter writer = new StreamWriter(fileFullPath))
                    {
                        writer.Write(Clipboard.GetText());
                    }
                }

                if (local)
                {
                    this.upClient.UploadManager.CleanupTempFile(tempFolderPath, fileName, fileExt, true);
                }
                else
                {
                    this.upClient.UploadManager.AddItem(new UploadItem { FolderPath = tempFolderPath, FileName = fileName, FileExtension = fileExt, TemporaryFile = true });
                }
            }
        }

        //internal void UploadWindow()
        //{
        //    Thread.Sleep(350);

        //    IntPtr topWindow = User32.GetForegroundWindow();
        //    User32.RECT rect = new User32.RECT();

        //    User32.GetWindowRect(topWindow, ref rect);

        //    int width = rect.Right - rect.Left;
        //    int height = rect.Bottom - rect.Top;

        //    if (width > 0 && height > 0)
        //    {
        //        string tempFolderPath;

        //        do
        //        {
        //            tempFolderPath = Path.Combine(Path.GetTempPath(), $"up_{Util.GetRandomString(4)}");
        //        } while (Directory.Exists(tempFolderPath));

        //        Directory.CreateDirectory(tempFolderPath);

        //        DateTime now = DateTime.Now;

        //        string fileExtension = (this.upClient.Config.PngScreenshots ? ".png" : ".jpeg");
        //        string fileName = $"ss_{now.Year}-{now.Month.Pad(2)}-{now.Day.Pad(2)}_{now.Hour.Pad(2)}-{now.Minute.Pad(2)}-{now.Second.Pad(2)}";
        //        string fileFullPath = Path.Combine(tempFolderPath, $"{fileName}{fileExtension}");

        //        using (Bitmap bitmap = new Bitmap(width, height))
        //        using (Graphics graphics = Graphics.FromImage(bitmap))
        //        {
        //            User32.PrintWindow(topWindow, graphics.GetHdc(), 0);
        //            graphics.ReleaseHdc();

        //            if (this.upClient.Config.PngScreenshots)
        //            {
        //                bitmap.Save(fileFullPath, ImageFormat.Png);
        //            }
        //            else
        //            {
        //                bitmap.Save(fileFullPath, ImageCodecInfo.GetImageEncoders().First(x => x.MimeType.Equals("image/jpeg")), new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, 90L) } });
        //            }
        //        }

        //        this.upClient.UploadManager.AddItem(new UploadItem { FolderPath = tempFolderPath, FileName = fileName, FileExtension = fileExtension, TemporaryFile = true });
        //    }
        //}

        internal void ToggleFileDropArea()
        {
            this.upClient.Config.DropArea.Show = !this.upClient.Config.DropArea.Show;
            this.upClient.Config.Save();

            this.upClient.FileDropForm.Reload();
            this.upClient.ConfigurationForm.RebuildMenu();
        }

        internal void ShowFiles()
        {
            this.upClient.StorageExplorerForm.Show();

            //if (m.fileExplorer == null)
            //    m.fileExplorer = new Files();

            //if (!m.fileExplorer.Visible)
            //    m.fileExplorer.Show();
            //else if (m.fileExplorer.WindowState == FormWindowState.Minimized)
            //    m.fileExplorer.WindowState = FormWindowState.Normal;
            //else
            //    m.fileExplorer.BringToFront();

            //m.fileExplorer.dataGridView1.Visible = false;
            //m.fileExplorer.Refresh();

            //m.fileExplorer.RefreshList(m.textBox1.Text, m.textBox2.Text, m.textBox3.Text, 0);
        }

        internal void ShowConfiguration()
        {
            this.upClient.ConfigurationForm.Restore();
        }

        internal void ShowInfo()
        {
            this.upClient.AboutForm.Restore();
        }

        internal void Exit()
        {
            this.upClient.ConfigurationForm.HideTrayIcon();
            Application.Exit();
        }
    }
}
