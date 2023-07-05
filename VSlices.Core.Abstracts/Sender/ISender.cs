using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.Sender;

public interface ISender
{
    ValueTask<Response<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

}
