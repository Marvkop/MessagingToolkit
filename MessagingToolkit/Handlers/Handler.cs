namespace MessagingToolkit.Handlers;

/// <summary>
/// Base-Handler providing execution
/// </summary>
/// <param name="action"></param>
/// <typeparam name="T"></typeparam>
public class Handler<T>(Action<T> action) : IHandler<T>
{
    private readonly Action<T> _action = action ?? throw new ArgumentNullException(nameof(action));

    /// <inheritdoc/>
    public void Execute(T message)
    {
        _action(message);
    }

    /// <inheritdoc/>
    public Task ExecuteAsync(T message)
    {
        _action(message);

        return Task.CompletedTask;
    }
}