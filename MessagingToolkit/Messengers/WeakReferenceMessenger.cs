using MessagingToolkit.Handlers;
using MessagingToolkit.Messengers.ReferenceHolder;

namespace MessagingToolkit.Messengers;

/// <summary>
/// Implementation of <see cref="IMessenger"/> using weak references.
/// <br />
/// <b>Cleanup is not automatic!</b> Make sure to call <see cref="Cleanup"/> at appropriate times.
/// </summary>
/// <param name="isThreadSafe">if true, the handlers are locked during registration, deregistration and cleanup.</param>
public class WeakReferenceMessenger(bool isThreadSafe) : IMessenger
{
    private readonly WeakReferenceHolder _handlers = new(isThreadSafe);

    /// <inheritdoc />
    public virtual void Publish<T>(T message)
    {
        foreach (var handler in _handlers.Get<T>())
        {
            handler.Execute(message);
        }
    }

    /// <inheritdoc />
    public virtual async Task PublishAsync<T>(T message)
    {
        foreach (var handler in _handlers.Get<T>())
        {
            await handler.ExecuteAsync(message);
        }
    }

    /// <inheritdoc />
    public virtual void Register<T>(object recipient, Action<T> action)
    {
        _handlers.Register(recipient, new Handler<T>(action));
    }

    /// <inheritdoc />
    public virtual void Register<T>(object recipient, IMessenger.AsyncAction<T> action)
    {
        _handlers.Register(recipient, new AsyncHandler<T>(action));
    }

    /// <inheritdoc />
    public virtual void Register<T>(object recipient, IHandler<T> handler)
    {
        _handlers.Register(recipient, handler);
    }

    /// <inheritdoc />
    public virtual void Unregister<T>(object recipient)
    {
        _handlers.Unregister<T>(recipient);
    }

    /// <summary>
    /// Removes all dead weak references.
    /// </summary>
    public void Cleanup()
    {
        _handlers.Cleanup();
    }
}