namespace MessagingToolkit.Handlers;

public interface IHandler<in T>
{
    void Execute(T message);

    Task ExecuteAsync(T message);
}