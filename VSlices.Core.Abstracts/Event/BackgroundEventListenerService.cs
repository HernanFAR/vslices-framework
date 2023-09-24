using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Configurations;
using System.Text.Json;

namespace VSlices.Core.Abstracts.Event;

/// <summary>
/// Listens to a event queue and publishes the event to a event pipeline
/// </summary>
public sealed class BackgroundEventListenerService : BackgroundService
{
    private readonly ILogger<BackgroundEventListenerService> _logger;
    private readonly IPublisher _publisher;
    private readonly IEventQueue _eventQueue;
    private readonly BackgroundEventListenerConfiguration _config;
    private readonly Dictionary<Guid, int> _retries = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundEventListenerService"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="publisher">Publisher</param>
    /// <param name="eventQueue">EventQueueReader</param>
    /// <param name="config">Configuration</param>
    public BackgroundEventListenerService(ILogger<BackgroundEventListenerService> logger,
        IPublisher publisher,
        IEventQueue eventQueue,
        BackgroundEventListenerConfiguration config)
    {
        _logger = logger;
        _publisher = publisher;
        _eventQueue = eventQueue;
        _config = config;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background event listener service ({ReaderProvider}) is running.", _eventQueue.BackgroundReaderProvider);

        while (!stoppingToken.IsCancellationRequested)
        {
            await BackgroundProcessing(stoppingToken);
        }
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        var workItem = await _eventQueue.DequeueAsync(stoppingToken);
        var retry = false;

        do
        {
            try
            {
                await _publisher.PublishAsync(workItem, stoppingToken);

                _retries.Remove(workItem.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", workItem.GetType().FullName);

                retry = await CheckRetry(workItem, stoppingToken);
            }
        } while (retry);
    }

    private async Task<bool> CheckRetry(IEvent workItem, CancellationToken stoppingToken)
    {
        if (_retries.TryGetValue(workItem.Id, out var retries))
        {
            _retries[workItem.Id] = retries + 1;
        }
        else
        {
            _retries.Add(workItem.Id, 1);
        }

        if (_retries[workItem.Id] > _config.MaxRetries)
        {
            _logger.LogError("Max retries {RetryLimit} reached for {WorkItem}.", _config.MaxRetries, JsonSerializer.Serialize(workItem));
            _retries.Remove(workItem.Id);

            return false;
        }
        
        switch (_config.ActionInException)
        {
            case MoveActions.MoveLast:
                await _eventQueue.EnqueueAsync(workItem, stoppingToken);

                return false;

            case MoveActions.InmediateRetry:

                return true;

            default:
                throw new ArgumentOutOfRangeException(nameof(_config.ActionInException));
        }
    }
}
