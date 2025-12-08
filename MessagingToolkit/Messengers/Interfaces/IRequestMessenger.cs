using MessagingToolkit.Handlers.Requests;

namespace MessagingToolkit.Messengers.Interfaces;

/// <summary>
/// Implementations handle requests.
/// </summary>
public interface IRequestMessenger
{
    delegate IEnumerable<TResponse> RequestHandler<in TRequest, out TResponse>(TRequest request);

    delegate IAsyncEnumerable<TResponse> AsyncRequestHandler<in TRequest, out TResponse>(TRequest request);

    IEnumerable<TResult> Request<TRequest, TResult>(TRequest request);

    IAsyncEnumerable<TResult> RequestAsync<TRequest, TResult>(TRequest request);

    void RegisterRequest<TRequest, TResponse>(object recipient, RequestHandler<TRequest, TResponse> handler);

    void RegisterRequest<TRequest, TResponse>(object recipient, AsyncRequestHandler<TRequest, TResponse> handler);

    void RegisterRequest<TRequest, TResponse>(object recipient, IRequestHandler<TRequest, TResponse> handler);

    public void UnregisterRequest<TRequest, TResponse>(object recipient);
}