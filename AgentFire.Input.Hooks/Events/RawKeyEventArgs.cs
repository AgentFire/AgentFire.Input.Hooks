using System.Windows.Input;

namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Keyboard event.
/// </summary>
public sealed class RawKeyEventArgs : RawPressableEventArgs
{
    /// <summary>
    /// Which keyboard key action was detected.
    /// </summary>
    public Key Key { get; }

    internal RawKeyEventArgs(Key key, bool wasPressed) : base(wasPressed)
    {
        Key = key;
    }
}