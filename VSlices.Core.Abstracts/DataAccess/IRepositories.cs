using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.DataAccess;

public interface ICreatableRepository<TEntity>
{
    ValueTask<OneOf<Success, BusinessFailure>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IUpdateableRepository<TEntity>
{
    ValueTask<OneOf<Success, BusinessFailure>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}


public interface IRemovableRepository<TEntity>
{
    ValueTask<OneOf<Success, BusinessFailure>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IReadableRepository<TDto>
{
    ValueTask<OneOf<TDto, BusinessFailure>> ReadAsync(CancellationToken cancellationToken = default);
}

public interface IReadableRepository<TDto, TSearchOptions>
{
    ValueTask<OneOf<TDto, BusinessFailure>> ReadAsync(TSearchOptions options, CancellationToken cancellationToken = default);
}
