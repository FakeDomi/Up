using Domi.DarkControls;
using Domi.UpCore.Config;

namespace Domi.UpClient.Forms
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
            this.uiUploadScreenshotKeyBox.Set(hotkeys.UploadScreenshot.Modifier, hotkeys.UploadScreenshot.Key);
            this.uiUploadScreenAreaKeyBox.Set(hotkeys.UploadScreenArea.Modifier, hotkeys.UploadScreenArea.Key);
            this.uiUploadClipboardKeyBox.Set(hotkeys.UploadClipboard.Modifier, hotkeys.UploadClipboard.Key);
            this.uiShowFileDropAreaKeyBox.Set(hotkeys.ShowFileDropArea.Modifier, hotkeys.ShowFileDropArea.Key);
            this.uiOpenStorageExplorerKeyBox.Set(hotkeys.OpenStorageExplorer.Modifier, hotkeys.OpenStorageExplorer.Key);
            this.uiSaveLocalScreenshotKeyBox.Set(hotkeys.SaveLocalScreenshot.Modifier, hotkeys.SaveLocalScreenshot.Key);
            this.uiSaveLocalScreenAreaKeyBox.Set(hotkeys.SaveLocalScreenArea.Modifier, hotkeys.SaveLocalScreenArea.Key);
            this.uiSaveLocalClipboardKeyBox.Set(hotkeys.SaveLocalClipboard.Modifier, hotkeys.SaveLocalClipboard.Key);
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
