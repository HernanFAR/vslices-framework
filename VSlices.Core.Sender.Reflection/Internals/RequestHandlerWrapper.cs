using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Sender.Reflection.Internals;

internal abstract class AbstractHandlerWrapper
{
    public abstract ValueTask<Response<object?>> HandleAsync(
        object request, 
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal abstract class AbstractHandlerWrapper<TResponse> : AbstractHandlerWrapper
{
    public abstract ValueTask<Response<TResponse>> HandleAsync(
        IBaseRequest<TResponse> request, 
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal class RequestHandlerWrapper<TRequest, TResponse> : AbstractHandlerWrapper<TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    public override async ValueTask<Response<object?>> HandleAsync(
        object request, IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
        await HandleAsync((IBaseRequest<TResponse>)request, serviceProvider, cancellationToken);

    public override ValueTask<Response<TResponse>> HandleAsync(
        IBaseRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ValueTask<Response<TResponse>> Handler()
        {
            return serviceProvider.GetRequiredService<IHandler<TRequest, TResponse>>()
                .HandleAsync((TRequest)request, cancellationToken);
        }

        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => () => pipeline.HandleAsync((TRequest)request, next, cancellationToken))();
    }
}
