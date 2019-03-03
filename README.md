# Usage

1. Instantiate your application-wide hook.

```csharp
HardwareHook hook = new HardwareHook();
```
    
2. Use it's events.

```csharp
_hook.KeyEvent += Hook_KeyEvent; // Your handler
_hook.MouseEvent += Hook_MouseEvent; // Your handler
```
    
3. Eat an input if you like.

```csharp
private void Hook_KeyEvent(object sender, RawKeyEventArgs args)
{
    if (args.Key == Key.CapsLock)
    {
        args.EatInput = true;
    }
}
```
    
It is that easy.
Now for the "what's the catch" part:

### 1. You have to call the constructor (`new HardwareHook()`) from a thread which has a running message loop

Assuming you are using WPF's engine, here is a nice way to start a new thread and the loop inside it:

```csharp
using (ManualResetEvent mre = new ManualResetEvent(false))
{
    Thread thread = new Thread(() =>
    {
        // Initiate the hook and the frame on this thread.
        _hook = new HardwareHook();
        _frame = new DispatcherFrame(true);

        // Proceed with the caller method.
        mre.Set();

        // Start the message loop.
        Dispatcher.PushFrame(_frame);
    })
    {
        IsBackground = false,

        // This thread should be more prioritized over UI thread and all others.
        Priority = ThreadPriority.Highest
    };

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();

    mre.WaitOne();

    _hook.KeyEvent += Hook_KeyEvent; // Your handler
    _hook.MouseEvent += Hook_MouseEvent; // Your handler
}
```
    
### 2. Don't forget to clean up

```csharp
// Unsubscribe.
_hook.KeyEvent -= Hook_KeyEvent;
_hook.MouseEvent -= Hook_MouseEvent;

// Stop the message loop if you want.
_frame.Continue = false;

// The dispose itself.
_hook.Dispose();
```
    
### 3. Other notes

1. Don't forget that Global Hooks considered a bad practice since they hurt the performance system-wide.
2. Try to minimize your handler code. If you spend more than 1 ms on handling the message, consider using cross-thread message queue.
3. That's why I'm creating a new thread in my example above, so that no UI thread would be a part of this.
4. Don't create more than one hook per application. I just can't imagine why would one do that.
5. Feedback is appreciated.
6. Enjoy.
