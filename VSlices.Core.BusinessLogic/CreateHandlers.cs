using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class AbstractCreateHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly ICreatableRepository<TDomain> _repository;

    protected AbstractCreateHandler(ICreatableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.CreateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken = default);
}

public abstract class AbstractCreateRequestValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly ICreatableRepository<TDomain> _repository;

    protected AbstractCreateRequestValidatedHandler(ICreatableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.CreateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken = default);
}

public abstract class AbstractCreateDomainValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly ICreatableRepository<TDomain> _repository;

    protected AbstractCreateDomainValidatedHandler(ICreatableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.CreateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain domain,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken = default);
}

public abstract class AbstractCreateFullyValidatedHandler<TRequest, TResponse, TDomain> : IHandler<TRequest, TResponse>
{
    private readonly ICreatableRepository<TDomain> _repository;

    protected AbstractCreateFullyValidatedHandler(ICreatableRepository<TDomain> repository)
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

        var dataAccessResult = await _repository.CreateAsync(domainEntity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return await GetResponseAsync(domainEntity, request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TDomain> GetDomainEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(TDomain request,
        CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> GetResponseAsync(TDomain domainEntity, TRequest request,
        CancellationToken cancellationToken = default);
}