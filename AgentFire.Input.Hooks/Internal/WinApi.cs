using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AgentFire.Input.Hooks.Internal;

internal static class WinApi
{
    private static readonly nint _currentModuleHandle;

    static WinApi()
    {
        _currentModuleHandle = GetModuleHandle(null);
    }

    public enum HardwareHookType
    {
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14,
    }

    public enum KeyboardMessages
    {
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
    }

    public enum MouseMessages
    {
        //WM_MOUSEMOVE = 0x0200, // No.
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,

        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,

        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,

        WM_XBUTTONDOWN = 0x020b,
        WM_XBUTTONUP = 0x020c,
        //WM_MOUSEWHEEL = 0x020A, // We'll get to that later.
    }

    /// <summary>
    /// The set of valid MapTypes used in MapVirtualKey
    /// </summary>
    public enum MapVirtualKeyMapTypes : uint
    {
        /// <summary>
        /// uCode is a virtual-key code and is translated into a scan code.
        /// If it is a virtual-key code that does not distinguish between left- and
        /// right-hand keys, the left-hand scan code is returned.
        /// If there is no translation, the function returns 0.
        /// </summary>
        VkToVsc = 0x00,

        /// <summary>
        /// uCode is a scan code and is translated into a virtual-key code that
        /// does not distinguish between left- and right-hand keys. If there is no
        /// translation, the function returns 0.
        /// </summary>
        VscToVk = 0x01,

        /// <summary>
        /// uCode is a virtual-key code and is translated into an unshifted
        /// character value in the low-order word of the return value. Dead keys (diacritics)
        /// are indicated by setting the top bit of the return value. If there is no
        /// translation, the function returns 0.
        /// </summary>
        VkToChar = 0x02,

        /// <summary>
        /// Windows NT/2000/XP: uCode is a scan code and is translated into a
        /// virtual-key code that distinguishes between left- and right-hand keys. If
        /// there is no translation, the function returns 0.
        /// </summary>
        VcsToVkEx = 0x03,

        /// <summary>
        /// Not currently documented
        /// </summary>
        VkToVscEx = 0x04
    }

    /// <summary>
    /// Specifies the type of the input event. This member can be one of the following values. 
    /// </summary>
    public enum InputType
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
    public enum KeyboardFlag
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
    public struct Input
    {
        [FieldOffset(0)]
        public int type;

        [FieldOffset(sizeof(int))]
        public MouseParams mi;

        [FieldOffset(sizeof(int))]
        public KeyboardParams ki;

        [FieldOffset(sizeof(int))]
        public HardwareParams hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseParams
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardParams
    {
        public short wVk;
        public short wScan;
        public int dwFlags;
        public int time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareParams
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [Flags]
    public enum MouseFlag : uint
    {
        /// <summary>
        /// Specifies that movement occurred.
        /// </summary>
        Move = 1u,

        /// <summary>
        /// Specifies that the left button was pressed.
        /// </summary>
        LeftDown = 2u,

        /// <summary>
        /// Specifies that the left button was released.
        /// </summary>
        LeftUp = 4u,

        /// <summary>
        /// Specifies that the right button was pressed.
        /// </summary>
        RightDown = 8u,

        /// <summary>
        /// Specifies that the right button was released.
        /// </summary>
        RightUp = 16u,

        /// <summary>
        /// Specifies that the middle button was pressed.
        /// </summary>
        MiddleDown = 32u,

        /// <summary>
        /// Specifies that the middle button was released.
        /// </summary>
        MiddleUp = 64u,

        /// <summary>
        /// Windows 2000/XP: Specifies that an X button was pressed.
        /// </summary>
        XDown = 128u,

        /// <summary>
        /// Windows 2000/XP: Specifies that an X button was released.
        /// </summary>
        XUp = 256u,

        /// <summary>
        /// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
        /// </summary>
        VerticalWheel = 2048u,

        /// <summary>
        /// Specifies that the wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. Windows 2000/XP:  Not supported.
        /// </summary>
        HorizontalWheel = 4096u,

        /// <summary>
        /// Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
        /// </summary>
        VirtualDesk = 16384u,

        /// <summary>
        /// Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
        /// </summary>
        Absolute = 32768u
    }

    public static nint SetKeyboardHook(LowLevelProc proc)
    {
        return SetWindowsHookEx((int)HardwareHookType.WH_KEYBOARD_LL, proc, _currentModuleHandle, 0);
    }
    public static nint SetMouseHook(LowLevelProc proc)
    {
        return SetWindowsHookEx((int)HardwareHookType.WH_MOUSE_LL, proc, _currentModuleHandle, 0);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint SetWindowsHookEx(int idHook, LowLevelProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint GetModuleHandle(string? lpModuleName);

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
    public static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] Input[] pInputs, int cbSize);
}