using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.Core.Abstracts.Event;

/// <summary>
/// Represents a queue writer of events
/// </summary>
public interface IEventQueueWriter
{
    /// <summary>
    /// Asynchronously enqueue a event to the queue
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask EnqueueAsync(IEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// The background provider of the queue
    /// </summary>
    string BackgroundWriterProvider { get; }
}

/// <summary>
/// Represents a queue reader of events
/// </summary>
public interface IEventQueueReader
{
    /// <summary>
    /// Asynchronously dequeue the next event from the queue
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>the dequeued event from the queue</returns>
    ValueTask<IEvent> DequeueAsync(CancellationToken cancellationToken);

    /// <summary>
    /// The background provider of the queue
    /// </summary>
    string BackgroundReaderProvider { get; }
}

/// <summary>
/// Represents a queue of events, with write and read capabilities
/// </summary>
public interface IEventQueue : IEventQueueReader, IEventQueueWriter
{
}
