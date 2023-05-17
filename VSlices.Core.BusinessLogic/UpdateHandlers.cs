using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class AbstractUpdateHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateRequestValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateRequestValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateDomainValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateDomainValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateFullyValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateFullyValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var domainEntity = await GetDomainEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected Task<Success> GetResponseAsync(TDomain _, TRequest __, CancellationToken ___ = default)
        => Task.FromResult(new Success());
}

public abstract class AbstractUpdateRequestValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateRequestValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected Task<Success> GetResponseAsync(TDomain _, TRequest __, CancellationToken ___ = default)
        => Task.FromResult(new Success());
}

public abstract class AbstractUpdateDomainValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateDomainValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected Task<Success> GetResponseAsync(TDomain _, TRequest __, CancellationToken ___ = default)
        => Task.FromResult(new Success());
}

public abstract class AbstractUpdateFullyValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateFullyValidatedHandler(IUpdateableRepository<TDomain> repository)
    {
        _repository = repository;
    }

    public virtual async Task<OneOf<Success, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected Task<Success> GetResponseAsync(TDomain _, TRequest __, CancellationToken ___ = default)
        => Task.FromResult(new Success());
}
