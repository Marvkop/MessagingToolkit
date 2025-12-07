namespace MessagingToolkit.Async;

internal class DisposableMonitorLock : IDisposable
{
    private readonly object _obj;

    private DisposableMonitorLock(object obj)
    {
        _obj = obj;

        Monitor.Enter(obj);
    }

    public void Dispose()
    {
        Monitor.Exit(_obj);
    }

    public static IDisposable CreateDisposable(bool isThreadSafe, object obj)
    {
        return isThreadSafe ? new DisposableMonitorLock(obj) : NoOpDisposable.Instance;
    }
}