using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.DataAccess.EntityFramework;

/// <summary>
/// Defines a repository that can create <see cref="TEntity"/> entities using EntityFramework Core
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> related to the use case</typeparam>
/// <typeparam name="TEntity">The entity to be created</typeparam>
public abstract class EFCreateRepository<TDbContext, TEntity> : ICreateRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance with a given <see cref="TDbContext"/> and <see cref="ILogger"/>
    /// </summary>
    /// <param name="context">EF Core Context of the use case</param>
    /// <param name="logger">Logger in case of errors</param>
    protected EFCreateRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Message template for concurrency errors
    /// </summary>
    protected internal virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when creating entity of type {EntityType}, with data {EntityJson}";

    /// <summary>
    /// Processes a <see cref="DbUpdateConcurrencyException"/> and returns a <see cref="BusinessFailure"/>, usually with a concurrency error
    /// </summary>
    /// <param name="ex">The concurrency error</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="BusinessFailure"/></returns>
    protected internal virtual ValueTask<BusinessFailure> ProcessConcurrencyExceptionAsync(DbUpdateConcurrencyException ex, CancellationToken cancellationToken)
        => ValueTask.FromResult(BusinessFailure.Of.ConcurrencyError(Array.Empty<string>()));

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA2254", Justification = "Logging template can be translated to other languages in this way")]
    public virtual async ValueTask<Response<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Add(entity);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, ConcurrencyMessageTemplate, typeof(TEntity).Namespace, JsonSerializer.Serialize(entity));

            return await ProcessConcurrencyExceptionAsync(ex, cancellationToken);
        }
    }
}

/// <summary>
/// Defines a repository that can create <see cref="TDbContext"/> with <see cref="TEntity"/> input, using EntityFramework Core, useful to use DDD with database-first scenarios
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> related to the use case</typeparam>
/// <typeparam name="TEntity">The entity to input</typeparam>
/// <typeparam name="TDbEntity">The entity to be created</typeparam>
public abstract class EFCreateRepository<TDbContext, TEntity, TDbEntity> : ICreateRepository<TEntity>
    where TDbContext : DbContext
    where TDbEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance with a given <see cref="TDbContext"/> and <see cref="ILogger"/>
    /// </summary>
    /// <param name="context">EF Core Context of the use case</param>
    /// <param name="logger">Logger in case of errors</param>
    protected EFCreateRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Message template for concurrency errors
    /// </summary>
    protected internal virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when creating entity of type {EntityType}, with data {EntityJson}";

    /// <summary>
    /// Converts a <see cref="TEntity"/> to a <see cref="TDbEntity"/>
    /// </summary>
    /// <param name="entity">The database entity to convert</param>
    /// <returns>A <see cref="TDbEntity"/></returns>
    protected internal abstract TDbEntity ToDatabaseEntity(TEntity entity);

    /// <summary>
    /// Converts a <see cref="TDbEntity"/> to a <see cref="TEntity"/>
    /// </summary>
    /// <param name="entity">The entity to convert</param>
    /// <returns>A <see cref="TEntity"/></returns>
    protected internal abstract TEntity ToEntity(TDbEntity entity);

    /// <summary>
    /// Processes a <see cref="DbUpdateConcurrencyException"/> and returns a <see cref="BusinessFailure"/>, usually with a concurrency error
    /// </summary>
    /// <param name="ex">The concurrency error</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="BusinessFailure"/></returns>
    protected internal virtual ValueTask<BusinessFailure> ProcessConcurrencyExceptionAsync(DbUpdateConcurrencyException ex, CancellationToken cancellationToken)
        => ValueTask.FromResult(BusinessFailure.Of.ConcurrencyError(Array.Empty<string>()));

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA2254", Justification = "Logging template can be translated to other languages in this way")]
    public virtual async ValueTask<Response<TEntity>> CreateAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var entity = ToDatabaseEntity(domain);

        _context.Set<TDbEntity>().Add(entity);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return ToEntity(entity);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, ConcurrencyMessageTemplate, typeof(TDbEntity).Namespace, JsonSerializer.Serialize(entity));

            return await ProcessConcurrencyExceptionAsync(ex, cancellationToken);
        }
    }
}

