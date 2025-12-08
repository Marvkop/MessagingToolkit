using System.Collections.Concurrent;
using MessagingToolkit.Async;
using MessagingToolkit.Handlers.Messaging;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementation of <see cref="IReferenceHolder"/> using strong references.
/// </summary>
/// <param name="isThreadSafe">if true, the handlers are locked during registration and deregistration.</param>
public class StrongReferenceHolder(bool isThreadSafe) : IReferenceHolder
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, IList<IHandler>>> _handlers = new();

    /// <inheritdoc />
    public void Register<T>(object recipient, IHandler<T> handler)
    {
        Register(recipient, handler, typeof(T));
    }

    /// <inheritdoc />
    public void Register(object recipient, IHandler handler, Type type)
    {
        using (DisposableMonitorLock.CreateDisposable(isThreadSafe, _handlers))
        {
            _handlers
                .GetOrAdd(type, _ => new())
                .AddOrUpdate(recipient, _ => [handler], (_, list) =>
                {
                    list.Add(handler);
                    return list;
                });
        }
    }

    /// <inheritdoc />
    public void Unregister<T>(object recipient)
    {
        Unregister(recipient, typeof(T));
    }

    /// <inheritdoc />
    public void Unregister(object recipient, Type type)
    {
        using (DisposableMonitorLock.CreateDisposable(isThreadSafe, _handlers))
        {
            if (_handlers.TryGetValue(type, out var handlers))
            {
                handlers.Remove(recipient, out _);
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler<T>> Get<T>()
    {
        // explicit implementation to not enumerate twice
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            using (DisposableMonitorLock.CreateDisposable(isThreadSafe, _handlers))
            {
                return handlers
                    .SelectMany(kvp => kvp.Value)
                    .OfType<IHandler<T>>()
                    .ToArray();
            }
        }

        return [];
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler> Get(Type type)
    {
        if (_handlers.TryGetValue(type, out var handlers))
        {
            using (DisposableMonitorLock.CreateDisposable(isThreadSafe, _handlers))
            {
                return handlers
                    .SelectMany(kvp => kvp.Value)
                    .ToArray();
            }
        }

        return [];
    }
}