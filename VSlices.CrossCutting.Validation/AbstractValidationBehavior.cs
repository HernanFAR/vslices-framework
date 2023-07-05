using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation;

public abstract class AbstractValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult.BusinessFailure;
        }

        return await next();
    }

    protected internal abstract ValueTask<Response<Success>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
