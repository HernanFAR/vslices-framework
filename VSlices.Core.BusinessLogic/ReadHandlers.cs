using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class ReadHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TSearchOptions> _repository;

    protected ReadHandler(IReadableRepository<TResponse, TSearchOptions> repository)
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

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class ReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TRequest> _repository;

    protected ReadHandler(IReadableRepository<TResponse, TRequest> repository)
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

        return await _repository.ReadAsync(request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class BasicReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse> _repository;

    protected BasicReadHandler(IReadableRepository<TResponse> repository)
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

        return await _repository.ReadAsync(cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class RequestValidatedReadHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TSearchOptions> _repository;

    protected RequestValidatedReadHandler(IReadableRepository<TResponse, TSearchOptions> repository)
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

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class RequestValidatedReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse, TRequest> _repository;

    protected RequestValidatedReadHandler(IReadableRepository<TResponse, TRequest> repository)
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

        return await _repository.ReadAsync(request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class RequestValidatedBasicReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
{
    private readonly IReadableRepository<TResponse> _repository;

    protected RequestValidatedBasicReadHandler(IReadableRepository<TResponse> repository)
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

        return await _repository.ReadAsync(cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}
