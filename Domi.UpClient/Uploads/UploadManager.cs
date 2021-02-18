using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Domi.UpClient.Forms;
using Domi.UpCore.Config;
using Domi.UpCore.Network;
using Domi.UpCore.Utilities;
using Domi.UpCore.Windows;

namespace Domi.UpClient.Uploads
{
    internal class UploadManager
    {
        private readonly Config config;
        private readonly NetClient netClient;

        private readonly UploadQueueForm queueForm;

        private readonly List<UploadItem> uploadItems = new List<UploadItem>();

        internal UploadManager(UpClient upClient)
        {
            this.config = upClient.Config;
            this.netClient = upClient.NetClient;

            this.queueForm = new UploadQueueForm(upClient);
            this.queueForm.CreateHandle();

            this.queueForm.BackgroundWorker.DoWork += this.StartUpload;
            this.queueForm.BackgroundWorker.RunWorkerCompleted += this.UploadCompleted;
        }

        internal void AddItem(UploadItem item)
        {
            lock (this.uploadItems)
            {
                this.uploadItems.Add(item);
            }

            this.Refresh();
        }

        internal void AddItems(IEnumerable<string> paths)
        {
            lock (this.uploadItems)
            {
                foreach (string path in paths)
                {
                    this.uploadItems.Add(new UploadItem(path));
                }
            }

            this.Refresh();
        }

        internal void Invoke(Action e, bool isBackgroundThread)
        {
            if (isBackgroundThread)
            {
                this.queueForm.Invoke(e);
            }
            else
            {
                e.Invoke();
            }
        }

        private void Refresh()
        {
            this.queueForm.FitSize(this.queueForm.RefreshList(this.uploadItems));

            if (!this.queueForm.BackgroundWorker.IsBusy)
            {
                this.queueForm.KeepVisible = true;
                this.queueForm.BackgroundWorker.RunWorkerAsync();
                this.queueForm.BackgroundWorkerProgressChanged(this, new ProgressChangedEventArgs(0, 0L));
            }

            this.queueForm.Show();
        }

        private void StartUpload(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            try
            {
                if (!this.netClient.ClaimConnectHandle())
                {
                    args.Result = new UploadResult { Title = "Connection failed!", Message = "Key is not trusted or has changed." };
                    return;
                }
            }
            catch (SocketException ex)
            {
                args.Result = new UploadResult { Title = "Connection failed!", Message = ex.Message };
                return;
            }

            args.Result = this.netClient.Login(this.config.UserId, this.config.Password) ? this.Upload(worker, this.netClient) : new UploadResult { Title = "Login failed!", Message = "Please check your account settings." };

            this.netClient.ReleaseConnectHandle();
        }

        private UploadResult Upload(BackgroundWorker worker, NetClient client)
        {
            byte[] fileBuf = new byte[4096];
            UploadResult result = new UploadResult();

            while (this.uploadItems.Count > 0)
            {
                UploadItem item;

                lock (this.uploadItems)
                {
                    item = this.uploadItems[0];
                }

                string file = Path.Combine(item.FolderPath, $"{item.FileName}{item.FileExtension}");

                if (File.Exists(file))
                {
                    string transferKey = client.InitiateUpload_old(Path.GetFileName(file), new FileInfo(file).Length);

                    if (!string.IsNullOrEmpty(transferKey))
                    {
                        using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            int currentRead;
                            long totalRead = 0;

                            Stopwatch stopwatch = Stopwatch.StartNew();
                            int lastUpdate = 0;

                            worker.ReportProgress(0, 0L);

                            while ((currentRead = stream.Read(fileBuf, 0, (int)Math.Min(fileBuf.Length, stream.Length - totalRead))) > 0)
                            {
                                client.UploadPacket(transferKey, fileBuf, 0, currentRead);

                                totalRead += currentRead;

                                if (stopwatch.ElapsedMilliseconds / 100 >= lastUpdate)
                                {
                                    lastUpdate = (int)(stopwatch.ElapsedMilliseconds / 100);

                                    worker.ReportProgress((int)(100 * totalRead / stream.Length), (long?)(1000 * totalRead / (stopwatch.ElapsedMilliseconds + 1)));
                                }
                            }
                        }

                        worker.ReportProgress(100, 0L);

                        string link = client.FinishUpload(transferKey);

                        result.FileLinks.Add(link);
                        result.SucceededFiles++;
                    }
                    else
                    {
                        result.FailedFiles++;
                    }

                    this.queueForm.Invoke(new Action(this.Refresh));
                }
                else
                {
                    result.FailedFiles++;
                }

                if (item.TemporaryFile) // Screenshot or clipboard dump
                {
                    CleanupTempFile(item.FolderPath, item.FileName, item.FileExtension, this.config.LocalScreenshotCopy);
                }

                lock (this.uploadItems)
                {
                    this.uploadItems.RemoveAt(0);
                }

                worker.ReportProgress(100, 0L);
            }

