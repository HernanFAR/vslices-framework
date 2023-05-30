using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class UpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly IUpdateRepository<TEntity> _repository;

    protected UpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class RequestValidatedUpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly IUpdateRepository<TEntity> _repository;

    protected RequestValidatedUpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await ValidateRequestAsync(request, cancellationToken);

        if (requestValidationResult.IsT1)
        {
            return requestValidationResult.AsT1;
        }

        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class EntityValidatedUpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly IUpdateRepository<TEntity> _repository;

    protected EntityValidatedUpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var entityValidationResult = await ValidateEntityAsync(entity, cancellationToken);

        if (entityValidationResult.IsT1)
        {
            return entityValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity request,
        CancellationToken cancellationToken = default);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class FullyValidatedUpdateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly IUpdateRepository<TEntity> _repository;

    protected FullyValidatedUpdateHandler(IUpdateRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestValidationResult = await ValidateRequestAsync(request, cancellationToken);

        if (requestValidationResult.IsT1)
        {
            return requestValidationResult.AsT1;
        }

        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var entity = await GetAndProcessEntityAsync(request, cancellationToken);

        var entityValidationResult = await ValidateEntityAsync(entity, cancellationToken);

        if (entityValidationResult.IsT1)
        {
            return entityValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.UpdateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TEntity> GetAndProcessEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity request,
        CancellationToken cancellationToken = default);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class UpdateHandler<TRequest, TEntity> : UpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected UpdateHandler(IUpdateRepository<TEntity> repository) : base(repository)
    { }
    
    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}

public abstract class RequestValidatedUpdateHandler<TRequest, TEntity> : RequestValidatedUpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected RequestValidatedUpdateHandler(IUpdateRepository<TEntity> repository) : base(repository) 
    { }
    
    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}

public abstract class DomainValidatedUpdateHandler<TRequest, TEntity> : EntityValidatedUpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected DomainValidatedUpdateHandler(IUpdateRepository<TEntity> repository) : base(repository)
    { }
    
    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}

public abstract class FullyValidatedUpdateHandler<TRequest, TEntity> : FullyValidatedUpdateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected FullyValidatedUpdateHandler(IUpdateRepository<TEntity> repository) : base(repository)
    { }
    
    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}
