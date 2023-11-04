using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VSlices.Core.Abstracts.Configurations;
using VSlices.Core.Abstracts.Events;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;

namespace VSlices.Core.Abstracts.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public class Sender : ISender
    {
        public ValueTask<Response<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class Publisher : IPublisher
    {
        public ValueTask PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class Behavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class DependencyDefinition1 : IFeatureDependencyDefinition
    {
        public class A { }
        public class B { }

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddTransient<A>();
            services.AddTransient<B>();
        }
    }

    public class DependencyDefinition2 : IFeatureDependencyDefinition
    {
        public class C { }

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddTransient<C>();
        }
    }

    public record Request1 : IRequest { }
    public class Handler1 : IHandler<Request1>
    {
        public ValueTask<Response<Success>> HandleAsync(Request1 request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public record Response2 { }
    public record Request2 : IRequest<Response2> { }
    public class Handler2 : IHandler<Request2, Response2>
    {
        public ValueTask<Response<Response2>> HandleAsync(Request2 request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class EventQueue : IEventQueue
    {
        public ValueTask<IEvent> PeekAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public string BackgroundReaderProvider => throw new NotImplementedException();

        public ValueTask EnqueueAsync(IEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public string BackgroundWriterProvider => throw new NotImplementedException();
    }

    [Fact]
    public void AddSender_ShouldAddSender()
    {
        var services = new ServiceCollection();

        services.AddSender<Sender>();

        services
            .Where(e => e.ServiceType == typeof(ISender))
            .Where(e => e.ImplementationType == typeof(Sender))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddPublisher_ShouldAddPublisher()
    {
        var services = new ServiceCollection();

        services.AddPublisher<Publisher>();

        services
            .Where(e => e.ServiceType == typeof(IPublisher))
            .Where(e => e.ImplementationType == typeof(Publisher))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddPipelineBehavior_ShouldAddOpenBehavior()
    {
        var services = new ServiceCollection();

        services.AddPipelineBehavior(typeof(Behavior<,>));

        services
            .Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Where(e => e.ImplementationType == typeof(Behavior<,>))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddPipelineBehavior_ShouldThrowInvalidOperation()
    {
        var services = new ServiceCollection();

        var act = () => services.AddPipelineBehavior(typeof(int));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddCoreDependenciesFrom_ShouldAddDependenciesInDependencyDefinitions()
    {
        var services = new ServiceCollection();

        services.AddFeatureDependenciesFromAssemblyContaining<Anchor>();

        services.Any(e => e.ImplementationType == typeof(DependencyDefinition1.A))
            .Should().BeTrue();
        services.Any(e => e.ImplementationType == typeof(DependencyDefinition1.B))
            .Should().BeTrue();
        services.Any(e => e.ImplementationType == typeof(DependencyDefinition2.C))
            .Should().BeTrue();
    }

    [Fact]
    public void AddHandlersFrom_ShouldAdHandlerImplementationsUsingTwoGenericOverload()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddHandlersFromAssemblyContaining<Handler1>();


        // Assert
        services
            .Where(e => e.ImplementationType == typeof(Handler1))
            .Any(e => e.ServiceType == typeof(IHandler<Request1, Success>))
            .Should().BeTrue();

        services
            .Where(e => e.ImplementationType == typeof(Handler2))
            .Any(e => e.ServiceType == typeof(IHandler<Request2, Response2>))
            .Should().BeTrue();

    }

    [Fact]
    public void AddEventQueue_ShouldAddEventQueue()
    {
        var services = new ServiceCollection();

        services.AddEventQueue<EventQueue>();

        services
            .Where(e => e.ServiceType == typeof(IEventQueue))
            .Where(e => e.ImplementationType == typeof(EventQueue))
            .Any(e => e.Lifetime == ServiceLifetime.Singleton)
            .Should().BeTrue();

    }

    [Fact]
    public void AddBackgroundEventListenerService_ShouldAddBackgroundEventListenerAndConfiguration_WithoutConfiguration()
    {
        var services = new ServiceCollection();

        services.AddBackgroundEventListenerService();

        services
            .Where(e => e.ServiceType == typeof(IHostedService))
            .Where(e => e.ImplementationType == typeof(BackgroundEventListenerService))
            .Any(e => e.Lifetime == ServiceLifetime.Singleton)
            .Should().BeTrue();

        var descriptor = services
            .Where(e => e.ServiceType == typeof(BackgroundEventListenerConfiguration))
            .Single(e => e.Lifetime == ServiceLifetime.Singleton);

        var opts = (BackgroundEventListenerConfiguration)descriptor.ImplementationInstance!;

        opts.ActionInException.Should().Be(MoveActions.MoveLast);
        opts.MaxRetries.Should().Be(3);


    }

    [Fact]
    public void AddBackgroundEventListenerService_ShouldAddBackgroundEventListenerAndConfiguration_WithConfiguration()
    {
        var services = new ServiceCollection();

        services.AddBackgroundEventListenerService(opt =>
        {
            opt.ActionInException = MoveActions.InmediateRetry;
            opt.MaxRetries = 5;
        });

        services
            .Where(e => e.ServiceType == typeof(IHostedService))
            .Where(e => e.ImplementationType == typeof(BackgroundEventListenerService))
            .Any(e => e.Lifetime == ServiceLifetime.Singleton)
            .Should().BeTrue();

        var descriptor = services
            .Where(e => e.ServiceType == typeof(BackgroundEventListenerConfiguration))
            .Single(e => e.Lifetime == ServiceLifetime.Singleton);

        var opts = (BackgroundEventListenerConfiguration)descriptor.ImplementationInstance!;

        opts.ActionInException.Should().Be(MoveActions.InmediateRetry);
        opts.MaxRetries.Should().Be(5);
    }
}
