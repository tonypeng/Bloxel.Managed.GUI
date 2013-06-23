/*
 * Bloxel.Managed.GUI - KeyboardBuffer.Windows.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

#if WINDOWS
///-------------------------------------------------------------------------------------  
/// KeyboardBuffer.cs - provides buffered keyboard input XNA app via Win32 hooks  
/// Author: Erin Hastings <ejhastings@gmail.com>  
///   
/// Based on code by Ben Ryves:  
/// http://www.benryves.com/index.php?module=journal&mode=filtered&single_post=3024426  
///   
/// How to use:  
///   
/// (1) create a KeyboardBuffer object in your XNA game window class  
///     ex: KeyboardBuffer kb = new KeyboardBuffer(Window.Handle);  
///   
/// (2) enable the buffer to capture key presses  
///     ex: kb.Enable();  
///   
/// (3) add the captured in the buffer to a string  
///     ex: string text = kb.GetText();  
///       
/// (4) make sure to disable the buffer when user is not typing  
///     or the buffer may become extremely large (e.g. WASD movement keys)  
///     ex: kb.disable()  
///-------------------------------------------------------------------------------------  
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bloxel.Managed.GUI.Input
{
    public class KeyboardBuffer : IDisposable
    {
        public static KeyboardBuffer Singleton { get; internal set; }

        // the text buffer that user typed  
        // access contents with GetText()  
        string buffer = "";


        // true if backspace key was pressed  
        // provide special handling outside the keybuffer class  
        bool backSpaceKey = false;


        // true if enter key was pressed  
        // provide special handling outside the keybuffer class  
        bool enterKey = false;

        bool escapeKey = false;

        bool selectAll = false;

        bool leftKey = false;
        bool rightKey = false;

        // enable or disable buffer accumlation  
        // be sure to disable the buffer when user is not typing text  
        // or it will become very large (e.g. from WASD movement)  
        bool enableBuffer = false;


        ///-------------------------------------------------------------------------------------  
        /// Return true if backspace was pressed.  
        /// Deleting chars must be handled outside the keybuffer class.  
        ///-------------------------------------------------------------------------------------  
        public bool BackSpaceKey
        {
            get
            {
                bool b = backSpaceKey;
                backSpaceKey = false;
                return b;
            }
        }


        ///-------------------------------------------------------------------------------------  
        /// Return true if enter was pressed.  
        /// Fire events from the enter key outside the keybuffer class,  
        /// e.g. when the user is done typing and buffer is submitted for use  
        ///-------------------------------------------------------------------------------------  
        public bool EnterKey
        {
            get
            {
                bool b = enterKey;
                enterKey = false;
                return b;
            }
        }

        public bool EscapeKey
        {
            get
            {
                bool b = escapeKey;
                escapeKey = false;
                return b;
            }
        }

        public bool SelectAll
        {
            get
            {
                bool b = selectAll;
                selectAll = false;
                return b;
            }
        }

        public bool LeftKey
        {
            get
            {
                bool b = leftKey;
                leftKey = false;
                return b;
            }
        }

        public bool RightKey
        {
            get
            {
                bool b = rightKey;
                rightKey = false;
                return b;
            }
        }

        ///-------------------------------------------------------------------------------------  
        /// Return true if buffer enabled.  
        ///-------------------------------------------------------------------------------------  
        public bool Enabled
        {
            get
            {
                return enableBuffer;
            }
        }


        ///-------------------------------------------------------------------------------------  
        /// Returns the buffer contents since last time GetText() was called.  
        ///-------------------------------------------------------------------------------------  
        public string GetText()
        {
            string text = buffer;
            buffer = "";
            return text;
        }


        ///-------------------------------------------------------------------------------------  
        /// <summary>  
        /// Returns text with spaces filtered out.  
        /// </summary>  
        ///-------------------------------------------------------------------------------------  
        public string GetTextNoSpaces()
        {
            string text = buffer;
            buffer = "";
            if (text.Contains(" ")) text = "";
            return text;
        }


        ///-------------------------------------------------------------------------------------  
        /// <summary>  
        /// Returns text only digits.  
        /// </summary>  
        ///-------------------------------------------------------------------------------------  
        public string GetTextIntegersOnly()
        {
            string text = buffer;
            buffer = "";
            int i;
            bool numeric = Int32.TryParse(text, out i);
            if (numeric) return text;
            else return "";
        }


        ///-------------------------------------------------------------------------------------  
        /// Clears the buffer.  
        ///-------------------------------------------------------------------------------------  
        public void ClearBuffer()
        {
            buffer = "";
        }


        ///-------------------------------------------------------------------------------------  
        /// Enables the buffering of keys.   
        /// Buffer should only be enabled when the user is typing text.  
        /// Do not leave buffer accumulation on during the entire game!  
        ///-------------------------------------------------------------------------------------  
        public void Enable()
        {
            enableBuffer = true;
            buffer = "";

            if (HookHandle != IntPtr.Zero)
                throw new Exception("Hook handle already existed.");

            HookHandle = SetWindowsHookEx(HookId.WH_GETMESSAGE, this.ProcessMessagesCallback,
                                               IntPtr.Zero, GetCurrentThreadId());
        }


        ///-------------------------------------------------------------------------------------  
        /// Disables the buffering of keys  
        /// Buffer should be disabled when the user is not typing text!  
        ///-------------------------------------------------------------------------------------  
        public void Disable()
        {
            enableBuffer = false;
            buffer = "";

            if (HookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(HookHandle);
                HookHandle = IntPtr.Zero;
            }
        }
        #region Win32


        ///-------------------------------------------------------------------------------------  
        /// Types of hook that can be installed using the SetWindwsHookEx function.  
        ///-------------------------------------------------------------------------------------  
        public enum HookId
        {
            WH_CALLWNDPROC = 4,
            WH_CALLWNDPROCRET = 12,
            WH_CBT = 5,
            WH_DEBUG = 9,
            WH_FOREGROUNDIDLE = 11,
            WH_GETMESSAGE = 3,
            WH_HARDWARE = 8,
            WH_JOURNALPLAYBACK = 1,
            WH_JOURNALRECORD = 0,
            WH_KEYBOARD = 2,
            WH_KEYBOARD_LL = 13,
            WH_MAX = 11,
            WH_MAXHOOK = WH_MAX,
            WH_MIN = -1,
            WH_MINHOOK = WH_MIN,
            WH_MOUSE_LL = 14,
            WH_MSGFILTER = -1,
            WH_SHELL = 10,
            WH_SYSMSGFILTER = 6,
        };


        ///-------------------------------------------------------------------------------------  
        /// A subset of relevant Windows message types.  
        ///-------------------------------------------------------------------------------------  
        public enum WindowMessage
        {
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
        };


        ///-------------------------------------------------------------------------------------  
        /// A delegate used to create a hook callback.  
        ///-------------------------------------------------------------------------------------  
        public delegate int GetMsgProc(int nCode, int wParam, ref Message msg);


        ///-------------------------------------------------------------------------------------  
        /// Install an application-defined hook procedure into a hook chain.  
        /// If the function succeeds, the return value is the handle to the hook procedure.   
        /// Otherwise returns 0.  
        ///  
        /// idHook : Specifies the type of hook procedure to be installed.  
        /// lpfn : Pointer to the hook procedure.  
        /// hmod : Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.  
        /// dwThreadId : Specifies the identifier of the thread with which the hook procedure is  
        ///               to be associated.  
        ///-------------------------------------------------------------------------------------  
        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExA")]
        public static extern IntPtr SetWindowsHookEx(HookId idHook, GetMsgProc lpfn, IntPtr hmod, int dwThreadId);


        ///-------------------------------------------------------------------------------------  
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.   
        /// If the function fails, the return value is zero. To get extended error information,  
        /// call GetLastError.  
        ///   
        /// hHook : Handle to the hook to be removed. This parameter is a hook handle obtained   
        ///         by a previous call to SetWindowsHookEx.  
        ///-------------------------------------------------------------------------------------  
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hHook);


        ///-------------------------------------------------------------------------------------  
        /// Passes the hook information to the next hook procedure in the current hook chain.  
        /// Return : This value is returned by the next hook procedure in the chain.  
        ///   
        /// hHook : Ignored.  
        /// ncode : Specifies the hook code passed to the current hook procedure.  
        /// wParam : Specifies the wParam value passed to the current hook procedure.  
        /// lParam : Specifies the lParam value passed to the current hook procedure.  
        ///-------------------------------------------------------------------------------------  
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int hHook, int ncode, int wParam, ref Message lParam);


        ///-------------------------------------------------------------------------------------  
        /// Translates virtual-key messages into character messages.  
        /// If the message is translated (that is, a character message is posted to the thread's  
        /// message queue), the return value is true.  
        ///   
        /// lpMsg : Pointer to an Message structure that contains message information retrieved   
        ///         from the calling thread's message queue.  
        ///-------------------------------------------------------------------------------------  
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref Message lpMsg);


        [DllImport("user32.dll")]
        public static extern short GetKeyState(int key);

        ///-------------------------------------------------------------------------------------  
        /// <summary>  
        /// Retrieves the thread identifier of the calling thread.  
        /// </summary>  
        /// <returns>The thread identifier of the calling thread.</returns>  
        ///-------------------------------------------------------------------------------------  
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        #endregion


        #region Hook management and class construction.


        ///-------------------------------------------------------------------------------------  
        /// Handle for the created hook  
        //-------------------------------------------------------------------------------------  
        private static IntPtr HookHandle;
        private readonly GetMsgProc ProcessMessagesCallback;


        ///-------------------------------------------------------------------------------------  
        /// Create an instance of the TextInputHandler.  
        /// Handle of the window you wish to receive messages (and thus keyboard input) from.  
        ///-------------------------------------------------------------------------------------  
        public KeyboardBuffer(IntPtr whnd)
        {
            // create the delegate callback  
            // create the keyboard hook  
            ProcessMessagesCallback = new GetMsgProc(ProcessMessages);
            //this.HookHandle = SetWindowsHookEx(HookId.WH_GETMESSAGE, this.ProcessMessagesCallback,
            //IntPtr.Zero, GetCurrentThreadId());
        }


        ///-------------------------------------------------------------------------------------  
        /// Remove the keyboard hook.  
        ///-------------------------------------------------------------------------------------  
        public void Dispose()
        {
            if (HookHandle != IntPtr.Zero) UnhookWindowsHookEx(HookHandle);

            HookHandle = IntPtr.Zero;
        }
        #endregion


        #region Message processing


        ///-------------------------------------------------------------------------------------  
        /// Process Windows messages.  
        ///-------------------------------------------------------------------------------------  
        private int ProcessMessages(int nCode, int wParam, ref Message msg)
        {
            // Check if we must process this message (and whether it has been retrieved via GetMessage):  
            if (nCode == 0 && wParam == 1)
            {
                // We need character input, so use TranslateMessage to generate WM_CHAR messages.  
                TranslateMessage(ref msg);

                // If it's one of the keyboard-related messages, raise an event for it:  
                switch ((WindowMessage)msg.Msg)
                {
                    case WindowMessage.WM_CHAR:
                        this.OnKeyPress(new KeyPressEventArgs((char)msg.WParam));
                        break;
                    case WindowMessage.WM_KEYDOWN:
                        this.OnKeyDown(new KeyEventArgs((Keys)msg.WParam));
                        break;
                    case WindowMessage.WM_KEYUP:
                        this.OnKeyUp(new KeyEventArgs((Keys)msg.WParam));
                        break;
                }
            }

            // Call next hook in chain:  
            return CallNextHookEx(0, nCode, wParam, ref msg);
        }
        #endregion


        #region Events


        ///-------------------------------------------------------------------------------------  
        /// KeyUp event.  
        ///-------------------------------------------------------------------------------------  
        public event KeyEventHandler KeyUp;
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (this.KeyUp != null) this.KeyUp(this, e);
        }


        ///-------------------------------------------------------------------------------------  
        /// KeyDown event.  
        ///-------------------------------------------------------------------------------------  
        public event KeyEventHandler KeyDown;
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (enableBuffer)
            {
                if (this.KeyDown != null) this.KeyDown(this, e);

                if (e.KeyCode == Keys.Left)
                {
                    leftKey = true;
                }

                if (e.KeyCode == Keys.Right)
                {
                    rightKey = true;
                }
            }
        }


        ///-------------------------------------------------------------------------------------  
        /// KeyPress event.  
        /// Only processes events if this buffer is enabled.  
        ///-------------------------------------------------------------------------------------  
        public event KeyPressEventHandler KeyPress;
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            if (enableBuffer)
            {
                if (this.KeyPress != null) this.KeyPress(this, e);

                // if user pressed backspace  
                // set backspace to true  
                if (e.KeyChar.GetHashCode().ToString() == "524296")
                {
                    backSpaceKey = true;
                }
                else if (e.KeyChar.GetHashCode().ToString() == "851981")
                {
                    enterKey = true;
                }
                else if (e.KeyChar == (char)27)
                {
                    escapeKey = true;
                }
                else if (e.KeyChar < 32)
                {
                    if (e.KeyChar == 1)
                        selectAll = true;
                }
                else
                {
                    buffer += e.KeyChar;
                }
            }
        }
        #endregion
    }
}
#endif