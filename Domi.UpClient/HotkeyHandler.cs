using System;
using System.Windows.Forms;
using Domi.UpCore.Config;
using Domi.UpCore.Windows;

namespace Domi.UpClient
{
    /// <summary>
    /// This class registers and unregisters hotkeys.
    /// It initiates running of actions caused by hotkeys.
    /// </summary>
    internal class HotkeyHandler
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

        /// <summary>
        /// Construct a new HotkeyHandler using a Win32 host window and an action manager.
        /// </summary>
        /// <param name="host">The host that hotkeys will be registered for.</param>
        /// <param name="actionManager">The action manager that will be called to process hotkey presses.</param>
        internal HotkeyHandler(IWin32Window host, ActionManager actionManager)
        {
            this.hostHandle = host.Handle;
            this.actionManager = actionManager;
        }

        /// <summary>
        /// Unregister all hotkeys, then reload using a new config.
        /// </summary>
        /// <param name="config">The config to use.</param>
        internal void ReloadHotkeys(Config config)
        {
            this.SuspendHotkeys();
            this.ActivateHotkeys(config);
        }

        /// <summary>
        /// Unregister all hotkeys.
        /// </summary>
        internal void SuspendHotkeys()
        {
            for (int id = 0; id < this.registeredHotkeys; id++)
            {
                User32.UnregisterHotKey(this.hostHandle, id);
            }

            this.registeredHotkeys = 0;
        }

        /// <summary>
        /// Register all known hotkeys.
        /// </summary>
        /// <param name="config">The configuration to use hotkey data from.</param>
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

        /// <summary>
        /// Process a hotkey event.
        /// </summary>
        /// <param name="keyCode">The Win32 key press of the hotkey event.</param>
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

        /// <summary>
        /// Register a hotkey using the host handle as callback object.
        /// </summary>
        /// <param name="hotkey">A hotkey config entity to register.</param>
        /// <returns>An int that represents a Win32 key press with scancode and modifiers.</returns>
        private int RegisterHotkey(Hotkey hotkey)
        {
            if (hotkey.Key > 0 && hotkey.Modifier > 0)
            {
                User32.RegisterHotKey(this.hostHandle, this.registeredHotkeys++, ConvertModifiers(hotkey.Modifier), (int)hotkey.Key);
                return ((int)hotkey.Key << 16) | ConvertModifiers(hotkey.Modifier);
            }

            return 0;
        }

        /// <summary>
        /// Convert .NET key modifiers to Win32 modifiers.
        /// Supports ALT, CONTROL and SHIFT.
        /// </summary>
        /// <param name="keys">The keys entity holding the needed modifiers.</param>
        /// <returns>An int that represents the input modifiers as Win32 modifiers.</returns>
        private static int ConvertModifiers(Keys keys)
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
