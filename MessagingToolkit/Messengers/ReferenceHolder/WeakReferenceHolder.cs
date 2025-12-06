using System.Collections.Concurrent;
using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementation of <see cref="IReferenceHolder"/> using weak references.
/// <br />
/// <b>Cleanup is not automatic!</b> Make sure to call <see cref="Cleanup"/> at appropriate times.
/// </summary>
internal class WeakReferenceHolder : IReferenceHolder
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<WeakReference, IHandler>> _handlers = new();

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
            .AddOrUpdate(new WeakReference(recipient), handler, (_, _) => handler);
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
            var key = handlers.Keys.FirstOrDefault(reference => reference.Target == recipient);

            if (key is not null)
            {
                handlers.TryRemove(key, out _);
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IHandler<T>> Get<T>()
    {
        // explicit implementation to not enumerate twice
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            return handlers
                .Where(kvp => kvp.Key.IsAlive)
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
                .Where(kvp => kvp.Key.IsAlive)
                .Select(kvp => kvp.Value)
                .ToArray();
        }

        return [];
    }

    /// <summary>
    /// Removes all dead weak references.
    /// </summary>
    public void Cleanup()
    {
        foreach (var handlers in _handlers.Values)
        {
            var deadReferences = handlers.Keys.Where(x => !x.IsAlive).ToList();

            deadReferences.ForEach(x => handlers.TryRemove(x, out _));
        }
    }
}