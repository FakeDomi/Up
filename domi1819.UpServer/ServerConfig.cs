using System.IO;
using System.Xml.Serialization;

namespace domi1819.UpServer
{
    public class ServerConfig
    {
        public string HostName { get; set; }
        
        public int ServerPort { get; set; }
        
        public int WebPort { get; set; }
        
        public string OverrideAddress { get; set; }
        
        public string FileStorageFolder { get; set; }
        
        public string FileTransferFolder { get; set; }

        public string DataFolder { get; set; }
        
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ServerConfig));

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter("settings.xml"))
            {
                Serializer.Serialize(new WrappedXmlWriter(writer), this);
            }
        }

        public static ServerConfig Load()
        {
            ServerConfig settings;

            if (File.Exists("config.xml"))
            {
                using (StreamReader reader = new StreamReader("config.xml"))
                {
                    settings = (ServerConfig)Serializer.Deserialize(reader);
                }
            }
            else
            {
                settings = new ServerConfig
                {
                    HostName = "localhost",
                    ServerPort = 1819,
                    WebPort = 1880,
                    OverrideAddress = "",
                    FileStorageFolder = "storage",
                    FileTransferFolder = "transfer",
                    DataFolder = "data"
                };
            }

            return settings;
        }
    }
}
