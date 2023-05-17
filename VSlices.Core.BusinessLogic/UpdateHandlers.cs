﻿using OneOf;
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

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateRequestValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateRequestValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateDomainValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateDomainValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateFullyValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateFullyValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken);
}

public abstract class AbstractUpdateHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractUpdateRequestValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateRequestValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractUpdateDomainValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateDomainValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}

public abstract class AbstractUpdateFullyValidatedHandler<TRequest, TDomain> : IHandler<TRequest, Success>
{
    private readonly IUpdateableRepository<TDomain> _repository;

    protected AbstractUpdateFullyValidatedHandler(IUpdateableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.UpdateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken);

    protected abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Can not use two or more _")]
    protected ValueTask<Success> GetResponseAsync(TDomain _, TRequest r, CancellationToken c = default)
        => ValueTask.FromResult(new Success());
}
