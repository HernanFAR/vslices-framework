using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.Sender;

/// <summary>
/// Sends a request through the VSlices pipeline to be handled by a single handler
/// </summary>
public interface ISender
{
    /// <summary>
    /// Asynchronously sends a request to a handler
    /// </summary>
    /// <typeparam name="TResponse">Expected response type</typeparam>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

}
