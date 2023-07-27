namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Contains a <see cref="WasPressed"/> indicator.
/// </summary>
public abstract class RawPressableEventArgs : RawInputEventArgs
{
    /// <summary>
    /// Gets a value indicating if the input was activated or deactivated.
    /// </summary>
    public bool WasPressed { get; }

    private protected RawPressableEventArgs(bool wasPressed)
    {
        WasPressed = wasPressed;
    }
}