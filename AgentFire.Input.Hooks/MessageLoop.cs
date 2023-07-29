using System;
using System.Threading;
using System.Windows.Threading;

namespace AgentFire.Input.Hooks;

/// <summary>
/// Provides a way to explicitly create a new thread and the WPF's Message Loop on it.
/// </summary>
public static class MessageLoop
{
    #region Impl

    private sealed class Impl : IDisposable
    {
        private readonly Action _disposeActions;

        public Impl(Action disposeActions)
        {
            _disposeActions = disposeActions;
        }

        public void Dispose()
        {
            _disposeActions();
        }
    }

    #endregion

    /// <summary>
    /// Creates a new thread, starts WPF's Message Loop and performs <paramref name="initActions"/> on it.
    /// </summary>
    /// <typeparam name="T">Custom result type.</typeparam>
    /// <param name="initActions">Actions to perform on the new thread.</param>
    /// <returns><see cref="IDisposable"/> which is used to stop the Message Loop and the <typeparamref name="T"/> custom result.</returns>
    public static (IDisposable Loop, T Data) Create<T>(Func<T> initActions)
    {
        DispatcherFrame? frame = null;

        Thread thread = new Thread(() =>
        {
            frame = new DispatcherFrame(true);

            // Start the message loop.
            Dispatcher.PushFrame(frame);
        })
        {
            // This thread should be prioritized over the others to improve the callback's performance.
            Priority = ThreadPriority.Highest
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        while (frame is null)
        {
            Thread.Sleep(1);
        }

        T result = frame.Dispatcher.Invoke(initActions);

        return (new Impl(() =>
        {
            frame.Continue = false;
        }), result);
    }
}