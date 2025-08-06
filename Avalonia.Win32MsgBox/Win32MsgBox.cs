/*
 MIT License

Copyright (c) 2025 sparkchaser

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/
using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Win32MsgBox
{
    /// <summary>
    /// Indicates which button on the dialog box was pressed.
    /// </summary>
    public enum MsgBoxRetVal
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7,
        TryAgain = 10,
        Continue = 11,
    }

    /// <summary>
    /// Thin wrapper around the Win32 MessageBox API.
    /// </summary>
    public static class Win32MsgBox
    {
        #region Win32 Interop

        // See: https://learn.microsoft.com/en-us/windows/win32/dlgbox/using-dialog-boxes
        //      https://pinvoke.net/default.aspx/user32/MessageBox.htm
        // These have default Windows controls and not Avalonia controls, though.
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int MessageBoxEx(IntPtr hWnd, string lpText, string lpCaption, uint uType, ushort wLanguageId);
        // From: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
        // Choices of which buttons the dialog should have
        public const uint MB_OK = 0x0;                  // OK button only
        public const uint MB_OKCANCEL = 0x1;            // OK/cancel
        public const uint MB_ABORTRETRYIGNORE = 0x2;    // abort/retry/ignore
        public const uint MB_YESNOCANCEL = 0x3;         // Yes/No/Cancel
        public const uint MB_YESNO = 0x4;               // Yes/No
        public const uint MB_RETRYCANCEL = 0x5;         // Retry/Cancel
        public const uint MB_CANCELTRYCONTINUE = 0x6;   // cancel/try again/continue
                                                        // Icon display choices
        public const uint MB_ICONSTOP = 0x10;           // Stop sign
        public const uint MB_ICONERROR = 0x10;          // Error indicator (stop sign)
        public const uint MB_ICONHAND = 0x10;           // Stop sign (was a "stop hand" in old Windows versions)
        public const uint MB_ICONQUESTION = 0x20;       // '?' in a circle (not recommended to use)
        public const uint MB_ICONEXCLAMATION = 0x30;    // Exclamation point
        public const uint MB_ICONWARNING = 0x30;        // Exclamation point
        public const uint MB_ICONINFORMATION = 0x40;    // 'i' in a circle
        public const uint MB_ICONASTERISK = 0x40;       // 'i' in a circle (was "asterisk in circle" in old Windows versions)
                                                        // Default button selection
        public const uint MB_DEFBUTTON1 = 0x000;        // First button is default
        public const uint MB_DEFBUTTON2 = 0x100;        // Second button is default
        public const uint MB_DEFBUTTON3 = 0x200;        // Third button is default
        public const uint MB_DEFBUTTON4 = 0x300;        // Fourth button is default
                                                        // Window modality
        public const uint MB_APPLMODAL = 0x0000;        // Modal, blocks parent and its other child windows
        public const uint MB_SYSTEMMODAL = 0x1000;      // Same as APPLMODAL, but box has WS_EX_TOPMOST style
        public const uint MB_TASKMODAL = 0x2000;        // Blocks all top-level windows in current thread, use when you don't have a window handle
                                                        // Return values

        /// <summary> Display a modal dialog box using direct Win32 calls. </summary>
        /// <param name="hWnd"> Native handle of the window that will own this one. </param>
        /// <param name="itsMessage"> Message to display in dialog. </param>
        /// <param name="itsCaption"> Text to display in title bar. </param>
        /// <param name="itsMessageBoxOptions"> Options controlling which icons/buttons are displayed. </param>
        /// <returns> Constant indicating which button the user clicked. </returns>
        private static MsgBoxRetVal SetupnCallMessageBoxTimeOut(IntPtr hWnd, string itsMessage, string itsCaption, uint itsMessageBoxOptions)
        {
            return (MsgBoxRetVal)MessageBoxEx(hWnd, itsMessage, itsCaption, itsMessageBoxOptions, 0);
        }

        /// <summary> Display a modal dialog box using direct Win32 calls. </summary>
        /// <remarks> Only use this version when the caller can't provide a parent window. </remarks>
        /// <param name="itsMessage"> Message to display in dialog. </param>
        /// <param name="itsCaption"> Text to display in title bar. </param>
        /// <param name="itsMessageBoxOptions"> Options controlling which icons/buttons are displayed. </param>
        /// <returns> Constant indicating which button the user clicked. </returns>
        private static MsgBoxRetVal SetupnCallMessageBoxTimeOut_NoParent(string itsMessage, string itsCaption, uint itsMessageBoxOptions)
        {
            return (MsgBoxRetVal)MessageBoxEx(nint.Zero, itsMessage, itsCaption, itsMessageBoxOptions | MB_TASKMODAL, 0);
        }

        #endregion

        /// <summary> Display a simple modal dialog box. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display in dialog. </param>
        /// <param name="title"> Text to display in title bar. </param>
        /// <param name="type"> Bitmask of options that control which icons/buttons the dialog will have. </param>
        /// <returns> Constant indicating which button the user clicked. </returns>
        public static MsgBoxRetVal MsgBox(Window? owner, string message, string title, uint type)
        {
            if (owner != null)
            {
                IntPtr? hwnd = owner.TryGetPlatformHandle()?.Handle;
                if (hwnd != null)
                    return SetupnCallMessageBoxTimeOut(hwnd.Value, message, title, type);
            }
            return SetupnCallMessageBoxTimeOut_NoParent(message, title, type);
        }

        #region Specific dialog types

        /// <summary> Display an error message. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static void ShowError(Window? owner, string message, string title = "Error") => MsgBox(owner, message, title, MB_ICONERROR);

        /// <summary> Display a warning message. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static void ShowWarning(Window? owner, string message, string title = "Warning") => MsgBox(owner, message, title, MB_ICONWARNING);

        /// <summary> Display an informational message. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static void ShowInfo(Window? owner, string message, string title = "Information") => MsgBox(owner, message, title, MB_ICONINFORMATION);

        /// <summary> Display a dialog asking a Yes/No question. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static MsgBoxRetVal ShowYesNo(Window? owner, string message, string title = "Question") => MsgBox(owner, message, title, MB_YESNO | MB_ICONQUESTION | MB_DEFBUTTON2);

        /// <summary> Display a dialog asking a Yes/No question with the option to cancel. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static MsgBoxRetVal ShowYesNoCancel(Window? owner, string message, string title = "Question") => MsgBox(owner, message, title, MB_YESNOCANCEL | MB_ICONQUESTION | MB_DEFBUTTON2);

        /// <summary> Display a dialog prompting the user to retry or cancel. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static MsgBoxRetVal ShowRetryCancel(Window? owner, string message, string title = "Retry?") => MsgBox(owner, message, title, MB_RETRYCANCEL);

        /// <summary> Display a dialog prompting the user to cancel, retry, or continue. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static MsgBoxRetVal ShowCancelRetryContinue(Window? owner, string message, string title = "Retry?") => MsgBox(owner, message, title, MB_CANCELTRYCONTINUE | MB_DEFBUTTON2);

        /// <summary> Display a dialog prompting the user to abort, retry, or ignore. </summary>
        /// <param name="owner"> Window that will own this dialog. </param>
        /// <param name="message"> Message to display. </param>
        /// <param name="title"> (optional) Text to display in title bar. </param>
        public static MsgBoxRetVal ShowAbortRetryIgnore(Window? owner, string message, string title = "Retry?") => MsgBox(owner, message, title, MB_ABORTRETRYIGNORE | MB_ICONEXCLAMATION | MB_DEFBUTTON2);

        #endregion
    }
}
