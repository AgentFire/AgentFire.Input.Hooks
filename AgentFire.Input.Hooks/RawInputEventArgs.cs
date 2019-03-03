namespace AgentFire.Input.Hooks
{
    public abstract class RawInputEventArgs
    {
        public bool EatInput { get; set; } = false;

        private protected RawInputEventArgs() { }
    }
}
