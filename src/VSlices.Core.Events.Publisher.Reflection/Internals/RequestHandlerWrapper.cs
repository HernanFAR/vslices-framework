using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Events.Publisher.Reflection.Strategies;

namespace VSlices.Core.Events.Publisher.Reflection.Internals;

internal abstract class AbstractHandlerWrapper
{
    public abstract ValueTask HandleAsync(
        object request, 
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal abstract class AbstractHandlerWrapper<TResponse> : AbstractHandlerWrapper
{
    public abstract ValueTask HandleAsync(
        IBaseRequest<TResponse> request, 
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal class RequestHandlerWrapper<TRequest, TResponse> : AbstractHandlerWrapper<TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    private readonly IPublishingStrategy _strategy;

    public RequestHandlerWrapper(IPublishingStrategy strategy)
    {
        _strategy = strategy;
    }

    public override async ValueTask HandleAsync(
        object request, IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
        await HandleAsync((IBaseRequest<TResponse>)request, serviceProvider, cancellationToken);

    public override async ValueTask HandleAsync(
        IBaseRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handlers = serviceProvider.GetServices<IHandler<TRequest, TResponse>>();
        
        var handlerDelegates = handlers.Select(handler =>
            {
                ValueTask<Response<TResponse>> Handler()
                {
                    return handler.HandleAsync((TRequest)request, cancellationToken);
                }

                return serviceProvider
                    .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                    .Reverse()
                    .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                        (next, pipeline) => () => pipeline.HandleAsync((TRequest)request, next, cancellationToken));
            })
            .ToArray();

        await _strategy.HandleAsync(handlerDelegates);

    }
}
