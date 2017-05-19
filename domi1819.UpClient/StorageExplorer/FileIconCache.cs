using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using domi1819.UpCore.Windows;
using static domi1819.UpCore.Windows.WinConsts;

namespace domi1819.UpClient.StorageExplorer
{
    internal class FileIconCache
    {
        private readonly Dictionary<string, Icon> iconMap = new Dictionary<string, Icon>();

        internal Icon this[string extension]
        {
            get
            {
                if (this.iconMap.TryGetValue(extension, out Icon icon))
                {
                    return icon;
                }

                Shell32.SHFILEINFO shInfo = new Shell32.SHFILEINFO();

                Shell32.SHGetFileInfo(extension, FILE_ATTRIBUTE_NORMAL, ref shInfo, (uint)Marshal.SizeOf(shInfo), SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES | SHGFI_ICON);

                icon = (Icon)Icon.FromHandle(shInfo.hIcon).Clone();
                User32.DestroyIcon(shInfo.hIcon);

                this.iconMap[extension] = icon;

                return icon;
            }
        }
    }
}
