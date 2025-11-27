using System.Collections.Concurrent;
using MessagingToolkit.Extensions;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers;

public class WeakReferenceMessenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, List<(WeakReference Reference, object Handler)>> _handlers = new();

    /// <inheritdoc />
    public void Publish<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
            return;

        foreach (var handler in handlers
                     .Where(handler => handler.Reference.IsAlive)
                     .Select(handler => handler.Handler)
                     .OfType<IHandler<T>>())
        {
            handler.Execute(message);
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
            return;

        await Task.WhenAll(handlers
                .Where(handler => handler.Reference.IsAlive)
                .Select(handler => handler.Handler)
                .OfType<IHandler<T>>()
                .Select(handler => handler.ExecuteAsync(message)))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Register<T>(object recipient, Action<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((new WeakReference(recipient), new Handler<T>(action)));
    }

    /// <inheritdoc />
    public void Register<T>(object recipient, IMessenger.AsyncAction<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((new WeakReference(recipient), new AsyncHandler<T>(action)));
    }

    /// <inheritdoc />
    public void Unregister<T>(object recipient)
    {
        lock (_handlers)
        {
            if (_handlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers.RemoveAll(tuple => !tuple.Reference.IsAlive || ReferenceEquals(tuple.Reference.Target, recipient));
            }
        }
    }

    /// <summary>
    /// Removes all expired weak references.
    /// </summary>
    public void Cleanup()
    {
        foreach (var kvp in _handlers)
        {
            kvp.Value.RemoveAll(tuple => !tuple.Reference.IsAlive);
        }
    }
}