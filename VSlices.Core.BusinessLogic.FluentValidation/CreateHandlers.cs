using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class AbstractCreateRequestFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractCreateRequestValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractCreateRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class AbstractCreateDomainFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractCreateDomainValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractCreateDomainFluentValidatedHandler(IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
    {
        _domainValidator = domainValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractCreateFullyFluentValidatedHandler<TRequest, TResponse, TDomain> : AbstractCreateFullyValidatedHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractCreateFullyFluentValidatedHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _domainValidator = domainValidator;
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

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractCreateRequestFluentValidatedHandler<TRequest, TDomain> : AbstractCreateRequestValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected AbstractCreateRequestFluentValidatedHandler(IValidator<TRequest> requestValidator, ICreatableRepository<TDomain> repository) : base(repository)
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

public abstract class AbstractCreateDomainFluentValidatedHandler<TRequest, TDomain> : AbstractCreateDomainValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractCreateDomainFluentValidatedHandler(IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
    {
        _domainValidator = domainValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class AbstractCreateFullyFluentValidatedHandler<TRequest, TDomain> : AbstractCreateFullyValidatedHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected AbstractCreateFullyFluentValidatedHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, ICreatableRepository<TDomain> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _domainValidator = domainValidator;
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

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
