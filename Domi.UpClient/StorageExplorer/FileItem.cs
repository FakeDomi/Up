﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Domi.UpCore.Utilities;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Domi.UpClient.StorageExplorer
{
    internal class FileItem
    {
        [Browsable(false)]
        internal string Identifier { get; }

        public Icon Icon { get; }

        public string Name { get; }

        public string Size { get; }

        public string Downloads { get; }

        public string Timestamp { get; }

        internal FileItem(string id, Icon icon, string name, string size, int downloads, string timestamp)
        {
            this.Identifier = id;
            this.Icon = icon;
            this.Name = name;
            this.Size = size;
            this.Downloads = $"{downloads}x";
            this.Timestamp = timestamp;
        }

        internal static FileItem Construct(string id, string name, long size, DateTime timestamp, int downloads, FileIconCache icons)
        {
            return new FileItem(id, icons[Path.GetExtension(name)], name, Util.GetByteSizeText(size), downloads, timestamp.FormatString());
        }
    }
}
