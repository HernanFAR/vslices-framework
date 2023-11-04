using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation;

/// <summary>
/// Base class for validation behaviors
/// </summary>
/// <typeparam name="TRequest">The intercepted request to validate</typeparam>
/// <typeparam name="TResponse">The expected successful response</typeparam>
public abstract class AbstractValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    /// <inheritdoc/>
    public async ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult.BusinessFailure;
        }

        return await next();
    }
    /// <summary>
    /// Asynchronously validates the request
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
