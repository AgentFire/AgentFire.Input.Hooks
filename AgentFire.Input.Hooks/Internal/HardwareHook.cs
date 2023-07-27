using System;

namespace AgentFire.Input.Hooks.Internal;

/// <summary>
/// Consult <a href="https://github.com/AgentFire/AgentFire.Input.Hooks">https://github.com/AgentFire/AgentFire.Input.Hooks</a> for usage.
/// </summary>
internal abstract class HardwareHook : IDisposable
{
    private static readonly nint _eatInputValue = 1;

    private readonly nint _hookId;
    private readonly Action<nint> _unhookCall;
    private LowLevelProc? _callback;

    private nint HookCallback(int code, nint paramW, nint paramL)
    {
        return HookCallbackInternal(code, paramW, paramL) ? WinApi.CallNextHookEx(_hookId, code, paramW, paramL) : _eatInputValue;
    }

    private protected abstract bool HookCallbackInternal(int code, nint paramW, nint paramL);

    private protected HardwareHook(Func<LowLevelProc, nint> hookCall, Action<nint> unhookCall)
    {
        _callback = HookCallback;
        _unhookCall = unhookCall;

        _hookId = hookCall(_callback);
    }

    /// <summary>
    /// Disposes of the object.
    /// </summary>
    public void Dispose()
    {
        _unhookCall(_hookId);
        _callback = null;
        GC.SuppressFinalize(this);
    }
}