using domi1819.DarkControls;
using domi1819.UpCore.Config;

namespace domi1819.UpClient.Forms
{
    public partial class HotkeyForm : DarkForm
    {
        public HotkeyForm()
        {
            this.InitializeComponent();
        }

        public void ResetFields(Config settings)
        {
            Hotkeys hotkeys = settings.Hotkeys;

            this.uiUploadFileKeyBox.Key = hotkeys.UploadFile.Key;
            this.uiUploadFileKeyBox.Modifiers = hotkeys.UploadFile.Modifier;
            this.uiUploadFileKeyBox.RefreshText();
            
            this.uiUploadScreenshotKeyBox.Key = hotkeys.UploadScreenshot.Key;
            this.uiUploadScreenshotKeyBox.Modifiers = hotkeys.UploadScreenshot.Modifier;
            this.uiUploadScreenshotKeyBox.RefreshText();

            this.uiUploadScreenAreaKeyBox.Key = hotkeys.UploadScreenArea.Key;
            this.uiUploadScreenAreaKeyBox.Modifiers = hotkeys.UploadScreenArea.Modifier;
            this.uiUploadScreenAreaKeyBox.RefreshText();

            this.uiUploadClipboardKeyBox.Key = hotkeys.UploadClipboard.Key;
            this.uiUploadClipboardKeyBox.Modifiers = hotkeys.UploadClipboard.Modifier;
            this.uiUploadClipboardKeyBox.RefreshText();

            this.uiShowFileDropAreaKeyBox.Key = hotkeys.ShowFileDropArea.Key;
            this.uiShowFileDropAreaKeyBox.Modifiers = hotkeys.ShowFileDropArea.Modifier;
            this.uiShowFileDropAreaKeyBox.RefreshText();

            this.uiOpenStorageExplorerKeyBox.Key = hotkeys.OpenStorageExplorer.Key;
            this.uiOpenStorageExplorerKeyBox.Modifiers = hotkeys.OpenStorageExplorer.Modifier;
            this.uiOpenStorageExplorerKeyBox.RefreshText();

            this.uiSaveLocalScreenshotKeyBox.Key = hotkeys.SaveLocalScreenshot.Key;
            this.uiSaveLocalScreenshotKeyBox.Modifiers = hotkeys.SaveLocalScreenshot.Modifier;
            this.uiSaveLocalScreenshotKeyBox.RefreshText();

            this.uiSaveLocalScreenAreaKeyBox.Key = hotkeys.SaveLocalScreenArea.Key;
            this.uiSaveLocalScreenAreaKeyBox.Modifiers = hotkeys.SaveLocalScreenArea.Modifier;
            this.uiSaveLocalScreenAreaKeyBox.RefreshText();

            this.uiSaveLocalClipboardKeyBox.Key = hotkeys.SaveLocalClipboard.Key;
            this.uiSaveLocalClipboardKeyBox.Modifiers = hotkeys.SaveLocalClipboard.Modifier;
            this.uiSaveLocalClipboardKeyBox.RefreshText();
        }

        public void SaveToConfig(Config settings)
        {
            Hotkeys hotkeys = settings.Hotkeys;

            hotkeys.UploadFile.Key = this.uiUploadFileKeyBox.Key;
            hotkeys.UploadFile.Modifier = this.uiUploadFileKeyBox.Modifiers;

            hotkeys.UploadScreenshot.Key = this.uiUploadScreenshotKeyBox.Key;
            hotkeys.UploadScreenshot.Modifier = this.uiUploadScreenshotKeyBox.Modifiers;

            hotkeys.UploadScreenArea.Key = this.uiUploadScreenAreaKeyBox.Key;
            hotkeys.UploadScreenArea.Modifier = this.uiUploadScreenAreaKeyBox.Modifiers;

            hotkeys.UploadClipboard.Key = this.uiUploadClipboardKeyBox.Key;
            hotkeys.UploadClipboard.Modifier = this.uiUploadClipboardKeyBox.Modifiers;

            hotkeys.ShowFileDropArea.Key = this.uiShowFileDropAreaKeyBox.Key;
            hotkeys.ShowFileDropArea.Modifier = this.uiShowFileDropAreaKeyBox.Modifiers;

            hotkeys.OpenStorageExplorer.Key = this.uiOpenStorageExplorerKeyBox.Key;
            hotkeys.OpenStorageExplorer.Modifier = this.uiOpenStorageExplorerKeyBox.Modifiers;

            hotkeys.SaveLocalScreenshot.Key = this.uiSaveLocalScreenshotKeyBox.Key;
            hotkeys.SaveLocalScreenshot.Modifier = this.uiSaveLocalScreenshotKeyBox.Modifiers;

            hotkeys.SaveLocalScreenArea.Key = this.uiSaveLocalScreenAreaKeyBox.Key;
            hotkeys.SaveLocalScreenArea.Modifier = this.uiSaveLocalScreenAreaKeyBox.Modifiers;

            hotkeys.SaveLocalClipboard.Key = this.uiSaveLocalClipboardKeyBox.Key;
            hotkeys.SaveLocalClipboard.Modifier = this.uiSaveLocalClipboardKeyBox.Modifiers;
        }
    }
}
