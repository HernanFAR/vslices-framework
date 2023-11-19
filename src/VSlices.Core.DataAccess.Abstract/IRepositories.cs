using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.DataAccess.Abstracts;

/// <summary>
/// Defines a repository that can create <typeparamref name="TEntity"/> entities
/// </summary>
/// <typeparam name="TEntity">The entity type to create</typeparam>
public interface ICreateRepository<TEntity>
{
    /// <summary>
    /// Creates a new <typeparamref name="TEntity"/> entity
    /// </summary>
    /// <param name="entity">The values to create the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a repository that can update <typeparamref name="TEntity"/> entities
/// </summary>
/// <typeparam name="TEntity">The entity type to update</typeparam>
public interface IUpdateRepository<TEntity>
{
    /// <summary>
    /// Updates a new <typeparamref name="TEntity"/> entity
    /// </summary>
    /// <param name="entity">The values to update the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a repository that can remove <typeparamref name="TEntity"/> entities
/// </summary>
/// <typeparam name="TEntity">The entity type to remove</typeparam>
public interface IRemoveRepository<TEntity>
{
    /// <summary>
    /// Removes a new <typeparamref name="TEntity"/> entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TEntity>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a repository that response with a <typeparamref name="TDto"/> 
/// </summary>
/// <typeparam name="TDto">The response to get</typeparam>
public interface IReadRepository<TDto>
{
    /// <summary>
    /// Queries data to response with a <see cref="Response{TDto}"/>
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TDto>> ReadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a repository that response with a <typeparamref name="TDto"/> with <typeparamref name="TSearchOptions"/> search options
/// </summary>
/// <typeparam name="TDto"></typeparam>
/// <typeparam name="TSearchOptions"></typeparam>
public interface IReadRepository<TDto, in TSearchOptions>
{
    /// <summary>
    /// Queries data using <typeparamref name="TSearchOptions"/> to response with a <see cref="Response{TDto}"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TDto>> ReadAsync(TSearchOptions options, CancellationToken cancellationToken = default);
}
