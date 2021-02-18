using System.Windows.Forms;

namespace Domi.UpClient
{
    /// <summary>
    /// This class populates a context menu and initiates actions of it.
    /// </summary>
    internal class ContextMenuHandler
    {
        private readonly UpClient upClient;

        private readonly MenuItem menuItemUpdate, menuItemUpload, menuItemDropArea, menuItemScreenshot, menuItemClipboard, menuItemScreenArea, menuItemStorageExplorer, menuItemConfiguration, menuItemAbout, menuItemExit;
        private readonly MenuItem menuItemLocalScreenshot, menuItemLocalScreenArea, menuItemLocalClipboard;

        /// <summary>
        /// Construct a new context menu handler.
        /// </summary>
        /// <param name="upClient">The up client instance creating this context menu handler.</param>
        internal ContextMenuHandler(UpClient upClient)
        {
            ActionManager actionManager = upClient.ActionManager;

            this.menuItemUpdate = new MenuItem("Update", (s, a) => actionManager.Update());
            this.menuItemUpload = new MenuItem("Upload file", (s, a) => actionManager.UploadFile());
            this.menuItemScreenshot = new MenuItem("Upload screenshot", (s, a) => actionManager.UploadScreenshot(true, false, 300));
            this.menuItemScreenArea = new MenuItem("Upload screen area", (s, a) => actionManager.UploadScreenshot(false, false, 300));
            this.menuItemClipboard = new MenuItem("Upload clipboard", (s, a) => actionManager.UploadClipboard());
            this.menuItemDropArea = new MenuItem("Show file drop area", (s, a) => actionManager.ToggleFileDropArea());
            this.menuItemStorageExplorer = new MenuItem("Storage explorer", (s, a) => actionManager.ShowFiles());
            this.menuItemConfiguration = new MenuItem("Configuration", (s, a) => actionManager.ShowConfiguration()) { DefaultItem = true };
            this.menuItemAbout = new MenuItem("About", (s, a) => actionManager.ShowInfo());
            this.menuItemExit = new MenuItem("Exit", (s, a) => actionManager.Exit());

            this.menuItemLocalScreenshot = new MenuItem("Save screenshot", (s, a) => actionManager.UploadScreenshot(true, true, 300));
            this.menuItemLocalScreenArea = new MenuItem("Save screen area", (s, a) => actionManager.UploadScreenshot(false, true, 300));
            this.menuItemLocalClipboard = new MenuItem("Save clipboard", (s, a) => actionManager.UploadClipboard(true));

            this.upClient = upClient;
        }

        /// <summary>
        /// Clear the context menu, then re-add the proper items.
        /// </summary>
        /// <param name="menuItems">The context menu colleciton to rebuild.</param>
        internal void Rebuild(Menu.MenuItemCollection menuItems)
        {
            menuItems.Clear();

            if (this.upClient.Config.PendingUpdate)
            {
                menuItems.Add(this.menuItemUpdate);
                menuItems.Add("-");
            }

            this.menuItemDropArea.Text = this.upClient.Config.DropArea.Show ? "Hide file drop area" : "Show file drop area";

            menuItems.Add(this.menuItemUpload);
            menuItems.Add(this.menuItemScreenshot);
            menuItems.Add(this.menuItemScreenArea);
            menuItems.Add(this.menuItemClipboard);
            menuItems.Add("-");
            menuItems.Add(this.menuItemLocalScreenshot);
            menuItems.Add(this.menuItemLocalScreenArea);
            menuItems.Add(this.menuItemLocalClipboard);
            menuItems.Add("-");
            menuItems.Add(this.menuItemDropArea);
            menuItems.Add("-");
            menuItems.Add(this.menuItemStorageExplorer);
            menuItems.Add("-");
            menuItems.Add(this.menuItemConfiguration);

            menuItems.Add(this.menuItemAbout);
            menuItems.Add("-");
            menuItems.Add(this.menuItemExit);
        }
    }
}
