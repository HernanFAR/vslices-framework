using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IHandler<in TRequest> : IHandler<TRequest, Success>
    where TRequest : IRequest<Success>
{ }

public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public delegate ValueTask<Response<TResponse>> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}