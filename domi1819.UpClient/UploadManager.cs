using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using domi1819.UpClient.Forms;
using domi1819.UpCore.Config;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient
{
    internal class UploadManager
    {
        private readonly UpClient upClient;
        private readonly UploadQueueForm uploadForm;
        
        internal List<UploadItem> UploadItems { get; }

        internal UploadManager(UpClient upClient)
        {
            this.upClient = upClient;

            this.UploadItems = new List<UploadItem>();
            this.uploadForm = new UploadQueueForm(upClient, this);
        }

        internal void AddItem(UploadItem item)
        {
            this.UploadItems.Add(item);
            this.Refresh();
        }

        internal void AddItems(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                this.UploadItems.Add(new UploadItem { FolderPath = Path.GetDirectoryName(path), FileName = Path.GetFileNameWithoutExtension(path), FileExtension = Path.GetExtension(path) });
            }

            this.Refresh();
        }

        internal void Refresh()
        {
            this.uploadForm.FitSize(this.uploadForm.RefreshList(this.UploadItems));

            if (!this.uploadForm.BackgroundWorker.IsBusy)
            {
                this.uploadForm.KeepVisible = true;
                this.uploadForm.BackgroundWorker.RunWorkerAsync();
            }

            this.uploadForm.Show();
        }

        internal void StartUpload(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            Config settings = this.upClient.Config;
            NetClient client = this.upClient.NetClient;

            try
            {
                client.ClaimConnectHandle();
            }
            catch (SocketException ex)
            {
                args.Result = new UploadResult { Title = "Connection failed!", Message = ex.Message };
                return;
            }

            args.Result = client.Login(settings.UserId, settings.Password) ? this.Upload(worker, client) : new UploadResult { Title = "Login failed!", Message = "Please check your account settings." };
            client.ReleaseConnectHandle();
        }

        internal UploadResult Upload(BackgroundWorker worker, NetClient client)
        {
            try
            {
                client.ClaimConnectHandle();
            }
            catch (Exception)
            {
                // Could not connect or smth

                foreach (UploadItem item in this.UploadItems.Where(item => item.TemporaryFile))
                {
                    this.CleanupTempFile(item.FolderPath, item.FileName, item.FileExtension);
                }

                this.UploadItems.Clear();

                return null;
            }

            byte[] fileBuf = new byte[4096];
            UploadResult result = new UploadResult();

            while (this.UploadItems.Count > 0)
            {
                UploadItem item = this.UploadItems[0];
                string file = Path.Combine(item.FolderPath, $"{item.FileName}{item.FileExtension}");

                if (File.Exists(file))
                {
                    string transferKey = client.InitiateUpload(Path.GetFileName(file), new FileInfo(file).Length);

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
                }
                else
                {
                    result.FailedFiles++;
                }

                if (item.TemporaryFile) // Screenshot or clipboard dump
                {
                    this.CleanupTempFile(item.FolderPath, item.FileName, item.FileExtension);
                }

                this.UploadItems.RemoveAt(0);
                worker.ReportProgress(100, 0L);
            }

            client.ReleaseConnectHandle();

            return result;
        }

        internal void UploadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UploadResult result = (UploadResult)e.Result;

            this.uploadForm.Hide();

            if (result.SucceededFiles + result.FailedFiles == 0)
            {
                InfoForm.Show(result.Title, result.Message, 5000);
            }

            if (result.SucceededFiles == 1 && result.FailedFiles == 0)
            {
                Clipboard.SetText(result.FileLinks[0]);
                InfoForm.Show("Upload completed!", "A link to your file has been copied to your clipboard.", 3000);
            }
            else if (result.SucceededFiles > 2 && result.FailedFiles == 0)
            {
                Clipboard.SetText(string.Join(Environment.NewLine, result.FileLinks));
                InfoForm.Show("Upload completed!", "A list of download links has been copied to your clipboard.", 3000);
            }
            else
            {
                InfoForm.Show("Upload completed with errors!", "Some files were uploaded, some failed. Links to successfully uploaded files are in your Clipboard. Check the log for details.", 5000);
            }
        }

        internal void CleanupTempFile(string folderPath, string fileName, string fileExtension, bool showInExplorer = false)
        {
            if (!Directory.Exists(Constants.Client.LocalItemsFolder))
            {
                Directory.CreateDirectory(Constants.Client.LocalItemsFolder);
            }

            string sourcePath = Path.Combine(folderPath, $"{fileName}{fileExtension}");
            string destinationPath = Path.Combine(Constants.Client.LocalItemsFolder, $"{fileName}{fileExtension}");
            int tries = 0;
            
            while (File.Exists(destinationPath))
            {
                tries++;
                destinationPath = Path.Combine(Constants.Client.LocalItemsFolder, $"{fileName}_{tries}{fileExtension}");
            }

            if (this.upClient.Config.LocalScreenshotCopy || showInExplorer)
            {
                File.Move(sourcePath, destinationPath);

                if (showInExplorer)
                {
                    Process.Start("explorer", $"/select, \"{destinationPath}\"");
                }
            }

            Directory.Delete(folderPath, true);
        }
    }
}
