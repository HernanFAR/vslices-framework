using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.DataAccess;

public interface ICreatableRepository<TEntity>
{
    Task<OneOf<Success, BusinessFailure>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IUpdateableRepository<TEntity>
{
    Task<OneOf<Success, BusinessFailure>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}


public interface IRemovableRepository<TEntity>
{
    Task<OneOf<Success, BusinessFailure>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IReadableRepository<TDto>
{
    Task<OneOf<TDto, BusinessFailure>> ReadAsync(CancellationToken cancellationToken = default);
}

public interface IReadableRepository<TDto, TOptions>
{
    Task<OneOf<TDto, BusinessFailure>> ReadAsync(TOptions options, CancellationToken cancellationToken = default);
}
