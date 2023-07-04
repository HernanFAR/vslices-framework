using System.Runtime.ExceptionServices;
using OneOf;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.ExceptionHandling;

public abstract class AbstractExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TRequest> 
    where TRequest : IRequest<TRequest>
{
    public async ValueTask<OneOf<TRequest, BusinessFailure>> HandleAsync(TRequest request, RequestHandlerDelegate<TRequest> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(ex);

            return BusinessFailure.Of.UnhandledException();
        }
    }

    protected abstract ValueTask ProcessExceptionAsync(Exception ex);
}
