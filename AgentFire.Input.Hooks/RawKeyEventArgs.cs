using System.Windows.Input;

namespace AgentFire.Input.Hooks
{
    public sealed class RawKeyEventArgs : RawPressableEventArgs
    {
        public Key Key { get; }

        internal RawKeyEventArgs(Key key, bool wasPressed) : base(wasPressed)
        {
            Key = key;
        }
    }
}
