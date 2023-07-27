using System.Windows;
using System.Windows.Input;

namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Mouse event.
/// </summary>
public sealed class RawMouseButtonEventArgs : RawPressableEventArgs
{
    /// <summary>
    /// Which mouse button action was detected.
    /// </summary>
    public MouseButton MouseButton { get; }

    /// <summary>
    /// The position of the mouse at the moment.
    /// </summary>
    public Point MousePosition { get; }

    internal RawMouseButtonEventArgs(MouseButton mouseButton, Point mousePosition, bool wasPressed) : base(wasPressed)
    {
        MouseButton = mouseButton;
        MousePosition = mousePosition;
    }
}