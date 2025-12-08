using MessagingToolkit.Handlers.Messaging;

namespace MessagingToolkit.Messengers.Interfaces;

/// <summary>
/// Implementations handle messaging.
/// </summary>
public interface IMessenger
{
    /// <summary>
    /// Representing an async operation without a return value.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    public delegate Task AsyncAction<in T>(T message);

    /// <summary>
    /// Publishes a message to all registered recipients.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    void Publish<T>(T message);

    /// <summary>
    /// Publishes a message to all registered recipients and executes them asynchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <returns>The task representing the operation.</returns>
    Task PublishAsync<T>(T message);

    /// <summary>
    /// Registers the action for its recipient and the given type.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="action">The action.</param>
    /// <typeparam name="T">The type of the messages to receive.</typeparam>
    void Register<T>(object recipient, Action<T> action);

    /// <summary>
    /// Registers the async-action for its recipient and the given type.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="action">The async-action.</param>
    /// <typeparam name="T">The type of the messages to receive.</typeparam>
    void Register<T>(object recipient, AsyncAction<T> action);

    /// <summary>
    /// Registers the handler for its recipient and the given type.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="T">The type of the messages to receive.</typeparam>
    void Register<T>(object recipient, IHandler<T> handler);

    /// <summary>
    /// Removes the handlers for a given recipient and type.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    public void Unregister<T>(object recipient);
}