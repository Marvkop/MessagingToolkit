using MessagingToolkit.Messengers.Interfaces;

namespace MessagingToolkit.Handlers.Requests;

public class AsyncRequestHandler<TRequest, TResponse>(IRequestMessenger.AsyncRequestHandler<TRequest, TResponse> handler) : IRequestHandler<TRequest, TResponse>
{
    public IEnumerable<TResponse> Respond(TRequest request)
    {
        return handler(request).ToBlockingEnumerable();
    }

    public IAsyncEnumerable<TResponse> RespondAsync(TRequest request)
    {
        return handler(request);
    }
}