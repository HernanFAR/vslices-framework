using FluentValidation;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.FluentValidation;

/// <summary>
/// A validation behavior that uses FluentValidation
/// </summary>
/// <typeparam name="TRequest">The intercepted request to validate</typeparam>
/// <typeparam name="TResponse">The expected successful response</typeparam>
public class FluentValidationBehavior<TRequest, TResponse> : AbstractValidationBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _requestValidator;
    
    /// <summary>
    /// Creates a new instance using the validators registered in the container
    /// </summary>
    /// <remarks>Even if a <see cref="IEnumerable{T}"/> is used here, only the first validator will be used</remarks>
    /// <param name="requestValidators">Validators registered</param>
    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> requestValidators)
    {
        _requestValidator = requestValidators.FirstOrDefault();
    }

    /// <inheritdoc/>
    protected override async ValueTask<Response<Success>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (_requestValidator is null)
        {
            return Success.Value;
        }

        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return Success.Value;

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.ContractValidation(errors);
    }
}
