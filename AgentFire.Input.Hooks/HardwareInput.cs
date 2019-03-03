using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AgentFire.Input.Hooks
{
    public static class HardwareInput
    {
        /// <summary>
        /// The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        private enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>
            /// uCode is a virtual-key code and is translated into a scan code.
            /// If it is a virtual-key code that does not distinguish between left- and
            /// right-hand keys, the left-hand scan code is returned.
            /// If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x00,

            /// <summary>
            /// uCode is a scan code and is translated into a virtual-key code that
            /// does not distinguish between left- and right-hand keys. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x01,

            /// <summary>
            /// uCode is a virtual-key code and is translated into an unshifted
            /// character value in the low-order word of the return value. Dead keys (diacritics)
            /// are indicated by setting the top bit of the return value. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x02,

            /// <summary>
            /// Windows NT/2000/XP: uCode is a scan code and is translated into a
            /// virtual-key code that distinguishes between left- and right-hand keys. If
            /// there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x03,

            /// <summary>
            /// Not currently documented
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04
        }

        /// <summary>
        /// Specifies the type of the input event. This member can be one of the following values. 
        /// </summary>
        private enum InputType
        {
            /// <summary>
            /// INPUT_MOUSE = 0x00 (The event is a mouse event. Use the mi structure of the union.)
            /// </summary>
            Mouse = 0,

            /// <summary>
            /// INPUT_KEYBOARD = 0x01 (The event is a keyboard event. Use the ki structure of the union.)
            /// </summary>
            Keyboard = 1,

            /// <summary>
            /// INPUT_HARDWARE = 0x02 (Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.)
            /// </summary>
            Hardware = 2,
        }

        [Flags]
        private enum KeyboardFlag
        {
            KeyDown = 0,

            /// <summary>
            /// KEYEVENTF_EXTENDEDKEY = 0x0001 (If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).)
            /// </summary>
            ExtendedKey = 0x0001,

            /// <summary>
            /// KEYEVENTF_KEYUP = 0x0002 (If specified, the key is being released. If not specified, the key is being pressed.)
            /// </summary>
            KeyUp = 0x0002,

            /// <summary>
            /// KEYEVENTF_UNICODE = 0x0004 (If specified, wScan identifies the key and wVk is ignored.)
            /// </summary>
            Unicode = 0x0004,

            /// <summary>
            /// KEYEVENTF_SCANCODE = 0x0008 (Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.)
            /// </summary>
            ScanCode = 0x0008,
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        [Flags]
        internal enum MouseFlag : uint
        {
            /// <summary>
            /// Specifies that movement occurred.
            /// </summary>
            // Token: 0x04000022 RID: 34
            Move = 1u,
            /// <summary>
            /// Specifies that the left button was pressed.
            /// </summary>
            // Token: 0x04000023 RID: 35
            LeftDown = 2u,
            /// <summary>
            /// Specifies that the left button was released.
            /// </summary>
            // Token: 0x04000024 RID: 36
            LeftUp = 4u,
            /// <summary>
            /// Specifies that the right button was pressed.
            /// </summary>
            // Token: 0x04000025 RID: 37
            RightDown = 8u,
            /// <summary>
            /// Specifies that the right button was released.
            /// </summary>
            // Token: 0x04000026 RID: 38
            RightUp = 16u,
            /// <summary>
            /// Specifies that the middle button was pressed.
            /// </summary>
            // Token: 0x04000027 RID: 39
            MiddleDown = 32u,
            /// <summary>
            /// Specifies that the middle button was released.
            /// </summary>
            // Token: 0x04000028 RID: 40
            MiddleUp = 64u,
            /// <summary>
            /// Windows 2000/XP: Specifies that an X button was pressed.
            /// </summary>
            // Token: 0x04000029 RID: 41
            XDown = 128u,
            /// <summary>
            /// Windows 2000/XP: Specifies that an X button was released.
            /// </summary>
            // Token: 0x0400002A RID: 42
            XUp = 256u,
            /// <summary>
            /// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
            /// </summary>
            // Token: 0x0400002B RID: 43
            VerticalWheel = 2048u,
            /// <summary>
            /// Specifies that the wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. Windows 2000/XP:  Not supported.
            /// </summary>
            // Token: 0x0400002C RID: 44
            HorizontalWheel = 4096u,
            /// <summary>
            /// Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
            /// </summary>
            // Token: 0x0400002D RID: 45
            VirtualDesk = 16384u,
            /// <summary>
            /// Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
            /// </summary>
            // Token: 0x0400002E RID: 46
            Absolute = 32768u
        }

        /// <summary>
        /// The MapVirtualKey function translates (maps) a virtual-key code into a scan
        /// code or character value, or translates a scan code into a virtual-key code    
        /// </summary>
        /// <param name="uCode">[in] Specifies the virtual-key code or scan code for a key.
        /// How this value is interpreted depends on the value of the uMapType parameter
        /// </param>
        /// <param name="uMapType">[in] Specifies the translation to perform. The value of this
        /// parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>Either a scan code, a virtual-key code, or a character value, depending on
        /// the value of uCode and uMapType. If there is no translation, the return value is zero
        /// </returns>
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, int cbSize);

        public static void KeyInput(Key key, bool keyPressedState)
        {
            int scan = MapVirtualKey((uint)KeyInterop.VirtualKeyFromKey(key), MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);

            INPUT[] InputData = new INPUT[1];

            InputData[0].type = (int)InputType.Keyboard;
            InputData[0].ki.wScan = (short)scan;
            InputData[0].ki.dwFlags = (int)(keyPressedState ? KeyboardFlag.KeyDown : KeyboardFlag.KeyUp) | (int)KeyboardFlag.ScanCode;
            InputData[0].ki.time = 0;
            InputData[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }
        public static void MouseInput(MouseButton button, bool buttonPressedState)
        {
            INPUT[] InputData = new INPUT[1];

            InputData[0].type = (int)InputType.Mouse;
            //InputData[0].mi.wScan = 0;
            MouseFlag mouseFlag = buttonPressedState ? ToMouseButtonDownFlag(button) : ToMouseButtonUpFlag(button);
            InputData[0].mi.dwFlags = (int)mouseFlag;
            //InputData[0].mi.time = 0;
            //InputData[0].mi.dwExtraInfo = IntPtr.Zero;

            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00002B40 File Offset: 0x00000D40
        private static MouseFlag ToMouseButtonDownFlag(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseFlag.LeftDown;
                case MouseButton.Middle:
                    return MouseFlag.MiddleDown;
                case MouseButton.Right:
                    return MouseFlag.RightDown;
                case MouseButton.XButton1:
                    return MouseFlag.XDown;
                default:
                    return MouseFlag.LeftDown;
            }
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00002B6C File Offset: 0x00000D6C
        private static MouseFlag ToMouseButtonUpFlag(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseFlag.LeftUp;
                case MouseButton.Middle:
                    return MouseFlag.MiddleUp;
                case MouseButton.Right:
                    return MouseFlag.RightUp;
                case MouseButton.XButton1:
                    return MouseFlag.XUp;
                default:
                    return MouseFlag.LeftUp;
            }
        }
    }
}
