namespace AgentFire.Input.Hooks.Events;

/// <summary>
/// Allows to eat raw input event.
/// </summary>
/// <param name="WasPressed">Gets a value indicating if the input was activated or deactivated.</param>
public abstract record RawInputEvent(bool WasPressed)
{
    /// <summary>
    /// Gets or sets a value indicating if the Hook should eat the input. That is, if other applications will "see" it.
    /// Default value is <see langword="false"/>. Use <see cref="EatInput"/> to enable it.
    /// </summary>
    public bool WillEatInput { get; private set; } = false;

    /// <summary>
    /// Provides a way to nullify the input for other applications. Sets <see cref="WillEatInput"/> to <see langword="true"/>.
    /// </summary>
    public void EatInput()
    {
        WillEatInput = true;
    }
}