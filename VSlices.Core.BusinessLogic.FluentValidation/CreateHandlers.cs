using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedCreateHandler<TRequest, TResponse, TEntity> : RequestValidatedCreateHandler<TRequest, TResponse, TEntity>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedCreateHandler(IValidator<TRequest> requestValidator, ICreateRepository<TEntity> repository) : base(repository)
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

public abstract class EntityFluentValidatedCreateHandler<TRequest, TResponse, TEntity> : EntityValidatedCreateHandler<TRequest, TResponse, TEntity>
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class FullyFluentValidatedCreateHandler<TRequest, TResponse, TEntity> : FullyValidatedCreateHandler<TRequest, TResponse, TEntity>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _entityValidator;

    protected FullyFluentValidatedCreateHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _entityValidator = entityValidator;
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

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class RequestFluentValidatedCreateHandler<TRequest, TEntity> : RequestValidatedCreateHandler<TRequest, TEntity>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedCreateHandler(IValidator<TRequest> requestValidator, ICreateRepository<TEntity> repository) : base(repository)
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

public abstract class EntityFluentValidatedCreateHandler<TRequest, TEntity> : EntityValidatedCreateHandler<TRequest, TEntity>
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class FullyFluentValidatedCreateHandler<TRequest, TEntity> : FullyValidatedCreateHandler<TRequest, TEntity>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _entityValidator;

    protected FullyFluentValidatedCreateHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _requestValidator = requestValidator;
        _entityValidator = entityValidator;
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

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var entityValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (entityValidationResult.IsValid) return new Success();

        var errors = entityValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
