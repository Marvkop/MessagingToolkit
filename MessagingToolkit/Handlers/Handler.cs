namespace MessagingToolkit.Handlers;

public class Handler<T>(Action<T> action) : IHandler<T>
{
    private readonly Action<T> _action = action ?? throw new ArgumentNullException(nameof(action));

    public void Execute(T message)
    {
        _action(message);
    }

    public Task ExecuteAsync(T message)
    {
        _action(message);

        return Task.CompletedTask;
    }
}