using System.Windows.Forms;
using domi1819.UpClient.Forms;
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
            if (cmdArgs.Length > 0)
            {
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.Config = Config.Load();
            this.NetClient = new NetClient(this.Config.ServerAddress, this.Config.ServerPort);
            this.ActionManager = new ActionManager(this);
            this.ConfigurationForm = new ConfigurationForm(this);
            this.UploadManager = new UploadManager(this);
            
            Application.Run();
        }
    }
}
