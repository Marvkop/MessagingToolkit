using MessagingToolkit.Handlers;

namespace MessagingToolkit.Messengers.ReferenceHolder;

/// <summary>
/// Implementations provide handle registration/unregistration of handlers and supply them to messengers.  
/// </summary>
internal interface IReferenceHolder
{
    /// <summary>
    /// Registers a handler for the given message type and recipient.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="T">The message type.</typeparam>
    void Register<T>(object recipient, IHandler<T> handler);

    /// <summary>
    /// Registers a handler for the given message type and recipient.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="type">The message type.</param>
    void Register(object recipient, IHandler handler, Type type);

    /// <summary>
    /// Unregisters all handlers for the given message type and recipient.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <typeparam name="T">The message type.</typeparam>
    void Unregister<T>(object recipient);

    /// <summary>
    /// Unregisters all handlers for the given message type and recipient.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="type">The message type.</param>
    void Unregister(object recipient, Type type);

    /// <summary>
    /// Gets all handlers of the given message type.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <returns>Handlers for the message type.</returns>
    IReadOnlyList<IHandler<T>> Get<T>();

    /// <summary>
    /// Gets all handlers of the given message type.
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <returns>Handlers for the message type.</returns>
    IReadOnlyList<IHandler> Get(Type type);
}