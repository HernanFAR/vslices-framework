using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IHandler<in TRequest, TSuccessResponse>
{
    Task<OneOf<TSuccessResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
