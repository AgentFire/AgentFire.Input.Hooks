using System;

namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Allows to eat raw input event.
/// </summary>
public abstract class RawInputEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating if the Hook should eat the input. That is, if other applications will "see" it.
    /// </summary>
    public bool EatInput { get; set; } = false;

    private protected RawInputEventArgs() { }
}