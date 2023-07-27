using AgentFire.Input.Hooks.Internal;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AgentFire.Input.Hooks;

/// <summary>
/// Provides a way to simulate key strokes and mouse clicks.
/// </summary>
public static class HardwareInput
{
    /// <summary>
    /// Simulates keyboard.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="isPressed">Specify <see langword="true"/> for press, <see langword="false"/> for release.</param>
    public static void KeyInput(Key key, bool isPressed)
    {
        int scan = WinApi.MapVirtualKey((uint)KeyInterop.VirtualKeyFromKey(key), WinApi.MapVirtualKeyMapTypes.VkToVsc);

        WinApi.Input[] InputData = new WinApi.Input[1];

        InputData[0].type = (int)InputType.Keyboard;
        InputData[0].ki.wScan = (short)scan;
        InputData[0].ki.dwFlags = (int)(isPressed ? WinApi.KeyboardFlag.KeyDown : WinApi.KeyboardFlag.KeyUp) | (int)WinApi.KeyboardFlag.ScanCode;
        InputData[0].ki.time = 0;
        InputData[0].ki.dwExtraInfo = 0;

        _ = WinApi.SendInput(1, InputData, Marshal.SizeOf(typeof(WinApi.Input)));
    }

    /// <summary>
    /// Simulates mouse.
    /// </summary>
    /// <param name="button">Button</param>
    /// <param name="isPressed">Specify <see langword="true"/> for press, <see langword="false"/> for release.</param>
    public static void MouseInput(MouseButton button, bool isPressed)
    {
        WinApi.Input[] InputData = new WinApi.Input[1];

        InputData[0].type = (int)InputType.Mouse;
        //InputData[0].mi.wScan = 0;
        WinApi.MouseFlag MouseFlag = isPressed ? ToMouseButtonDownFlag(button) : ToMouseButtonUpFlag(button);
        InputData[0].mi.dwFlags = (int)MouseFlag;
        //InputData[0].mi.time = 0;
        //InputData[0].mi.dwExtraInfo = IntPtr.Zero;

        _ = WinApi.SendInput(1, InputData, Marshal.SizeOf(typeof(WinApi.Input)));
    }

    private static WinApi.MouseFlag ToMouseButtonDownFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WinApi.MouseFlag.LeftDown,
            MouseButton.Middle => WinApi.MouseFlag.MiddleDown,
            MouseButton.Right => WinApi.MouseFlag.RightDown,
            MouseButton.XButton1 => WinApi.MouseFlag.XDown,
            MouseButton.XButton2 => WinApi.MouseFlag.XDown,
            _ => WinApi.MouseFlag.LeftDown,
        };
    }

    private static WinApi.MouseFlag ToMouseButtonUpFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WinApi.MouseFlag.LeftUp,
            MouseButton.Middle => WinApi.MouseFlag.MiddleUp,
            MouseButton.Right => WinApi.MouseFlag.RightUp,
            MouseButton.XButton1 => WinApi.MouseFlag.XUp,
            MouseButton.XButton2 => WinApi.MouseFlag.XUp,
            _ => WinApi.MouseFlag.LeftUp,
        };
    }
}