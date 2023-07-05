using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.DataAccess;

public interface ICreateRepository<TEntity>
{
    ValueTask<Response<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IUpdateRepository<TEntity>
{
    ValueTask<Response<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IRemoveRepository<TEntity>
{
    ValueTask<Response<TEntity>> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IReadRepository<TDto>
{
    ValueTask<Response<TDto>> ReadAsync(CancellationToken cancellationToken = default);
}

public interface IReadRepository<TDto, TSearchOptions>
{
    ValueTask<Response<TDto>> ReadAsync(TSearchOptions options, CancellationToken cancellationToken = default);
}
