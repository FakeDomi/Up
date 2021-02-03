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

        public int[] UpServerPorts { get; set; }

        public int HttpServerPort { get; set; }

        public bool Ipv6Allowed { get; set; }

        public string HttpServerListenerName { get; set; }

        public string UrlOverride { get; set; }

        public string FileStorageFolder { get; set; }

        public string FileTransferFolder { get; set; }

        public string DataFolder { get; set; }

        public string WebFolder { get; set; }

        public bool WebInterfaceEnabled { get; set; }

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ServerConfig));

        public ServerConfig()
        {
            this.HostName = "localhost";
            this.UpServerPorts = new[] { 1819 };
            this.HttpServerPort = 1880;
            this.Ipv6Allowed = true;
            this.HttpServerListenerName = "+";
            this.UrlOverride = "";

            this.FileStorageFolder = "storage";
            this.FileTransferFolder = "transfer";
            this.DataFolder = "data";
            this.WebFolder = "web";

            this.WebInterfaceEnabled = true;
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

        static ServerConfig()
        {
            Serializer.UnknownElement += (sender, args) =>
            {
                if (args.Element.Name == "UpServerPort")
                {
                    ServerConfig config = (ServerConfig)args.ObjectBeingDeserialized;

                    if (int.TryParse(args.Element.InnerText, out int port))
                    {
                        config.UpServerPorts = new[] { port };
                    }
                }
            };
        }
    }
}
