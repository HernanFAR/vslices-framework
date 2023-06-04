using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedReadHandler<TRequest, TSearchOptions, TResponse> : RequestValidatedReadHandler<TRequest, TSearchOptions, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedReadHandler(IValidator<TRequest> requestValidator, IReadRepository<TResponse, TSearchOptions> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class RequestFluentValidatedReadHandler<TRequest, TResponse> : RequestValidatedReadHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedReadHandler(IValidator<TRequest> requestValidator, IReadRepository<TResponse, TRequest> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class RequestFluentValidatedBasicReadHandler<TRequest, TResponse> : RequestValidatedBasicReadHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedBasicReadHandler(IValidator<TRequest> requestValidator, IReadRepository<TResponse> repository) : base(repository)
    {
        _requestValidator = requestValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
