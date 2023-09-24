using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.Core.Events.Publisher.Reflection.Strategies;

public class AwaitForEachStrategy : IPublishingStrategy
{
    public async ValueTask HandleAsync<TResponse>(RequestHandlerDelegate<TResponse>[] handlerDelegates)
    {
        foreach (var handlerDelegate in handlerDelegates)
        {
            _ = await handlerDelegate();
        }
    }
}
