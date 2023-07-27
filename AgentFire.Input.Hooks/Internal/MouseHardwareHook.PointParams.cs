using System.Runtime.InteropServices;

namespace AgentFire.Input.Hooks.Internal;

internal partial class MouseHardwareHook
{
    [StructLayout(LayoutKind.Sequential)]
    private struct PointParams
    {
        public int x;
        public int y;
    }
}