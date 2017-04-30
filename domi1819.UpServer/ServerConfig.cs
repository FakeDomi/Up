using System.IO;
using System.Xml.Serialization;
using domi1819.UpCore.Utilities;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace domi1819.UpServer
{
    public class ServerConfig
    {
        public string HostName { get; set; }

        public int UpServerPort { get; set; }

        public int HttpServerPort { get; set; }

        public string HttpServerListenerName { get; set; }

        public string UrlOverride { get; set; }

        public string FileStorageFolder { get; set; }

        public string FileTransferFolder { get; set; }

        public string DataFolder { get; set; }

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ServerConfig));

        public ServerConfig()
        {
            this.HostName = "localhost";
            this.UpServerPort = 1819;
            this.HttpServerPort = 1880;
            this.HttpServerListenerName = "+";
            this.UrlOverride = "";

            this.FileStorageFolder = "storage";
            this.FileTransferFolder = "transfer";
            this.DataFolder = "data";
        }

        public void Save(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Serializer.Serialize(new FancyXmlWriter(writer), this);
            }
        }

        public static ServerConfig Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return (ServerConfig)Serializer.Deserialize(reader);
                }
            }

            return new ServerConfig();
        }
    }
}
