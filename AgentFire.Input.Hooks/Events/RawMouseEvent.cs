using System.Windows;
using System.Windows.Input;

namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Keyboard event.
/// </summary>
/// <param name="MouseButton">Which mouse button action was detected.</param>
/// <param name="Point">Mouse position.</param>
/// <param name="WasPressed"><inheritdoc/></param>
public sealed record RawMouseEvent(MouseButton MouseButton, Point Point, bool WasPressed) : RawInputEvent(WasPressed);