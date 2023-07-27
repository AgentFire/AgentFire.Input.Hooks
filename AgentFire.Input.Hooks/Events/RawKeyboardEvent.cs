using System.Windows.Input;

namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Keyboard event.
/// </summary>
/// <param name="Key">Keyboard key.</param>
/// <param name="WasPressed"><inheritdoc/></param>
public sealed record RawKeyboardEvent(Key Key, bool WasPressed) : RawInputEvent(WasPressed);