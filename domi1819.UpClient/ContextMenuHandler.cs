using System;
using System.Windows.Forms;

namespace domi1819.UpClient
{
    internal class ContextMenuHandler
    {
        private readonly UpClient upClient;

        private readonly MenuItem menuItemUpdate, menuItemUpload, menuItemDropArea, menuItemScreenshot, menuItemClipboard, menuItemScreenArea, menuItemStorageExplorer, menuItemConfiguration, menuItemAbout, menuItemExit;
        private readonly MenuItem menuItemLocalScreenshot, menuItemLocalScreenArea, menuItemLocalClipboard;

        internal ContextMenuHandler(UpClient upClient)
        {
            this.menuItemUpdate = new MenuItem("Update", this.Update);
            this.menuItemUpload = new MenuItem("Upload file", this.UploadFile);
            this.menuItemScreenshot = new MenuItem("Upload screenshot", this.UploadScreenshot);
            this.menuItemScreenArea = new MenuItem("Upload screen area", this.UploadArea);
            this.menuItemClipboard = new MenuItem("Upload clipboard", this.UploadClipboard);
            this.menuItemDropArea = new MenuItem("Show file drop area", this.ToggleFileDropArea);
            this.menuItemStorageExplorer = new MenuItem("Storage explorer", this.ShowFiles);
            this.menuItemConfiguration = new MenuItem("Configuration", this.ShowConfiguration) { DefaultItem = true };
            this.menuItemAbout = new MenuItem("About", this.ShowInfo);
            this.menuItemExit = new MenuItem("Exit", this.Exit);

            this.menuItemLocalScreenshot = new MenuItem("Save screenshot", this.SaveScreenshot);
            this.menuItemLocalScreenArea = new MenuItem("Save screen area", this.SaveArea);
            this.menuItemLocalClipboard = new MenuItem("Save clipboard", this.SaveClipboard);

            this.upClient = upClient;
        }
        
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

        private void Update(object o, EventArgs e)
        {
            this.upClient.ActionManager.Update();
        }

        private void UploadFile(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadFile();
        }

        private void UploadScreenshot(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadScreenshot(true, false, 300);
        }

        private void UploadArea(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadScreenshot(false, false, 300);
        }

        private void SaveScreenshot(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadScreenshot(true, true, 300);
        }

        private void SaveArea(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadScreenshot(false, true, 300);
        }

        private void SaveClipboard(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadClipboard(true);
        }

        private void UploadClipboard(object o, EventArgs e)
        {
            this.upClient.ActionManager.UploadClipboard();
        }

        private void ToggleFileDropArea(object o, EventArgs e)
        {
            this.upClient.ActionManager.ToggleFileDropArea();
        }

        private void ShowFiles(object o, EventArgs e)
        {
            this.upClient.ActionManager.ShowFiles();
        }

        private void ShowConfiguration(object o, EventArgs e)
        {
            this.upClient.ActionManager.ShowConfiguration();
        }
        
        private void ShowInfo(object o, EventArgs e)
        {
            this.upClient.ActionManager.ShowInfo();
        }

        private void Exit(object o, EventArgs e)
        {
            this.upClient.ActionManager.Exit();
        }
    }
}
