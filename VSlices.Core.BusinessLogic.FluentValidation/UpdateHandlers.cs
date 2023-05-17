using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class AbstractUpdateRequestFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractUpdateRequestValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractUpdateRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, IUpdateableRepository<TDomain> repository) : base(repository)
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

public abstract class AbstractUpdateDomainFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractUpdateDomainValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractUpdateDomainFluentValidatedHandler(IValidator<TDomain> domainValidator, IUpdateableRepository<TDomain> repository) : base(repository)
    {
        _domainValidator = domainValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractUpdateFullyFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractUpdateFullyValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractUpdateFullyFluentValidatedHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, IUpdateableRepository<TDomain> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _domainValidator = domainValidator;
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

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractUpdateRequestFluentValidatedHandler<TRequest, TDomain> : AbstractUpdateRequestValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractUpdateRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, IUpdateableRepository<TDomain> repository) : base(repository)
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

public abstract class AbstractUpdateDomainFluentValidatedHandler<TRequest, TDomain> : AbstractUpdateDomainValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractUpdateDomainFluentValidatedHandler(IValidator<TDomain> domainValidator, IUpdateableRepository<TDomain> repository) : base(repository)
    {
        _domainValidator = domainValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractUpdateFullyFluentValidatedHandler<TRequest, TDomain> : AbstractUpdateFullyValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractUpdateFullyFluentValidatedHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, IUpdateableRepository<TDomain> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _domainValidator = domainValidator;
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

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
