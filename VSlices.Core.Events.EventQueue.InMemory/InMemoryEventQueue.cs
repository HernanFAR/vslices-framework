using System.Threading.Channels;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Event;

namespace VSlices.Core.Events.EventQueue.InMemory;


/// <summary>
/// Allows publish, peek and dequeue events through a in memory channel
/// </summary>
public sealed class InMemoryEventQueue : IEventQueue
{
    internal readonly Channel<IEvent> _channel;

    /// <summary>
    /// Creates a new instance of <see cref="InMemoryEventQueue"/>
    /// </summary>
    /// <param name="inMemoryPublishConfigOptions"></param>
    public InMemoryEventQueue(InMemoryEventQueueConfiguration inMemoryPublishConfigOptions)
    {
        var options = new BoundedChannelOptions(inMemoryPublishConfigOptions.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        
        _channel = Channel.CreateBounded<IEvent>(options);
    }

    /// <inheritdoc />
    public async ValueTask<IEvent> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _channel.Reader.ReadAsync(cancellationToken);

        return workItem;
    }

    /// <inheritdoc />
    public async ValueTask EnqueueAsync(IEvent @event, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }
}
