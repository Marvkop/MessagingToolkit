using MessagingToolkit.Messengers.Interfaces;

namespace MessagingToolkit.Handlers.Messaging;

public class AsyncHandler<T>(IMessenger.AsyncAction<T> action) : IHandler<T>
{
    public void Execute(T message)
    {
        Task.Run(async () => await action(message)).GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(T message)
    {
        await action(message);
    }
}