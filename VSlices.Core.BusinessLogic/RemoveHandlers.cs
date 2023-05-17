using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class AbstractRemoveHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveHandler(IRemovableRepository<TDomain> repository)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractRemoveRequestValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveRequestValidatedHandler(IRemovableRepository<TDomain> repository)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractRemoveDomainValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveDomainValidatedHandler(IRemovableRepository<TDomain> repository)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var domainValidationResult = await ValidateDomainAsync(domainEntity, cancellationToken);

        if (domainValidationResult.IsT1)
        {
            return domainValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractRemoveFullyValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveFullyValidatedHandler(IRemovableRepository<TDomain> repository)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var domainValidationResult = await ValidateDomainAsync(domainEntity, cancellationToken);

        if (domainValidationResult.IsT1)
        {
            return domainValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractRemoveHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveHandler(IRemovableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected internal ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractRemoveRequestValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveRequestValidatedHandler(IRemovableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected internal ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractRemoveDomainValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveDomainValidatedHandler(IRemovableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var domainValidationResult = await ValidateDomainAsync(domainEntity, cancellationToken);

        if (domainValidationResult.IsT1)
        {
            return domainValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected internal ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractRemoveFullyValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IRemovableRepository<TDomain> _repository;

    protected AbstractRemoveFullyValidatedHandler(IRemovableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var domainValidationResult = await ValidateDomainAsync(domainEntity, cancellationToken);

        if (domainValidationResult.IsT1)
        {
            return domainValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.RemoveAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected internal ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}
