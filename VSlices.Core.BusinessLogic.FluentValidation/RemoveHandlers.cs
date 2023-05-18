using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedRemoveHandler<TRequest, TResponse, TDomain> : RequestValidatedRemoveHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator, IRemovableRepository<TDomain> repository) : base(repository)
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

public abstract class DomainFluentValidatedRemoveHandler<TRequest, TResponse, TDomain> : DomainValidatedRemoveHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected DomainFluentValidatedRemoveHandler(IValidator<TDomain> domainValidator, IRemovableRepository<TDomain> repository) : base(repository)
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

public abstract class FullyFluentValidatedRemoveHandler<TRequest, TResponse, TDomain> : FullyValidatedRemoveHandler<TRequest, TResponse, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected FullyFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, IRemovableRepository<TDomain> repository) : base(repository)
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

public abstract class RequestFluentValidatedRemoveHandler<TRequest, TDomain> : RequestValidatedRemoveHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator, IRemovableRepository<TDomain> repository) : base(repository)
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

public abstract class DomainFluentValidatedRemoveHandler<TRequest, TDomain> : DomainValidatedRemoveHandler<TRequest, TDomain>
{
    private readonly IValidator<TDomain> _domainValidator;

    protected DomainFluentValidatedRemoveHandler(IValidator<TDomain> domainValidator, IRemovableRepository<TDomain> repository) : base(repository)
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

public abstract class FullyFluentValidatedRemoveHandler<TRequest, TDomain> : FullyValidatedRemoveHandler<TRequest, TDomain>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TDomain> _domainValidator;

    protected FullyFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator,
        IValidator<TDomain> domainValidator, IRemovableRepository<TDomain> repository) : base(repository)
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
