using System.Collections.Concurrent;
using MessagingToolkit.Extensions;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers;

public class StrongReferenceMessenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, List<(object Recipient, object Handler)>> _handlers = new();

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

    public async Task PublishAsync<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
            return;

        await Task.WhenAll(handlers
            .Select(handler => handler.Handler)
            .OfType<IHandler<T>>()
            .Select(handler => handler.ExecuteAsync(message)));
    }

    public void Register<T>(object recipient, Action<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((new WeakReference(recipient), new Handler<T>(action)));
    }

    public void Register<T>(object recipient, IMessenger.AsyncAction<T> action)
    {
        _handlers
            .GetOrAdd(typeof(T), () => [])
            .Add((new WeakReference(recipient), new AsyncHandler<T>(action)));
    }

    public void Unregister<T>(object recipient)
    {
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            handlers.RemoveAll(tuple => ReferenceEquals(tuple.Recipient, recipient));
        }
    }
}