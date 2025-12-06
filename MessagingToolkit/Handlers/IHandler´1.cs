namespace MessagingToolkit.Handlers;

/// <summary>
/// Implementations provide the actions to execute when a message is published.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandler<in T> : IHandler
{
    /// <summary>
    /// Executes when a message of type <see cref="T"/> is published synchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    void Execute(T message);

    /// <summary>
    /// Executes when a message of type <see cref="T"/> is published asynchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The task executing the action.</returns>
    Task ExecuteAsync(T message);
}