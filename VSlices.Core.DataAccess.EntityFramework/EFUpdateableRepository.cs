using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.DataAccess;

public abstract class EFUpdateableRepository<TDbContext, TEntity> : IUpdateableRepository<TEntity>
    where TDbContext : DbContext 
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFUpdateableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate 
        => "There was a concurrency error when updating entity of type {EntityType}, with data {Entity}";

    public virtual async Task<OneOf<Success, BusinessFailure>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(entity);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, ConcurrencyMessageTemplate, typeof(TEntity).Namespace, JsonSerializer.Serialize(entity));

            return BusinessFailure.Of.ConcurrencyError(Array.Empty<string>());
        }
    }
}

public abstract class EFUpdateableRepository<TDbContext, TDomain, TEntity> : IUpdateableRepository<TDomain>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFUpdateableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when updating entity of type {EntityType}, with data {Entity}";

    public virtual async Task<OneOf<Success, BusinessFailure>> UpdateAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var databaseEntity = await DomainToDatabaseEntityAsync(domain, cancellationToken);

        _context.Set<TEntity>().Add(databaseEntity);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, ConcurrencyMessageTemplate, typeof(TEntity).Namespace, JsonSerializer.Serialize(databaseEntity));

            return BusinessFailure.Of.ConcurrencyError(Array.Empty<string>());
        }
    }

    protected abstract Task<TEntity> DomainToDatabaseEntityAsync(TDomain domain, CancellationToken cancellationToken = default);
}
