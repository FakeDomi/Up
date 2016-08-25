using System;
using System.Windows.Forms;
using domi1819.UpCore.Config;
using domi1819.UpCore.Native;

namespace domi1819.UpClient
{
    internal class HotkeyManager
    {
        private readonly IntPtr hostHandle;
        private readonly ActionManager actionManager;

        private int registeredHotkeys;

        private int uploadFileKeyCode;
        private int uploadScreenshotKeyCode;
        private int uploadScreenAreaKeyCode;
        private int uploadClipboardKeyCode;
        private int showFileDropAreaKeyCode;
        private int openStorageExplorerKeyCode;
        private int saveLocalScreenshotKeyCode;
        private int saveLocalScreenAreaKeyCode;
        private int saveLocalClipboardKeyCode;

        internal HotkeyManager(IWin32Window host, ActionManager actionManager)
        {
            this.hostHandle = host.Handle;
            this.actionManager = actionManager;
        }

        internal void ReloadHotkeys(Config settings)
        {
            this.SuspendHotkeys();
            this.ActivateHotkeys(settings);
        }

        internal void SuspendHotkeys()
        {
            for (int id = 0; id < this.registeredHotkeys; id++)
            {
                User32.UnregisterHotKey(this.hostHandle, id);
            }

            this.registeredHotkeys = 0;
        }

        internal void ActivateHotkeys(Config config)
        {
            Hotkeys hotkeySettings = config.Hotkeys;
            
            this.uploadFileKeyCode = this.RegisterHotkey(hotkeySettings.UploadFile);
            this.uploadScreenshotKeyCode = this.RegisterHotkey(hotkeySettings.UploadScreenshot);
            this.uploadScreenAreaKeyCode = this.RegisterHotkey(hotkeySettings.UploadScreenArea);
            this.uploadClipboardKeyCode = this.RegisterHotkey(hotkeySettings.UploadClipboard);
            this.showFileDropAreaKeyCode = this.RegisterHotkey(hotkeySettings.ShowFileDropArea);
            this.openStorageExplorerKeyCode = this.RegisterHotkey(hotkeySettings.OpenStorageExplorer);
            this.saveLocalScreenshotKeyCode = this.RegisterHotkey(hotkeySettings.SaveLocalScreenshot);
            this.saveLocalScreenAreaKeyCode = this.RegisterHotkey(hotkeySettings.SaveLocalScreenArea);
            this.saveLocalClipboardKeyCode = this.RegisterHotkey(hotkeySettings.SaveLocalClipboard);
        }

        internal void ProcessHotkey(int keyCode)
        {
            if (keyCode == this.uploadFileKeyCode)
            {
                this.actionManager.UploadFile();
            }
            else if (keyCode == this.uploadScreenshotKeyCode)
            {
                this.actionManager.UploadScreenshot(true, false);
            }
            else if (keyCode == this.uploadScreenAreaKeyCode)
            {
                this.actionManager.UploadScreenshot(false, false);
            }
            else if (keyCode == this.uploadClipboardKeyCode)
            {
                this.actionManager.UploadClipboard();
            }
            else if (keyCode == this.showFileDropAreaKeyCode)
            {
                this.actionManager.ToggleFileDropArea();
            }
            else if (keyCode == this.openStorageExplorerKeyCode)
            {
                this.actionManager.ShowFiles();
            }
            else if (keyCode == this.saveLocalScreenshotKeyCode)
            {
                this.actionManager.UploadScreenshot(true, true);
            }
            else if (keyCode == this.saveLocalScreenAreaKeyCode)
            {
                this.actionManager.UploadScreenshot(false, true);
            }
            else if (keyCode == this.saveLocalClipboardKeyCode)
            {
                this.actionManager.UploadClipboard(true);
            }
        }

        private int RegisterHotkey(Hotkey hotkey)
        {
            if (hotkey.Key > 0 && hotkey.Modifier > 0)
            {
                User32.RegisterHotKey(this.hostHandle, this.registeredHotkeys++, this.ConvertModifiers(hotkey.Modifier), (int)hotkey.Key);
                return ((int)hotkey.Key << 16) | this.ConvertModifiers(hotkey.Modifier);
            }

            return 0;
        }

        private int ConvertModifiers(Keys keys)
        {
            int returnValue = 0;

            if ((keys & Keys.Alt) > 0)
            {
                returnValue += 1;
            }

            if ((keys & Keys.Control) > 0)
            {
                returnValue += 2;
            }

            if ((keys & Keys.Shift) > 0)
            {
                returnValue += 4;
            }
            
            return returnValue;
        }
    }
}
