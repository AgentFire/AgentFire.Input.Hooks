namespace AgentFire.Input.Hooks
{
    public abstract class RawPressableEventArgs : RawInputEventArgs
    {
        public bool WasPressed { get; }

        private protected RawPressableEventArgs(bool wasPressed)
        {
            WasPressed = wasPressed;
        }
    }
}
