using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.ExceptionHandling;

public abstract class AbstractExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(ex);

            return Response(ex);
        }
    }

    protected internal abstract ValueTask ProcessExceptionAsync(Exception ex);

    protected internal virtual BusinessFailure Response(Exception ex) => BusinessFailure.Of.UnhandledException();
}
