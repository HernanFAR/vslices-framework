﻿using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Requests;

namespace VSlices.Core.Handlers;

/// <summary>
/// Base class for handlers of <see cref="IQuery{TResponse}"/> that reads data
/// </summary>
/// <typeparam name="TRequest">The query to handle</typeparam>
/// <typeparam name="TSearchOptions">The options to complete the read process</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
public abstract class ReadHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse, TSearchOptions> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IReadRepository{TResponse, TSearchOptions}"/>
    /// </summary>
    /// <param name="repository">Repository with read function</param>
    protected ReadHandler(IReadRepository<TResponse, TSearchOptions> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var featureValidationResult = await ValidateFeatureRulesAsync(request, cancellationToken);

        if (featureValidationResult.IsFailure)
        {
            return featureValidationResult.BusinessFailure;
        }

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    /// <summary>
    /// Validates the use case rules
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateFeatureRulesAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Converts the <typeparamref name="TRequest"/> to <typeparamref name="TSearchOptions"/>
    /// </summary>
    /// <param name="request">The request to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <typeparamref name="TSearchOptions"/></returns>
    protected internal abstract ValueTask<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for handlers of <see cref="IQuery{TResponse}"/> that reads data.
/// </summary>
/// <typeparam name="TRequest">The query to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>

public abstract class ReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse, TRequest> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IReadRepository{TResponse, TSearchOptions}"/>
    /// </summary>
    /// <param name="repository">Repository with read function</param>
    protected ReadHandler(IReadRepository<TResponse, TRequest> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var featureValidationResult = await ValidateFeatureRulesAsync(request, cancellationToken);

        if (featureValidationResult.IsFailure)
        {
            return featureValidationResult.BusinessFailure;
        }

        return await _repository.ReadAsync(request, cancellationToken);
    }

    /// <summary>
    /// Validates the use case rules
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>>
    protected internal abstract ValueTask<Response<Success>> ValidateFeatureRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for handlers of <see cref="IQuery{TResponse}"/> that reads basic data.
/// </summary>
/// <typeparam name="TRequest">The query to handle</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
public abstract class BasicReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse> _repository;

    /// <summary>
    /// Creates a new instance using the given <see cref="IReadRepository{TResponse}"/>
    /// </summary>
    /// <param name="repository">Repository with read function</param>
    protected BasicReadHandler(IReadRepository<TResponse> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var useCaseValidationResult = await ValidateFeatureRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsFailure)
        {
            return useCaseValidationResult.BusinessFailure;
        }

        return await _repository.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Validates the use case rules
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected internal abstract ValueTask<Response<Success>> ValidateFeatureRulesAsync(TRequest request, CancellationToken cancellationToken);
}
