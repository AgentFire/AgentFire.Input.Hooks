using AgentFire.Input.Hooks.Events;
using Nif.Disposable;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace AgentFire.Input.Hooks;

/// <summary>
/// This is your beloved hook. Consult <a href="https://github.com/AgentFire/AgentFire.Input.Hooks">https://github.com/AgentFire/AgentFire.Input.Hooks</a> for usage.
/// </summary>
public sealed class HardwareHook : Disposable
{
    private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static readonly IntPtr _eatInputValue = (IntPtr)1;

    #region Some Handle(r)s and Event Declaration

    /// <summary>
    /// Happens when a keyboard key action is detected. The event's subscribers are called on <see cref="HardwareHook"/>'s own thread, so there.
    /// </summary>
    public event EventHandler<RawKeyEventArgs> KeyEvent;

    /// <summary>
    /// Happens when a mouse button action is detected. The event's subscribers are called on <see cref="HardwareHook"/>'s own thread, so there.
    /// </summary>
    public event EventHandler<RawMouseButtonEventArgs> MouseEvent;

    private readonly IntPtr _keyboardHookId;
    private readonly IntPtr _mouseHookId;

    #endregion
    #region Some Mouse Structs

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }
    #endregion
    #region Win API

    private static class WinAPI
    {
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

        public static IntPtr SetKeyboardHook(LowLevelProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx((int)HardwareHookType.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
        public static IntPtr SetMouseHook(LowLevelProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx((int)HardwareHookType.WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    #endregion

    [MethodImpl(MethodImplOptions.NoInlining)]
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        bool eatInput = false;

        if (nCode >= 0)
        {
            #region Process Keyboard Event

            if (IsKeyboardEvent(wParam))
            {
                bool wasPressed = (WinAPI.KeyboardMessages)wParam is WinAPI.KeyboardMessages.WM_KEYDOWN or WinAPI.KeyboardMessages.WM_SYSKEYDOWN;

                Key key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));

                RawKeyEventArgs args = null;
                KeyEvent?.Invoke(this, args = new RawKeyEventArgs(key, wasPressed));
                eatInput = args != null && args.EatInput;
            }

            #endregion
            #region Process Mouse Event

            if (IsMouseEvent(wParam))
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                Point point = new Point(hookStruct.pt.x, hookStruct.pt.y);
                bool wasPressed;
                int xButton;
                MouseButton button;

                switch ((WinAPI.MouseMessages)wParam)
                {
                    case WinAPI.MouseMessages.WM_LBUTTONDOWN:
                        wasPressed = true;
                        button = MouseButton.Left;
                        break;
                    case WinAPI.MouseMessages.WM_LBUTTONUP:
                        wasPressed = false;
                        button = MouseButton.Left;
                        break;
                    case WinAPI.MouseMessages.WM_RBUTTONDOWN:
                        wasPressed = true;
                        button = MouseButton.Right;
                        break;
                    case WinAPI.MouseMessages.WM_RBUTTONUP:
                        wasPressed = false;
                        button = MouseButton.Right;
                        break;
                    case WinAPI.MouseMessages.WM_MBUTTONDOWN:
                        wasPressed = true;
                        button = MouseButton.Middle;
                        break;
                    case WinAPI.MouseMessages.WM_MBUTTONUP:
                        wasPressed = false;
                        button = MouseButton.Middle;
                        break;
                    case WinAPI.MouseMessages.WM_XBUTTONDOWN:
                        wasPressed = true;
                        xButton = hookStruct.mouseData >> 16;
                        button = xButton == 1 ? MouseButton.XButton1 : MouseButton.XButton2;
                        break;
                    case WinAPI.MouseMessages.WM_XBUTTONUP:
                        wasPressed = false;
                        xButton = hookStruct.mouseData >> 16;
                        button = xButton == 1 ? MouseButton.XButton1 : MouseButton.XButton2;
                        break;
                    default:
                        return IntPtr.Zero; // This will never hit.
                }

                RawMouseButtonEventArgs args = null;
                MouseEvent?.Invoke(this, args = new RawMouseButtonEventArgs(button, point, wasPressed));
                eatInput = args != null && args.EatInput;
            }

            #endregion
        }

        return eatInput ? _eatInputValue : WinAPI.CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsKeyboardEvent(IntPtr wParam)
    {
        // TODO: optimize this.
        return Enum.IsDefined(typeof(WinAPI.KeyboardMessages), (WinAPI.KeyboardMessages)wParam);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMouseEvent(IntPtr wParam)
    {
        // TODO: optimize this.
        return Enum.IsDefined(typeof(WinAPI.MouseMessages), (WinAPI.MouseMessages)wParam);
    }

    /// <summary>
    /// Do not remove this field as it should remain here for successful unmanaged callbacks.
    /// </summary>
    private LowLevelProc _proc;

    /// <summary>
    /// Creates two input hooks: mouse and keyboard. Requires running Message Loop on the calling thread.
    /// </summary>
    public HardwareHook()
    {
        _proc = HookCallback;

        _keyboardHookId = WinAPI.SetKeyboardHook(_proc);
        _mouseHookId = WinAPI.SetMouseHook(_proc);
    }

    /// <inheritdoc/>
    protected override void DisposeUnmanagedResources()
    {
        base.DisposeUnmanagedResources();

        WinAPI.UnhookWindowsHookEx(_keyboardHookId);
        WinAPI.UnhookWindowsHookEx(_mouseHookId);

        _proc = null;
    }
}