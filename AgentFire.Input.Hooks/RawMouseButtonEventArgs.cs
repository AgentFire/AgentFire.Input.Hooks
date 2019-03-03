using System.Windows;
using System.Windows.Input;

namespace AgentFire.Input.Hooks
{
    public sealed class RawMouseButtonEventArgs : RawPressableEventArgs
    {
        public MouseButton MouseButton { get; }
        public Point MousePosition { get; }

        internal RawMouseButtonEventArgs(MouseButton mouseButton, Point mousePosition, bool wasPressed) : base(wasPressed)
        {
            MouseButton = mouseButton;
            MousePosition = mousePosition;
        }
    }
}
