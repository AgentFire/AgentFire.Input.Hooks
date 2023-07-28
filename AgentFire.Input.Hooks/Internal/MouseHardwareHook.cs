using AgentFire.Input.Hooks.Events;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static AgentFire.Input.Hooks.Internal.WinApi;

namespace AgentFire.Input.Hooks.Internal;

internal sealed partial class MouseHardwareHook : HardwareHook, IDisposable
{
    private readonly Action<RawMouseEvent> _eventHandler;

    private protected override void HookCallbackInternal(int code, IntPtr paramW, IntPtr paramL)
    {
        if (code < 0 || !Enum.IsDefined((MouseMessages)paramW))
        {
            return;
        }

        var hookParams = Marshal.PtrToStructure<HookParams>(paramL);

        (bool WasPressed, MouseButton Button)? data = (MouseMessages)paramW switch
        {
            MouseMessages.WM_LBUTTONDOWN => (true, MouseButton.Left),
            MouseMessages.WM_LBUTTONUP => (false, MouseButton.Left),
            MouseMessages.WM_RBUTTONDOWN => (true, MouseButton.Right),
            MouseMessages.WM_RBUTTONUP => (false, MouseButton.Right),
            MouseMessages.WM_MBUTTONDOWN => (true, MouseButton.Middle),
            MouseMessages.WM_MBUTTONUP => (false, MouseButton.Middle),
            MouseMessages.WM_XBUTTONDOWN => (true, hookParams.MouseData >> 16 == 1 ? MouseButton.XButton1 : MouseButton.XButton2),
            MouseMessages.WM_XBUTTONUP => (false, hookParams.MouseData >> 16 == 1 ? MouseButton.XButton1 : MouseButton.XButton2),
            _ => null,
        };

        if (data is null)
        {
            return;
        }

        bool wasInjected = hookParams.Flags.HasFlag(HookFlags.Injected);
        Point point = new Point(hookParams.Point.X, hookParams.Point.Y);

        _eventHandler(new(data.Value.Button, point, data.Value.WasPressed, wasInjected));
    }

    /// <summary>
    /// Creates two input hooks: mouse and keyboard. Requires running Message Loop on the calling thread.
    /// </summary>
    internal MouseHardwareHook(Action<RawMouseEvent> eventHandler) : base(SetMouseHook, id => UnhookWindowsHookEx(id))
    {
        _eventHandler = eventHandler;
    }
}