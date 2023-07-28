namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Represents a raw input event.
/// </summary>
/// <param name="WasPressed">Gets a value indicating if the input was activated or deactivated.</param>
/// <param name="WasInjected">Gets a value indicating if the input was injected using an API call.</param>
public abstract record RawInputEvent(bool WasPressed, bool WasInjected);