using AgentFire.Input.Hooks.Events;
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static AgentFire.Input.Hooks.Internal.WinApi;

namespace AgentFire.Input.Hooks.Internal;

internal sealed class KeyboardHardwareHook : HardwareHook
{
    private readonly Action<RawKeyboardEvent> _eventHandler;

    private protected override void HookCallbackInternal(int code, nint paramW, nint paramL)
    {
        if (code < 0 || !Enum.IsDefined((KeyboardMessages)paramW))
        {
            return;
        }

        bool wasPressed = (KeyboardMessages)paramW is KeyboardMessages.WM_KEYDOWN or KeyboardMessages.WM_SYSKEYDOWN;

        var hookParams = Marshal.PtrToStructure<KeyboardHookParams>(paramL);

        Key key = KeyInterop.KeyFromVirtualKey((int)hookParams.VkCode);
        bool wasInjected = hookParams.Flags.HasFlag(HookFlags.Injected);

        _eventHandler(new(key, wasPressed, wasInjected));
    }

    internal KeyboardHardwareHook(Action<RawKeyboardEvent> eventHandler) : base(SetKeyboardHook, id => UnhookWindowsHookEx(id))
    {
        _eventHandler = eventHandler;
    }
}