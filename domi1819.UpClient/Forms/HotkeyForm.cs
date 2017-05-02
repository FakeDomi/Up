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

            this.uiUploadFileKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiUploadScreenshotKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiUploadScreenAreaKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiUploadClipboardKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiShowFileDropAreaKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiOpenStorageExplorerKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiSaveLocalScreenshotKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiSaveLocalScreenAreaKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
            this.uiSaveLocalClipboardKeyBox.Set(hotkeys.UploadFile.Modifier, hotkeys.UploadFile.Key);
        }

        public void FillConfig(Config settings)
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
