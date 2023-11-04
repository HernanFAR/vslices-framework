using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Event;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Events.EventQueue.InMemory;

namespace VSlices.Core.InMemoryQueue.ReflectionPublisher.IntegTests;

public class EventFlow
{
    public record AlwaysSuccessEvent() : EventBase;

    public class AlwaysSuccessHandler : IHandler<AlwaysSuccessEvent>
    {
        public AutoResetEvent HandledEvent { get; } = new(false);

        public ValueTask<Response<Success>> HandleAsync(AlwaysSuccessEvent request, CancellationToken cancellationToken = default)
        {
            HandledEvent.Set();

            return ResponseDefaults.TaskSuccess;
        }
    }

    public record FirstFailureThenSuccessEvent() : EventBase;

    public class FirstFailureThenSuccessHandler : IHandler<FirstFailureThenSuccessEvent>
    {
        public AutoResetEvent HandledEvent { get; } = new(false);

        public bool First { get; set; } = true;

        public ValueTask<Response<Success>> HandleAsync(FirstFailureThenSuccessEvent request, CancellationToken cancellationToken = default)
        {
            if (First)
            {
                First = false;
                throw new Exception("First failure");
            }

            HandledEvent.Set();

            return ResponseDefaults.TaskSuccess;
        }
    }

    public record FirstAndSecondFailureThenSuccessEvent() : EventBase;

    public class FirstAndSecondFailureThenSuccessHandler : IHandler<FirstAndSecondFailureThenSuccessEvent>
    {
        public AutoResetEvent HandledEvent { get; } = new(false);

        public bool First { get; set; } = true;

        public bool Second { get; set; } = true;

        public ValueTask<Response<Success>> HandleAsync(FirstAndSecondFailureThenSuccessEvent request, CancellationToken cancellationToken = default)
        {
            if (First)
            {
                First = false;

                throw new Exception("First failure");
            }

            if (Second)
            {
                Second = false;
                throw new Exception("Second failure");
            }

            HandledEvent.Set();

            return ResponseDefaults.TaskSuccess;
        }
    }

    public record AlwaysFailureEvent() : EventBase;

    public class AlwaysFailureHandler : IHandler<AlwaysFailureEvent>
    {
        public ValueTask<Response<Success>> HandleAsync(AlwaysFailureEvent request, CancellationToken cancellationToken = default)
        {
            throw new Exception("Always failure");
        }
    }

    [Fact]
    public async Task InMemoryEventFlow_AddedEventBeforeListeningStart_AlwaysSuccessHandler()
    {
        var provider = new ServiceCollection()
            .AddBackgroundEventListenerService()
            .AddInMemoryEventQueue()
            .AddReflectionPublisher()
            .AddLogging()
            .AddSingleton<AlwaysSuccessHandler>()
            .AddScoped<IHandler<AlwaysSuccessEvent, Success>>(s => s.GetRequiredService<AlwaysSuccessHandler>())
            .BuildServiceProvider();

        var backgroundEventListener = (BackgroundEventListenerService)provider.GetRequiredService<IHostedService>();
        var eventQueue = (InMemoryEventQueue)provider.GetRequiredService<IEventQueueWriter>();

        var event1 = new AlwaysSuccessEvent();

        await eventQueue.EnqueueAsync(event1, default);

        _ = backgroundEventListener.StartAsync(default);

        var handler = provider.GetRequiredService<AlwaysSuccessHandler>();
        handler.HandledEvent.WaitOne(5000).Should().BeTrue();
    }

