using System.Windows.Threading;
using MessagingToolkit.Handlers;
using MessagingToolkit.Handlers.Messaging;
using MessagingToolkit.Messengers;
using MessagingToolkit.Messengers.Interfaces;

namespace MessagingToolkit.Wpf.Messengers;

/// <summary>
/// Implementation of <see cref="IMessenger"/> using weak references.
/// <br />
/// Cleanup is called automatically on the dispatcher with  
/// </summary>
public sealed class AutoWeakReferenceMessenger() : WeakReferenceMessenger(true)
{
    private bool _isCleanupRequested;

    /// <inheritdoc/>
    public override void Publish<T>(T message)
    {
        base.Publish(message);
        RequestCleanup();
    }

    /// <inheritdoc/>
    public override async Task PublishAsync<T>(T message)
    {
        await base.PublishAsync(message);
        RequestCleanup();
    }

    /// <inheritdoc/>
    public override void Register<T>(object recipient, Action<T> action)
    {
        base.Register(recipient, action);
        RequestCleanup();
    }

    /// <inheritdoc/>
    public override void Register<T>(object recipient, IMessenger.AsyncAction<T> action)
    {
        base.Register(recipient, action);
        RequestCleanup();
    }

    /// <inheritdoc/>
    public override void Register<T>(object recipient, IHandler<T> handler)
    {
        base.Register(recipient, handler);
        RequestCleanup();
    }

    /// <inheritdoc/>
    public override void Unregister<T>(object recipient)
    {
        base.Unregister<T>(recipient);
        RequestCleanup();
    }

    private void RequestCleanup()
    {
        if (_isCleanupRequested)
            return;

        _isCleanupRequested = true;

        Dispatcher.CurrentDispatcher.BeginInvoke(() =>
        {
            _isCleanupRequested = false;
            Cleanup();
        }, DispatcherPriority.ApplicationIdle);
    }
}