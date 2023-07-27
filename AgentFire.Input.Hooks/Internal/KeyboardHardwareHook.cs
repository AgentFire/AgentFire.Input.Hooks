using AgentFire.Input.Hooks.Events;
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AgentFire.Input.Hooks.Internal;

internal sealed class KeyboardHardwareHook : HardwareHook
{
    private readonly Action<RawKeyboardEvent> _eventHandler;

    private protected override bool HookCallbackInternal(int code, nint paramW, nint paramL)
    {
        if (code < 0 || !Enum.IsDefined((WinApi.KeyboardMessages)paramW))
        {
            return true;
        }

        bool wasPressed = (WinApi.KeyboardMessages)paramW is WinApi.KeyboardMessages.WM_KEYDOWN or WinApi.KeyboardMessages.WM_SYSKEYDOWN;

        Key key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(paramL));

        RawKeyboardEvent args = new(key, wasPressed);
        _eventHandler(args);
        return !args.WillEatInput;
    }

    internal KeyboardHardwareHook(Action<RawKeyboardEvent> eventHandler) : base(WinApi.SetKeyboardHook, id => WinApi.UnhookWindowsHookEx(id))
    {
        _eventHandler = eventHandler;
    }
}