using AgentFire.Input.Hooks.Events;
using AgentFire.Input.Hooks.Internal;
using System;

namespace AgentFire.Input.Hooks;

/// <summary>
/// Hardware hooks factory.
/// The event's subscribers are called on the Message Loop's thread, so beware.
/// Also PLEASE make sure you release the event callback as fast as possible to reduce system lags.
/// </summary>
public static class Hardware
{
    /// <summary>
    /// Creates mouse hook.
    /// </summary>
    ///<param name="eventHandler">Happens when a mouse button action is detected.</param>
    /// <returns>MouseHardwareHook</returns>
    public static IDisposable HookMouse(Action<RawMouseEvent> eventHandler) => new MouseHardwareHook(eventHandler);

    /// <summary>
    /// Creates keyboard hook.
    /// </summary>
    /// <param name="eventHandler">Happens when a keyboard key action is detected.</param>
    /// <returns>KeyboardHardwareHook</returns>
    public static IDisposable HookKeyboard(Action<RawKeyboardEvent> eventHandler) => new KeyboardHardwareHook(eventHandler);
}