            return result;
        }

        private void UploadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UploadResult result = (UploadResult)e.Result;

            this.queueForm.Hide();

            if (result.SucceededFiles + result.FailedFiles == 0)
            {
                InfoForm.Show(result.Title, result.Message, 5000);
            }
            else if (result.SucceededFiles == 1 && result.FailedFiles == 0)
            {
                Clipboard.SetDataObject(result.FileLinks[0], true, 10, 100);
                InfoForm.Show("Upload completed!", "A link to your file has been copied to your clipboard.", 3000);
            }
            else if (result.SucceededFiles > 2 && result.FailedFiles == 0)
            {
                Clipboard.SetDataObject(string.Join(Environment.NewLine, result.FileLinks), true, 10, 100);
                InfoForm.Show("Upload completed!", "A list of download links has been copied to your clipboard.", 3000);
            }
            else
            {
                InfoForm.Show("Upload completed with errors!", "Some files were uploaded, some failed. This may happen when your storage is almost full.", 5000);
            }
            
            List<UploadItem> cleanupItems;

            lock (this.uploadItems)
            {
                cleanupItems = this.uploadItems.Where(item => item.TemporaryFile).ToList();
                this.uploadItems.Clear();
            }

            foreach (UploadItem item in cleanupItems)
            {
                CleanupTempFile(item.FolderPath, item.FileName, item.FileExtension, this.config.LocalScreenshotCopy);
            }
        }

        internal static void CleanupTempFile(string sourceFolder, string fileName, string fileExtension, bool copyLocal, bool showInExplorer = false)
        {
            if (copyLocal || showInExplorer)
            {
                string destinationFolder = Constants.Client.LocalItemsFolder;
                
                Directory.CreateDirectory(destinationFolder);

                string sourcePath = Path.Combine(sourceFolder, $"{fileName}{fileExtension}");
                string destinationPath = Path.Combine(destinationFolder, $"{fileName}{fileExtension}");
                int tries = 0;

                while (File.Exists(destinationPath))
                {
                    tries++;
                    destinationPath = Path.Combine(destinationFolder, $"{fileName}_{tries}{fileExtension}");
                }

                File.Move(sourcePath, destinationPath);

                if (showInExplorer)
                {
                    ShowFileInExplorer(destinationFolder, destinationPath);
                }
            }

            Directory.Delete(sourceFolder, true);
        }

        private static void ShowFileInExplorer(string folderPath, string filePath)
        {
            Shell32.SHParseDisplayName(Path.GetFullPath(folderPath), IntPtr.Zero, out IntPtr folder, 0, out uint psfgaoOut);

            if (folder == IntPtr.Zero)
            {
                return;
            }

            Shell32.SHParseDisplayName(Path.GetFullPath(filePath), IntPtr.Zero, out IntPtr file, 0, out psfgaoOut);

            if (file != IntPtr.Zero)
            {
                IntPtr[] files = { file };

                Shell32.SHOpenFolderAndSelectItems(folder, (uint)files.Length, files, 0);
                Marshal.FreeCoTaskMem(file);
            }

            Marshal.FreeCoTaskMem(folder);
        }
    }
}
