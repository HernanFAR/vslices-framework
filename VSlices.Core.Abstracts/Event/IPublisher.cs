using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.Event;

/// <summary>
/// Publishes a event through a event pipeline to be handled by many handlers
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Asynchronously publishes a event to a event pipeline
    /// </summary>
    /// <param name="event">Event</param>
    /// <param name="cancellationToken">CancellationToken</param> 
    /// <returns><see cref="ValueTask"/> representing the action</returns>
    ValueTask PublishAsync(IEvent @event, CancellationToken cancellationToken);

}
