using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.DataAccess;

public abstract class EFRemovableRepository<TDbContext, TEntity> : IRemovableRepository<TEntity>
    where TDbContext : DbContext 
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFRemovableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate 
        => "There was a concurrency error when removing entity of type {EntityType}, with data {Entity}";

    public virtual async  Task<OneOf<Success, BusinessFailure>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Remove(entity);

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

public abstract class EFRemovableRepository<TDbContext, TDomain, TEntity> : IRemovableRepository<TDomain>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFRemovableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when removing entity of type {EntityType}, with data {Entity}";

    public virtual async Task<OneOf<Success, BusinessFailure>> RemoveAsync(TDomain domain, CancellationToken cancellationToken = default)
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
