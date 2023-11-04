using FluentValidation;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Requests;

namespace VSlices.Core.BusinessLogic.FluentValidation;

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that validate entities with Fluent Validation, before create them
/// </summary>
/// <remarks>Useful to implement always valid entities</remarks>
/// <typeparam name="TRequest">The request to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class EntityFluentValidatedCreateHandler<TRequest, TResponse, TEntity> : EntityValidatedCreateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TEntity> _entityValidator;

    /// <summary>
    /// Creates a new instance using the given <see cref="ICreateRepository{TEntity}"/>
    /// </summary>
    /// <param name="entityValidator">Validator of the entity</param>
    /// <param name="repository">Repository with create function</param>
    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }
    
    /// <inheritdoc />
    protected override async ValueTask<Response<Success>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return Success.Value;

        var errors = domainValidationResult
            .Errors
            .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors: errors);
    }
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that validates entities with Fluent Validation, before create them
/// </summary>
/// <remarks>Useful in commands without response and to implement always-valid entities</remarks>
/// <typeparam name="TRequest">The request to handle</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class EntityFluentValidatedCreateHandler<TRequest, TEntity> : EntityValidatedCreateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    /// <summary>
    /// Creates a new instance using the given <see cref="ICreateRepository{TEntity}"/>
    /// </summary>
    /// <param name="entityValidator">Validator of the entity</param>
    /// <param name="repository">Repository with create function</param>
    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    /// <inheritdoc />
    protected override async ValueTask<Response<Success>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return Success.Value;

        var errors = domainValidationResult
            .Errors
            .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors: errors);
    }
}
