using MessagingToolkit.Messengers;

namespace MessagingToolkit.Handlers;

public class AsyncHandler<T>(IMessenger.AsyncAction<T> action) : IHandler<T>
{
    private readonly IMessenger.AsyncAction<T> _action = action ?? throw new ArgumentNullException(nameof(action));

    public void Execute(T message)
    {
        _action(message).Wait();
    }

    public async Task ExecuteAsync(T message)
    {
        await _action(message);
    }
}