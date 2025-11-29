using System.Collections.Concurrent;
using MessagingToolkit.Extensions;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers;

public class StrongReferenceMessenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, List<(object Recipient, object Handler)>> _handlers = new();

    /// <inheritdoc />
    public void Publish<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
            return;

        foreach (var (_, handler) in handlers)
        {
            var handlerInstance = (IHandler<T>)handler;
            handlerInstance.Execute(message);
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
            return;


        foreach (var handler in handlers
                     .Select(handler => handler.Handler)
                     .OfType<IHandler<T>>())
        {
            await handler.ExecuteAsync(message);
        }
    }

    /// <inheritdoc />
    public void Register<T>(object recipient, Action<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((recipient, new Handler<T>(action)));
    }

    /// <inheritdoc />
    public void Register<T>(object recipient, IMessenger.AsyncAction<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((recipient, new AsyncHandler<T>(action)));
    }

    /// <inheritdoc />
    public void Unregister<T>(object recipient)
    {
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            handlers.RemoveAll(tuple => ReferenceEquals(tuple.Recipient, recipient));
        }
    }
}