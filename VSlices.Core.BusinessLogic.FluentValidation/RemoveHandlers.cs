using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class RequestFluentValidatedRemoveHandler<TRequest, TResponse, TEntity> : RequestValidatedRemoveHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator, IRemoveRepository<TEntity> repository) : base(repository)
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

public abstract class EntityFluentValidatedRemoveHandler<TRequest, TResponse, TEntity> : EntityValidatedRemoveHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedRemoveHandler(IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
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

public abstract class FullyFluentValidatedRemoveHandler<TRequest, TResponse, TEntity> : FullyValidatedRemoveHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _entityValidator;

    protected FullyFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
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

public abstract class RequestFluentValidatedRemoveHandler<TRequest, TEntity> : RequestValidatedRemoveHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;

    protected RequestFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator, IRemoveRepository<TEntity> repository) : base(repository)
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

public abstract class EntityFluentValidatedRemoveHandler<TRequest, TEntity> : EntityValidatedRemoveHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedRemoveHandler(IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
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

public abstract class FullyFluentValidatedRemoveHandler<TRequest, TEntity> : FullyValidatedRemoveHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TRequest> _requestValidator;
    private readonly IValidator<TEntity> _entityValidator;

    protected FullyFluentValidatedRemoveHandler(IValidator<TRequest> requestValidator,
        IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
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
