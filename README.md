# Usage

1. Instantiate your keyboard hook.

```csharp
IDisposable keyboardHook = Hardware.HookKeyboard(ev => {
    // Your actions against the event of RawKeyboardEvent (ev).
});
```

2. Instantiate your mouse hook.

```csharp
IDisposable mouseHook = Hardware.HookMouse(ev => {
    // Your actions against the event of RawMouseEvent (ev).
});
```
    
It is that easy.
Now for the "what's the catch" part:

### 1. You have to call the factory (`Hardware.HookKeyboard()` / `Hardware.HookMouse()`) from a thread which has a running message loop

Assuming you are using WPF directly (from this [example code](https://github.com/AgentFire/AgentFire.Input.Hooks/blob/master/AgentFire.Input.Hooks.ManualTest/MainWindow.xaml.cs#L51):

```csharp
private readonly IDisposable _keyboardHook;

private void OnKey(RawKeyboardEvent ev)
{
    // Here you go.
}

public MainWindow()
{
    InitializeComponent();
     
    // As code is run on the app's main thread, the event's callbacks will also be called on that thread (via the Message Loop mechanism).
    // InvokeAsync posts the anonymous delegate asynchronously, which is my best attempt at releasing the event's callback as fast as possible.
    _keyboardHook = Hardware.HookKeyboard(ev => Dispatcher.InvokeAsync(() => OnKey(ev)));
}

protected override void OnClosed(EventArgs e)
{
    // Don't forget to clean up.
    _keyboardHook.Dispose();

    base.OnClosed(e);
}
```

Assuming you are using just the WPF's engine, here is a nice way to start a new thread and the required Message Loop inside it:

```csharp
private void OnMouse(RawMouseEvent ev)
{
    // Here you go.
    // This will run on the Message Loop's internal thread, so return as fast as possible.
}

(IDisposable mouseHookLoop, IDisposable mouseHook) = MessageLoop.Create(() => Hardware.HookMouse(OnMouse));

// Dispose of the hook when not needed anymore.
mouseHook.Dispose();

// Stop the message loop.
mouseHookLoop.Dispose();
```
    
### 3. Other notes

1. Don't forget that Global Hooks considered a bad practice since they hurt the performance system-wide.
2. Try to minimize your handler code. If you spend more than 1 ms on handling the message, consider using cross-thread message queue or posting a delegate elsewhere.
3. Feedback is appreciated.
4. Enjoy.