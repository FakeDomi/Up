namespace domi1819.UpCore.Config
{
    public class Hotkeys
    {
        public Hotkey UploadFile { get; set; }
        public Hotkey UploadScreenshot { get; set; }
        public Hotkey UploadScreenArea { get; set; }
        public Hotkey UploadClipboard { get; set; }
        public Hotkey ShowFileDropArea { get; set; }
        public Hotkey OpenStorageExplorer { get; set; }
        public Hotkey SaveLocalScreenshot { get; set; }
        public Hotkey SaveLocalScreenArea { get; set; }
        public Hotkey SaveLocalClipboard { get; set; }

        public Hotkeys()
        {
            this.UploadFile = new Hotkey();
            this.UploadScreenshot = new Hotkey();
            this.UploadScreenArea = new Hotkey();
            this.UploadClipboard = new Hotkey();
            this.ShowFileDropArea = new Hotkey();
            this.OpenStorageExplorer = new Hotkey();
            this.SaveLocalScreenshot = new Hotkey();
            this.SaveLocalScreenArea = new Hotkey();
            this.SaveLocalClipboard = new Hotkey();
        }
    }
}
