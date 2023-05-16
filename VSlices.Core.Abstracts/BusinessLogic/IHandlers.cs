using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IHandler<in TRequest, TSuccessResponse>
{
    Task<OneOf<TSuccessResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}


public interface IRequestValidatedHandler<in TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

}


public interface IDomainValidatedHandler<in TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default);

}

public interface IFullyValidatedHandler<in TRequest, TResponse, TDomain> :
    IRequestValidatedHandler<TRequest, TResponse>, IDomainValidatedHandler<TRequest, TResponse, TDomain>
{

}


