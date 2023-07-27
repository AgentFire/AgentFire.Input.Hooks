using AgentFire.Input.Hooks.Events;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace AgentFire.Input.Hooks.Internal;

internal sealed partial class MouseHardwareHook : HardwareHook, IDisposable
{
    private readonly Action<RawMouseEvent> _eventHandler;

    private protected override bool HookCallbackInternal(int code, IntPtr paramW, IntPtr paramL)
    {
        if (code < 0 || !Enum.IsDefined((WinApi.MouseMessages)paramW))
        {
            return true;
        }

        HookParams hookStruct = Marshal.PtrToStructure<HookParams>(paramL);

        (bool WasPressed, MouseButton Button)? data = (WinApi.MouseMessages)paramW switch
        {
            WinApi.MouseMessages.WM_LBUTTONDOWN => (true, MouseButton.Left),
            WinApi.MouseMessages.WM_LBUTTONUP => (false, MouseButton.Left),
            WinApi.MouseMessages.WM_RBUTTONDOWN => (true, MouseButton.Right),
            WinApi.MouseMessages.WM_RBUTTONUP => (false, MouseButton.Right),
            WinApi.MouseMessages.WM_MBUTTONDOWN => (true, MouseButton.Middle),
            WinApi.MouseMessages.WM_MBUTTONUP => (false, MouseButton.Middle),
            WinApi.MouseMessages.WM_XBUTTONDOWN => (true, hookStruct.mouseData >> 16 == 1 ? MouseButton.XButton1 : MouseButton.XButton2),
            WinApi.MouseMessages.WM_XBUTTONUP => (false, hookStruct.mouseData >> 16 == 1 ? MouseButton.XButton1 : MouseButton.XButton2),
            _ => null,
        };

        if (data is null)
        {
            return true;
        }

        Point point = new Point(hookStruct.pt.x, hookStruct.pt.y);
        RawMouseEvent args = new(data.Value.Button, point, data.Value.WasPressed);
        _eventHandler(args);
        return !args.WillEatInput;
    }

    /// <summary>
    /// Creates two input hooks: mouse and keyboard. Requires running Message Loop on the calling thread.
    /// </summary>
    internal MouseHardwareHook(Action<RawMouseEvent> eventHandler) : base(WinApi.SetMouseHook, id => WinApi.UnhookWindowsHookEx(id))
    {
        _eventHandler = eventHandler;
    }
}