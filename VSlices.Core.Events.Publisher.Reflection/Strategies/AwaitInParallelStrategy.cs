using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.Core.Events.Publisher.Reflection.Strategies;

public class AwaitInParallelStrategy : IPublishingStrategy
{
    public async ValueTask HandleAsync<TResponse>(RequestHandlerDelegate<TResponse>[] handlerDelegates)
    {
        await Task.WhenAll(handlerDelegates.Select(async handlerDelegate => await handlerDelegate()));
    }
}