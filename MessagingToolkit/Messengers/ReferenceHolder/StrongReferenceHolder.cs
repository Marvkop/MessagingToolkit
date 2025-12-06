using System.Collections.Concurrent;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementation of <see cref="IReferenceHolder"/> using strong references.
/// </summary>
internal class StrongReferenceHolder : IReferenceHolder
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
        try
        {
            Monitor.Enter(_handlers);

            _handlers
                .GetOrAdd(type, _ => new())
                .AddOrUpdate(recipient, _ => [handler], (_, list) =>
                {
                    list.Add(handler);
                    return list;
                });
        }
        finally
        {
            Monitor.Exit(_handlers);
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
        try
        {
            Monitor.Enter(_handlers);

            if (_handlers.TryGetValue(type, out var handlers))
            {
                handlers.Remove(recipient, out _);
            }
        }
        finally
        {
            Monitor.Exit(_handlers);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler<T>> Get<T>()
    {
        // explicit implementation to not enumerate twice
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            try
            {
                Monitor.Enter(_handlers);

                return handlers
                    .SelectMany(kvp => kvp.Value)
                    .OfType<IHandler<T>>()
                    .ToArray();
            }
            finally
            {
                Monitor.Exit(_handlers);
            }
        }

        return [];
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler> Get(Type type)
    {
        if (_handlers.TryGetValue(type, out var handlers))
        {
            try
            {
                Monitor.Enter(_handlers);

                return handlers
                    .SelectMany(kvp => kvp.Value)
                    .ToArray();
            }
            finally
            {
                Monitor.Exit(_handlers);
            }
        }

        return [];
    }
}