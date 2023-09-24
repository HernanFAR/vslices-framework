using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.Core.Events.Publisher.Reflection.Strategies;

public interface IPublishingStrategy
{
    ValueTask HandleAsync<TResponse>(RequestHandlerDelegate<TResponse>[] handlerDelegates);
}
