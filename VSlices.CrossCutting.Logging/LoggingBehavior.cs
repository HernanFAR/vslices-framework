using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;
using VSlices.CrossCutting.Logging.Attributes;
using VSlices.CrossCutting.Logging.Configurations;

namespace VSlices.CrossCutting.Logging;

/// <summary>
/// Base logging behavior
/// </summary>
/// <remarks>Logs at start, successful end and failed end</remarks>
/// <typeparam name="TRequest">The intercepted request to log about</typeparam>
/// <typeparam name="TResponse">The expected successful response</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly LoggingConfiguration _configuration;

    /// <summary>
    /// Creates a new instance using the provided <see cref="ILogger{TRequest}"/> and <see cref="LoggingConfiguration"/>
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Configuration to use</param>
    public LoggingBehavior(ILogger<TRequest> logger, LoggingConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public virtual async ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        InitialHandling(request);

        var result = await next();

        Action<Response<TResponse>, TRequest> handler = result.IsSuccess ? SuccessHandling : FailureHandling;

        handler(result, request);

        return result;
    }

    /// <summary>
    /// Logs information about the request, at the start of request 
    /// </summary>
    /// <param name="request">Request to log about</param>
    protected internal virtual void InitialHandling(TRequest request)
    {
        if (_configuration.SerializeAll || !typeof(TRequest).IsDefined(typeof(NoLoggableAttribute), false))
        {
            _logger.LogInformation(_configuration.Describer.Initial,
                DateTime.Now, typeof(TRequest).FullName, JsonSerializer.Serialize(request, _configuration.JsonOptions));
        }
        else
        {
            _logger.LogInformation(_configuration.Describer.InitialWithoutProperties,
                DateTime.Now, typeof(TRequest).FullName);
        }
    }

    /// <summary>
    /// Logs information about the request, only if it was successful
    /// </summary>
    /// <param name="response">Response to log about</param>
    /// <param name="request">Request to log about</param>
    protected internal virtual void SuccessHandling(Response<TResponse> response, TRequest request)
    {
        if (_configuration.SerializeAll || !typeof(TResponse).IsDefined(typeof(NoLoggableAttribute), false))
        {
            _logger.LogInformation(_configuration.Describer.Success,
                DateTime.Now, typeof(TRequest).FullName, JsonSerializer.Serialize(response.SuccessValue, _configuration.JsonOptions));
        }
        else
        {
            _logger.LogInformation(_configuration.Describer.SuccessWithoutProperties,
                DateTime.Now, typeof(TRequest).FullName);
        }
    }

    /// <summary>
    /// Logs information about the request, only if it failed
    /// </summary>
    /// <param name="response">Response to log about</param>
    /// <param name="request">Request to log about</param>
    protected internal virtual void FailureHandling(Response<TResponse> response, TRequest request)
    {
        _logger.LogWarning(_configuration.Describer.Failure,
            DateTime.Now, typeof(TRequest).FullName, JsonSerializer.Serialize(response.BusinessFailure, _configuration.JsonOptions));
    }
}
