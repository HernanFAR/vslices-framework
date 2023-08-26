using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that removes entities.
/// </summary>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to remove</typeparam>
public abstract class RemoveHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IRemoveRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IRemoveRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with remove function</param>
    protected RemoveHandler(IRemoveRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsFailure)
        {
            return useCaseValidationResult.BusinessFailure;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.RemoveAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

        return GetResponse(entity, request);
    }

    /// <summary>
    /// Validates the use case rules
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets and process the entity to be removed
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> with a <typeparamref name="TEntity"/> in</returns>
    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The removed entity</param>
    /// <param name="request">The handled request</param>
    /// <returns>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that validate entities, before remove them
/// </summary>
/// <remarks>Useful to implement always valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class EntityValidatedRemoveHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IRemoveRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IRemoveRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with remove function</param>
    protected EntityValidatedRemoveHandler(IRemoveRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsFailure)
        {
            return useCaseValidationResult.BusinessFailure;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var entityValidationResult = await ValidateEntityAsync(entity, cancellationToken);

        if (entityValidationResult.IsFailure)
        {
            return entityValidationResult.BusinessFailure;
        }

        var dataAccessResult = await _repository.RemoveAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

        return GetResponse(entity, request);
    }

    /// <summary>
    /// Validates the use case rules
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets and process the entity to be persisted
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> with a <typeparamref name="TEntity"/> in</returns>
    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Validates the before processed entity
    /// </summary>
    /// <param name="entity">The created entity</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateEntityAsync(TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The removed entity</param>
    /// <param name="request">The handled request</param>
    /// <returns>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that removes entities.
/// </summary>
/// <remarks>Useful in commands without response</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to remove</typeparam>
public abstract class RemoveHandler<TRequest, TEntity> : RemoveHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected RemoveHandler(IRemoveRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override Success GetResponse(TEntity _, TRequest r) => Success.Value;
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that validates entities, before remove them
/// </summary>
/// <remarks>Useful in commands without response and to implement always-valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to remove</typeparam>
public abstract class EntityValidatedRemoveHandler<TRequest, TEntity> : EntityValidatedRemoveHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected EntityValidatedRemoveHandler(IRemoveRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override Success GetResponse(TEntity _, TRequest r) => Success.Value;

}
