namespace MessagingToolkit.Extensions;

public static class ReaderWriterLockSlimExtensions
{
    public static IDisposable ReadLock(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterReadLock();

        return new DisposableAction(@lock.ExitReadLock);
    }

    public static IDisposable WriteLock(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterReadLock();

        return new DisposableAction(@lock.ExitWriteLock);
    }

    private class DisposableAction(Action action) : IDisposable
    {
        public void Dispose()
        {
            action();
        }
    }
}