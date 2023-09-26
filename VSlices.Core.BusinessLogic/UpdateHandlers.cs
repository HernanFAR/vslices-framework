using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that update entities.
/// </summary>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to update</typeparam>
public abstract class UpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUpdateRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IUpdateRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with update function</param>
    protected UpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsFailure)
        {
            return useCaseValidationResult.BusinessFailure;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

        await AfterUpdateAsync(entity, request, cancellationToken);

        return await GetResponseAsync(entity, request, cancellationToken);
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
    /// Gets and process the entity to be updated
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> with a <typeparamref name="TEntity"/> in</returns>
    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal virtual ValueTask AfterUpdateAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The updated entity</param>
    /// <param name="request">The handled request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract ValueTask<TResponse> GetResponseAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that validates entities, before update them
/// </summary>
/// <remarks>Useful to implement always valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to update</typeparam>
public abstract class EntityValidatedUpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUpdateRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IUpdateRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with update function</param>
    protected EntityValidatedUpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
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

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

        await AfterUpdateAsync(entity, request, cancellationToken);

        return await GetResponseAsync(entity, request, cancellationToken);
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

    protected internal virtual ValueTask AfterUpdateAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken) => ValueTask.CompletedTask;

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The updated entity</param>
    /// <param name="request">The handled request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract ValueTask<TResponse> GetResponseAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that updates entities.
/// </summary>
/// <remarks>Useful in commands without response</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to update</typeparam>
public abstract class UpdateHandler<TRequest, TEntity> : UpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected UpdateHandler(IUpdateRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override ValueTask<Success> GetResponseAsync(TEntity _, TRequest r, CancellationToken c) 
        => Success.TaskValue;
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that validates entities, before update them
/// </summary>
/// <remarks>Useful in commands without response and to implement always-valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to update</typeparam>
public abstract class EntityValidatedUpdateHandler<TRequest, TEntity> : EntityValidatedUpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected EntityValidatedUpdateHandler(IUpdateRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override ValueTask<Success> GetResponseAsync(TEntity _, TRequest r, CancellationToken c) 
        => Success.TaskValue;
}
