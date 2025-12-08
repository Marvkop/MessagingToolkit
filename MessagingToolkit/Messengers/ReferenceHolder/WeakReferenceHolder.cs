using System.Collections.Concurrent;
using MessagingToolkit.Async;
using MessagingToolkit.Handlers.Messaging;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementation of <see cref="IReferenceHolder"/> using weak references.
/// <br />
/// <b>Cleanup is not automatic!</b> Make sure to call <see cref="Cleanup"/> at appropriate times.
/// </summary>
/// <param name="isThreadSafe">if true, the handlers are locked during registration, deregistration and cleanup.</param>
internal class WeakReferenceHolder(bool isThreadSafe) : IReferenceHolder
{
    private readonly ConcurrentDictionary<int, (WeakReference Reference, ISet<Type> Types)> _references = new();
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, IList<IHandler>>> _handlers = new();

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
            var hashCode = recipient.GetHashCode();
            var value = new WeakReference(recipient);

            var (_, types) = _references.GetOrAdd(hashCode, _ => new(value, new HashSet<Type>()));

            types.Add(type);

            _handlers
                .GetOrAdd(type, _ => new())
                .AddOrUpdate(hashCode, _ => [handler], (_, list) =>
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
            var hashCode = recipient.GetHashCode();

            if (_references.TryGetValue(hashCode, out var value))
            {
                value.Types.Remove(type);
            }

            if (_handlers.TryGetValue(type, out var handlers))
            {
                handlers.Remove(hashCode, out _);

                if (handlers.IsEmpty)
                {
                    _handlers.Remove(type, out _);
                }
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
                    .Where(kvp => _references[kvp.Key].Reference.IsAlive)
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
                    .Where(kvp => _references[kvp.Key].Reference.IsAlive)
                    .SelectMany(kvp => kvp.Value)
                    .ToArray();
            }
        }

        return [];
    }

    /// <summary>
    /// Removes all dead weak references.
    /// </summary>
    public void Cleanup()
    {
        using (DisposableMonitorLock.CreateDisposable(isThreadSafe, _handlers))
        {
            var deadReferences = _references.Where(kvp => !kvp.Value.Reference.IsAlive).Select(kvp => (kvp.Key, kvp.Value.Types)).ToList();

            foreach (var (key, types) in deadReferences)
            {
                foreach (var type in types)
                {
                    var handlers = _handlers[type];

                    handlers.Remove(key, out _);

                    if (handlers.IsEmpty)
                    {
                        _handlers.Remove(type, out _);
                    }
                }

                _references.Remove(key, out _);
            }
        }
    }
}