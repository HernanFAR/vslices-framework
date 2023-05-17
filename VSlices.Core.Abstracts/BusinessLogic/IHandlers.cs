using OneOf;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IHandler<in TRequest, TSuccessResponse>
{
    ValueTask<OneOf<TSuccessResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
