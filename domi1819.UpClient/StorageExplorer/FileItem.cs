using System.ComponentModel;
using System.Drawing;

namespace domi1819.UpClient.StorageExplorer
{
    internal class FileItem
    {
        [Browsable(false)]
        internal string Identifier { get; }

        public Icon Icon { get; }

        public string Name { get; }

        public string Size { get; }

        public string Timestamp { get; }

        internal FileItem(string id, Icon icon, string name, string size, string timestamp)
        {
            this.Identifier = id;
            this.Icon = icon;
            this.Name = name;
            this.Size = size;
            this.Timestamp = timestamp;
        }
    }
}
