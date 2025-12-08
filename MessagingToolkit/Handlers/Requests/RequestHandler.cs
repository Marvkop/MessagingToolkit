using MessagingToolkit.Messengers.Interfaces;

namespace MessagingToolkit.Handlers.Requests;

public class RequestHandler<TRequest, TResponse>(IRequestMessenger.RequestHandler<TRequest, TResponse> handler) : IRequestHandler<TRequest, TResponse>
{
    private readonly IRequestMessenger.RequestHandler<TRequest, TResponse> _handler = handler ?? throw new ArgumentNullException(nameof(handler));

    public IEnumerable<TResponse> Respond(TRequest request)
    {
        return _handler(request);
    }

    public IAsyncEnumerable<TResponse> RespondAsync(TRequest request)
    {
        return _handler(request).ToAsyncEnumerable();
    }
}