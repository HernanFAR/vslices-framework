using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.DataAccess;

public interface ICreateRepository<TEntity>
{
    ValueTask<OneOf<TEntity, BusinessFailure>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IUpdateRepository<TEntity>
{
    ValueTask<OneOf<TEntity, BusinessFailure>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IRemoveRepository<TEntity>
{
    ValueTask<OneOf<TEntity, BusinessFailure>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IReadRepository<TDto>
{
    ValueTask<OneOf<TDto, BusinessFailure>> ReadAsync(CancellationToken cancellationToken = default);
}

public interface IReadRepository<TDto, TSearchOptions>
{
    ValueTask<OneOf<TDto, BusinessFailure>> ReadAsync(TSearchOptions options, CancellationToken cancellationToken = default);
}
