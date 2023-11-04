using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.ExceptionHandling;

/// <summary>
/// Base exception handling behavior
/// </summary>
/// <typeparam name="TRequest">The intercepted request to handle</typeparam>
/// <typeparam name="TResponse">The expected successful response</typeparam>
public abstract class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    /// <inheritdoc/>
    public async ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(ex, request);

            return BusinessFailure.Of.UnhandledException();
        }
    }

    /// <summary>
    /// Processes the exception
    /// </summary>
    /// <remarks>You can add more specific logging, email sending, etc. here</remarks>
    /// <param name="ex">The throw exception</param>
    /// <param name="request">The related request information</param>
    /// <returns>A <see cref="ValueTask"/> representing the processing of the exception</returns>
    protected internal abstract ValueTask ProcessExceptionAsync(Exception ex, TRequest request);
    
}
