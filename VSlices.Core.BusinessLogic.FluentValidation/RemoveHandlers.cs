using FluentValidation;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that validate entities with Fluent Validation, before remove them
/// </summary>
/// <remarks>Useful to implement always valid entities</remarks>
/// <typeparam name="TRequest">The request to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to remove</typeparam>
public abstract class EntityFluentValidatedRemoveHandler<TRequest, TResponse, TEntity> : EntityValidatedRemoveHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TEntity> _entityValidator;

    /// <summary>
    /// Creates a new instance using the given <see cref="IRemoveRepository{TEntity}"/>
    /// </summary>
    /// <param name="entityValidator">Validator of the entity</param>
    /// <param name="repository">Repository with remove function</param>
    protected EntityFluentValidatedRemoveHandler(IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    /// <inheritdoc />
    protected override async ValueTask<Response<Success>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return Success.Value;

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors);
    }
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that validates entities with Fluent Validation, before remove them
/// </summary>
/// <remarks>Useful in commands without response and to implement always-valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to remove</typeparam>
public abstract class EntityFluentValidatedRemoveHandler<TRequest, TEntity> : EntityValidatedRemoveHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    /// <summary>
    /// Creates a new instance using the given <see cref="IRemoveRepository{TEntity}"/>
    /// </summary>
    /// <param name="entityValidator">Validator of the entity</param>
    /// <param name="repository">Repository with remove function</param>
    protected EntityFluentValidatedRemoveHandler(IValidator<TEntity> entityValidator, IRemoveRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    /// <inheritdoc />
    protected override async ValueTask<Response<Success>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return Success.Value;

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors);
    }
}
