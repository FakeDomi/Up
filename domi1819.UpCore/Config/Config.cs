using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using domi1819.UpCore.Utilities;

namespace domi1819.UpCore.Config
{
    public class Config
    {
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public bool LocalScreenshotCopy { get; set; }

        public DropArea DropArea { get; set; }

        public UpdateBehavior UpdateBehavior { get; set; }

        public bool PngScreenshots { get; set; }

        public bool PendingUpdate { get; set; }

        public Hotkeys Hotkeys { get; set; }

        public WrappedColor ThemeColor { get; set; }

        public string TrustFolder { get; set; }

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Config));

        public Config()
        {
            this.ServerAddress = "up.domi1819.xyz";
            this.ServerPort = 1819;
            this.UserId = "username";
            this.Password = "password";
            this.LocalScreenshotCopy = true;
            this.DropArea = new DropArea();
            this.PngScreenshots = true;
            this.Hotkeys = new Hotkeys();
            this.ThemeColor = WrappedColor.Of(Color.FromArgb(16, 48, 128));
            this.TrustFolder = "trust";
        }

        public void SaveFile()
        {
            using (StreamWriter writer = new StreamWriter(Constants.Client.ConfigFileName))
            {
                Serializer.Serialize(writer, this);
            }
        }

        public static Config Load()
        {
            Config settings;

            if (File.Exists(Constants.Client.ConfigFileName))
            {
                using (StreamReader reader = new StreamReader(Constants.Client.ConfigFileName))
                {
                    settings = (Config)Serializer.Deserialize(reader);
                }
            }
            else
            {
                settings = new Config();
            }

            return settings;
        }
    }
}
