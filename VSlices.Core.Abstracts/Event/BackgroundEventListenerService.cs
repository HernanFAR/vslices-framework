using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VSlices.Core.Abstracts.Event;

/// <summary>
/// Listens to a event queue and publishes the event to a event pipeline
/// </summary>
public class BackgroundEventListenerService : BackgroundService
{
    private readonly ILogger<BackgroundEventListenerService> _logger;
    private readonly IPublisher _publisher;
    private readonly IEventQueueReader _eventQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundEventListenerService"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="publisher">Publisher</param>
    /// <param name="eventQueue">EventQueueReader</param>
    public BackgroundEventListenerService(ILogger<BackgroundEventListenerService> logger,
        IPublisher publisher,
        IEventQueueReader eventQueue)
    {
        _logger = logger;
        _publisher = publisher;
        _eventQueue = eventQueue;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Background event listener service ({_eventQueue.BackgroundProvider}) is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await BackgroundProcessing(stoppingToken);
        }
    }

    /// <summary>
    /// Describes the form of the background processing
    /// </summary>
    /// <remarks>
    /// This method is class inside a while loop
    /// </remarks>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected virtual async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var workItem = await _eventQueue.PeekAsync(stoppingToken);

        try
        {
            await _publisher.PublishAsync(workItem, stoppingToken);

            await _eventQueue.DequeueAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred executing {WorkItem}.", nameof(workItem));
        }
    }
}
