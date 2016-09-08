using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient.StorageExplorer
{
    internal class FileItem
    {
        [Browsable(false)]
        internal string Identifier { get; }

        public Icon Icon { get; }

        public string Name { get; }

        public string Size { get; }

        public int Downloads { get; }

        public string Timestamp { get; }

        internal FileItem(string id, Icon icon, string name, string size, int downloads, string timestamp)
        {
            this.Identifier = id;
            this.Icon = icon;
            this.Name = name;
            this.Size = size;
            this.Downloads = downloads;
            this.Timestamp = timestamp;
        }

        internal FileItem(string id, Icon icon, string name, string size, string timestamp)
        {
            this.Identifier = id;
            this.Icon = icon;
            this.Name = name;
            this.Size = size;
            this.Timestamp = timestamp;
        }

        internal static FileItem Construct(string id, string name, long size, DateTime timestamp, int downloads, FileIconCache icons)
        {
            return new FileItem(id, icons[Path.GetExtension(name)], name, Util.GetByteSizeText(size), downloads, timestamp.FormatString());
        }
    }
}
