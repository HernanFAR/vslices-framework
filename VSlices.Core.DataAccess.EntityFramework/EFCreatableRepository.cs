using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.DataAccess.EntityFramework;

public abstract class EFCreatableRepository<TDbContext, TEntity> : ICreatableRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFCreatableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when creating entity of type {EntityType}, with data {Entity}";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA2254", Justification = "Logging template can be translated to other languages in this way")]
    public virtual async ValueTask<OneOf<Success, BusinessFailure>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Add(entity);

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

public abstract class EFCreatableRepository<TDbContext, TDomain, TEntity> : ICreatableRepository<TDomain>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _context;
    private readonly ILogger _logger;

    protected EFCreatableRepository(TDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected virtual string ConcurrencyMessageTemplate
        => "There was a concurrency error when creating entity of type {EntityType}, with data {Entity}";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA2254", Justification = "Logging template can be translated to other languages in this way")]
    public virtual async ValueTask<OneOf<Success, BusinessFailure>> CreateAsync(TDomain domain, CancellationToken cancellationToken = default)
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

    protected abstract ValueTask<TEntity> DomainToDatabaseEntityAsync(TDomain domain, CancellationToken cancellationToken = default);
}
