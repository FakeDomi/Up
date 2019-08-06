﻿using System.IO;
using System.Windows.Forms;
using domi1819.UpClient.Forms;
using domi1819.UpClient.Uploads;
using domi1819.UpCore.Config;
using domi1819.UpCore.Network;

namespace domi1819.UpClient
{
    internal class UpClient
    {
        private ScreenshotForm screenshotForm;
        private AboutForm aboutForm;
        private FileDropForm fileDropForm;
        private StorageExplorerForm storageExplorerForm;

        internal Config Config { get; private set; }

        internal NetClient NetClient { get; private set; }

        internal ActionManager ActionManager { get; private set; }

        internal ConfigurationForm ConfigurationForm { get; private set; }

        internal UploadManager UploadManager { get; private set; }

        internal RsaCache RsaCache { get; private set; }

        internal ScreenshotForm ScreenshotForm => this.screenshotForm ?? (this.screenshotForm = new ScreenshotForm(this));

        internal AboutForm AboutForm => this.aboutForm ?? (this.aboutForm = new AboutForm());

        internal FileDropForm FileDropForm => this.fileDropForm ?? (this.fileDropForm = new FileDropForm(this));

        internal StorageExplorerForm StorageExplorerForm
        {
            get
            {
                if (this.storageExplorerForm == null || this.storageExplorerForm.IsDisposed)
                {
                    this.storageExplorerForm = new StorageExplorerForm(this);
                }

                return this.storageExplorerForm;
            }
        }

        internal void LaunchApplication(string[] cmdArgs)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.Config = Config.Load();
            this.RsaCache = new RsaCache(this.Config.TrustFolder);
            this.NetClient = new NetClient(this.Config.ServerAddress, this.Config.ServerPort, this.RsaCache);
            this.ActionManager = new ActionManager(this);
            this.ConfigurationForm = new ConfigurationForm(this);
            this.UploadManager = new UploadManager(this);
            
            if (cmdArgs.Length > 0)
            {
                this.ProcessLiveArgs(cmdArgs, false);
            }

            Application.Run();
        }

        internal void ProcessLiveArgs(string[] args, bool isBackgroundThread)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] == "-upload" && args.Length > 1 && File.Exists(args[1]))
                {
                    this.UploadManager.Invoke(() =>
                    {
                        this.UploadManager.AddItem(new UploadItem(args[1]));
                    }, isBackgroundThread);
                }
            }
        }
    }
}
