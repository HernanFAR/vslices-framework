using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that creates entities.
/// </summary>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class CreateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly ICreateRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="ICreateRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with create function</param>
    protected CreateHandler(ICreateRepository<TEntity> repository)
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

        var entity = await CreateEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.CreateAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

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
    /// Creates the entity to be persisted
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> with a <typeparamref name="TEntity"/> in</returns>
    protected internal abstract ValueTask<TEntity> CreateEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The created entity</param>
    /// <param name="request">The handled request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract ValueTask<TResponse> GetResponseAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand{TResponse}"/> that validate entities, before create them
/// </summary>
/// <remarks>Useful to implement always valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class EntityValidatedCreateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly ICreateRepository<TEntity> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="ICreateRepository{TEntity}"/>
    /// </summary>
    /// <param name="repository">Repository with create function</param>
    protected EntityValidatedCreateHandler(ICreateRepository<TEntity> repository)
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

        var entity = await CreateEntityAsync(request, cancellationToken);

        var entityValidationResult = await ValidateEntityAsync(entity, cancellationToken);

        if (entityValidationResult.IsFailure)
        {
            return entityValidationResult.BusinessFailure;
        }

        var dataAccessResult = await _repository.CreateAsync(entity, cancellationToken);

        if (dataAccessResult.IsFailure)
        {
            return dataAccessResult.BusinessFailure;
        }

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
    /// Creates the entity to be persisted
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> with a <typeparamref name="TEntity"/> in</returns>
    protected internal abstract ValueTask<TEntity> CreateEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Validates the before created entity
    /// </summary>
    /// <param name="entity">The created entity</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TResponse}" /> of <see cref="Success"/> that represents the result of the process</returns>
    protected internal abstract ValueTask<Response<Success>> ValidateEntityAsync(TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates the response to be returned
    /// </summary>
    /// <param name="entity">The created entity</param>
    /// <param name="request">The handled request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <typeparamref name="TResponse"/>A <typeparamref name="TResponse"/> </returns>
    protected internal abstract ValueTask<TResponse> GetResponseAsync(TEntity entity, TRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that creates entities.
/// </summary>
/// <remarks>Useful in commands without specific response</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class CreateHandler<TRequest, TEntity> : CreateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected CreateHandler(ICreateRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override ValueTask<Success> GetResponseAsync(TEntity _, TRequest r, CancellationToken c) 
        => Success.TaskValue;
}

/// <summary>
/// Base class for handlers of <see cref="ICommand"/> that validates entities, before create them
/// </summary>
/// <remarks>Useful in commands without specific response and to implement always-valid entities</remarks>
/// <typeparam name="TRequest">The command to handle</typeparam>
/// <typeparam name="TEntity">The entity to create</typeparam>
public abstract class EntityValidatedCreateHandler<TRequest, TEntity> : EntityValidatedCreateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    /// <inheritdoc />
    protected EntityValidatedCreateHandler(ICreateRepository<TEntity> repository) : base(repository)
    { }

    /// <inheritdoc />
    protected internal override ValueTask<Success> GetResponseAsync(TEntity _, TRequest r, CancellationToken c) 
        => Success.TaskValue;
}
