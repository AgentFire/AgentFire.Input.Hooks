using System;
using System.Runtime.InteropServices;

namespace AgentFire.Input.Hooks.Internal;

internal partial class MouseHardwareHook
{
    [StructLayout(LayoutKind.Sequential)]
    private struct HookParams
    {
        public PointParams pt;
        public int mouseData;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }
}