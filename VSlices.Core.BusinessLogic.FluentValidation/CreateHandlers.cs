using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedCreateHandler<TRequest, TResponse, TDomain> : RequestValidatedCreateHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedCreateHandler(IValidator<TRequest> requestValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class DomainFluentValidatedCreateHandler<TRequest, TResponse, TDomain> : DomainValidatedCreateHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected DomainFluentValidatedCreateHandler(IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class FullyFluentValidatedCreateHandler<TRequest, TResponse, TDomain> : FullyValidatedCreateHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected FullyFluentValidatedCreateHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class RequestFluentValidatedCreateHandler<TRequest, TDomain> : RequestValidatedCreateHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedCreateHandler(IValidator<TRequest> requestValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class DomainFluentValidatedCreateHandler<TRequest, TDomain> : DomainValidatedCreateHandler<TRequest, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected DomainFluentValidatedCreateHandler(IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class FullyFluentValidatedCreateHandler<TRequest, TDomain> : FullyValidatedCreateHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected FullyFluentValidatedCreateHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
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