    [Fact]
    public async Task InMemoryEventFlow_AddedEventAfterListeningStart()
    {
        var provider = new ServiceCollection()
            .AddBackgroundEventListenerService()
            .AddInMemoryEventQueue()
            .AddReflectionPublisher()
            .AddLogging()
            .AddSingleton<AlwaysSuccessHandler>()
            .AddScoped<IHandler<AlwaysSuccessEvent, Success>>(s => s.GetRequiredService<AlwaysSuccessHandler>())
            .BuildServiceProvider();

        var backgroundEventListener = (BackgroundEventListenerService)provider.GetRequiredService<IHostedService>();
        var eventQueue = (InMemoryEventQueue)provider.GetRequiredService<IEventQueueWriter>();

        var event2 = new AlwaysSuccessEvent();

        _ = backgroundEventListener.StartAsync(default);
        await Task.Delay(1000);

        await eventQueue.EnqueueAsync(event2, default);

        var handler = provider.GetRequiredService<AlwaysSuccessHandler>();
        handler.HandledEvent.WaitOne(5000).Should().BeTrue();
    }

    [Fact]
    public async Task InMemoryEventFlow_FirstRetry()
    {
        var provider = new ServiceCollection()
            .AddBackgroundEventListenerService()
            .AddInMemoryEventQueue()
            .AddReflectionPublisher()
            .AddLogging()
            .AddSingleton<FirstFailureThenSuccessHandler>()
            .AddScoped<IHandler<FirstFailureThenSuccessEvent, Success>>(s => s.GetRequiredService<FirstFailureThenSuccessHandler>())
            .BuildServiceProvider();

        var backgroundEventListener = (BackgroundEventListenerService)provider.GetRequiredService<IHostedService>();
        var eventQueue = (InMemoryEventQueue)provider.GetRequiredService<IEventQueueWriter>();

        var event2 = new FirstFailureThenSuccessEvent();

        _ = backgroundEventListener.StartAsync(default);
        await Task.Delay(1000);

        await eventQueue.EnqueueAsync(event2, default);

        var handler = provider.GetRequiredService<FirstFailureThenSuccessHandler>();
        
        handler.HandledEvent.WaitOne(5000).Should().BeTrue();
    }

    [Fact]
    public async Task InMemoryEventFlow_SecondRetry()
    {
        var provider = new ServiceCollection()
            .AddBackgroundEventListenerService()
            .AddInMemoryEventQueue()
            .AddReflectionPublisher()
            .AddLogging()
            .AddSingleton<FirstAndSecondFailureThenSuccessHandler>()
            .AddScoped<IHandler<FirstAndSecondFailureThenSuccessEvent, Success>>(s => s.GetRequiredService<FirstAndSecondFailureThenSuccessHandler>())
            .BuildServiceProvider();

        var backgroundEventListener = (BackgroundEventListenerService)provider.GetRequiredService<IHostedService>();
        var eventQueue = (InMemoryEventQueue)provider.GetRequiredService<IEventQueueWriter>();

        var event2 = new FirstAndSecondFailureThenSuccessEvent();

        _ = backgroundEventListener.StartAsync(default);
        await Task.Delay(1000);

        await eventQueue.EnqueueAsync(event2, default);

        var handler = provider.GetRequiredService<FirstAndSecondFailureThenSuccessHandler>();
        
        handler.HandledEvent.WaitOne(5000).Should().BeTrue();
    }

    [Fact]
    public async Task InMemoryEventFlow_ThirdTry()
    {
        var logger = Mock.Of<ILogger<BackgroundEventListenerService>>();
        var loggerMock = Mock.Get(logger);

        var provider = new ServiceCollection()
            .AddBackgroundEventListenerService()
            .AddInMemoryEventQueue()
            .AddReflectionPublisher()
            .AddScoped(_ => logger)
            .AddSingleton<AlwaysFailureHandler>()
            .AddScoped<IHandler<AlwaysFailureEvent, Success>>(s => s.GetRequiredService<AlwaysFailureHandler>())
            .BuildServiceProvider();

        var backgroundEventListener = (BackgroundEventListenerService)provider.GetRequiredService<IHostedService>();
        var eventQueue = (InMemoryEventQueue)provider.GetRequiredService<IEventQueueWriter>();

        var event2 = new AlwaysFailureEvent();

        _ = backgroundEventListener.StartAsync(default);
        await Task.Delay(1000);

        await eventQueue.EnqueueAsync(event2, default);

        await Task.Delay(5000);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf($"Max retries 3 reached for {JsonConvert.SerializeObject(event2)}.", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }
}