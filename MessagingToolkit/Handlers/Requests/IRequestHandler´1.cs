namespace MessagingToolkit.Handlers.Requests;

public interface IRequestHandler<in TRequest, out TResponse> : IRequestHandler
{
    IEnumerable<TResponse> Respond(TRequest request);

    IAsyncEnumerable<TResponse> RespondAsync(TRequest request);
}