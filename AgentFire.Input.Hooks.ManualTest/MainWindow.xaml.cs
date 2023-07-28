using AgentFire.Input.Hooks.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AgentFire.Input.Hooks.ManualTest;

public partial class MainWindow : Window
{
    private readonly IDisposable _keyboardHook;
    private readonly IDisposable _mouseHook;

    public ObservableCollection<Key> Keyboard { get; } = new();
    public ObservableCollection<MouseButton> Mouse { get; } = new();

    private void OnKey(RawKeyboardEvent ev)
    {
        if (ev.WasPressed)
        {
            if (!Keyboard.Contains(ev.Key))
            {
                Keyboard.Add(ev.Key);
            }
        }
        else
        {
            Keyboard.Remove(ev.Key);
        }
    }
    private void OnMouse(RawMouseEvent ev)
    {
        if (ev.WasPressed)
        {
            if (!Mouse.Contains(ev.MouseButton))
            {
                Mouse.Add(ev.MouseButton);
            }
        }
        else
        {
            Mouse.Remove(ev.MouseButton);
        }
    }

    public MainWindow()
    {
        InitializeComponent();

        _keyboardHook = Hardware.HookKeyboard(ev => Dispatcher.InvokeAsync(() => OnKey(ev)));
        _mouseHook = Hardware.HookMouse(ev => Dispatcher.InvokeAsync(() => OnMouse(ev)));
    }

    protected override void OnClosed(EventArgs e)
    {
        _keyboardHook.Dispose();
        _mouseHook.Dispose();

        base.OnClosed(e);
    }
}