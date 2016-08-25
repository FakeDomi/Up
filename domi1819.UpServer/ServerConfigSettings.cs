using System.IO;
using System.Xml.Serialization;

namespace domi1819.UpServer
{
    [XmlRoot("config")]
    public class ServerConfigSettings
    {
        [XmlElement("hostname")]
        public string HostName { get; set; }

        [XmlElement("serverport")]
        public int ServerPort { get; set; }

        [XmlElement("webport")]
        public int WebPort { get; set; }

        [XmlElement("override")]
        public string OverrideAddress { get; set; }

        private static XmlSerializer serializer = new XmlSerializer(typeof(ServerConfigSettings));

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter("settings.xml"))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static ServerConfigSettings Load()
        {
            ServerConfigSettings settings;

            if (File.Exists("settings.xml"))
            {
                using (StreamReader reader = new StreamReader("settings.xml"))
                {
                    settings = (ServerConfigSettings)serializer.Deserialize(reader);
                }
            }
            else
            {
                settings = new ServerConfigSettings { HostName = "localhost", ServerPort = 1819, WebPort = 1880, OverrideAddress = "" };
            }

            return settings;
        }
    }
}
