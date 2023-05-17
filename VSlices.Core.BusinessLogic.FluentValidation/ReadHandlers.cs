using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class AbstractReadRequestFluentValidatedHandler<TRequest, TSearchOptions, TResponse> : AbstractReadRequestValidatedHandler<TRequest, TSearchOptions, TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractReadRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, IReadableRepository<TResponse, TSearchOptions> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractReadRequestFluentValidatedHandler<TRequest, TResponse> : AbstractReadRequestValidatedHandler<TRequest, TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractReadRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, IReadableRepository<TResponse, TRequest> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractBasicReadRequestFluentValidatedHandler<TRequest, TResponse> : AbstractBasicReadRequestValidatedHandler<TRequest, TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractBasicReadRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, IReadableRepository<TResponse> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
