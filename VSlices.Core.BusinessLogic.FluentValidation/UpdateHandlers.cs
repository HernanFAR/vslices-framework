using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedUpdateHandler<TRequest, TResponse, TEntity> : RequestValidatedUpdateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedUpdateHandler(IValidator<TRequest> requestValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

public abstract class EntityFluentValidatedUpdateHandler<TRequest, TResponse, TEntity> : EntityValidatedUpdateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedUpdateHandler(IValidator<TEntity> entityValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

public abstract class FullyFluentValidatedUpdateHandler<TRequest, TResponse, TEntity> : FullyValidatedUpdateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _domainValidator;

    protected FullyFluentValidatedUpdateHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> domainValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class RequestFluentValidatedUpdateHandler<TRequest, TEntity> : RequestValidatedUpdateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedUpdateHandler(IValidator<TRequest> requestValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

public abstract class DomainFluentValidatedUpdateHandler<TRequest, TEntity> : DomainValidatedUpdateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    protected DomainFluentValidatedUpdateHandler(IValidator<TEntity> entityValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

public abstract class FullyFluentValidatedUpdateHandler<TRequest, TEntity> : FullyValidatedUpdateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _entityValidator;

    protected FullyFluentValidatedUpdateHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> entityValidator, IUpdateRepository<TEntity> repository) : base(repository)
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
