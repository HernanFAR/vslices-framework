using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class AbstractReadHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TSearchOptions> _repository;

    protected AbstractReadHandler(IReadableRepository<TResponse, TSearchOptions> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class AbstractReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TRequest> _repository;

    protected AbstractReadHandler(IReadableRepository<TResponse, TRequest> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        return await _repository.ReadAsync(request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class AbstractBasicReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse> _repository;

    protected AbstractBasicReadHandler(IReadableRepository<TResponse> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        return await _repository.ReadAsync(cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class AbstractReadRequestValidatedHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TSearchOptions> _repository;

    protected AbstractReadRequestValidatedHandler(IReadableRepository<TResponse, TSearchOptions> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class AbstractReadRequestValidatedHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TRequest> _repository;

    protected AbstractReadRequestValidatedHandler(IReadableRepository<TResponse, TRequest> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        return await _repository.ReadAsync(request, cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class AbstractBasicReadRequestValidatedHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse> _repository;

    protected AbstractBasicReadRequestValidatedHandler(IReadableRepository<TResponse> repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
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

        return await _repository.ReadAsync(cancellationToken);
    }

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}
