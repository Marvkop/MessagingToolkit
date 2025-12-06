using System.Collections.Concurrent;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementation of <see cref="IReferenceHolder"/> using strong references.
/// </summary>
internal class StrongReferenceHolder : IReferenceHolder
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, IHandler>> _handlers = new();

    /// <inheritdoc />
    public void Register<T>(object recipient, IHandler<T> handler)
    {
        Register(recipient, handler, typeof(T));
    }

    /// <inheritdoc />
    public void Register(object recipient, IHandler handler, Type type)
    {
        _handlers
            .GetOrAdd(type, _ => new())
            .AddOrUpdate(recipient, handler, (_, _) => handler);
    }

    /// <inheritdoc />
    public void Unregister<T>(object recipient)
    {
        Unregister(recipient, typeof(T));
    }

    /// <inheritdoc />
    public void Unregister(object recipient, Type type)
    {
        if (_handlers.TryGetValue(type, out var handlers))
        {
            handlers.TryRemove(recipient, out _);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler<T>> Get<T>()
    {
        // explicit implementation to not enumerate twice
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            return handlers
                .Select(kvp => kvp.Value)
                .OfType<IHandler<T>>()
                .ToArray();
        }

        return [];
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler> Get(Type type)
    {
        if (_handlers.TryGetValue(type, out var handlers))
        {
            return handlers
                .Select(kvp => kvp.Value)
                .ToArray();
        }

        return [];
    }
}