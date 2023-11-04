using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VSlices.Core.Abstracts.Event;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using VSlices.Core.Events.EventQueue.InMemory;
using VSlices.Core.Events.Publisher.Reflection;
using VSlices.Core.Presentation.AspNetCore;
using VSlices.Core.Sender.Reflection;

namespace VSlices.ShortCuts.Core.DistributedMonolith.AspNetFVEFReflection.UnitTests;

public class Dependency { }

public class Endpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        throw new NotImplementedException();
    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<Dependency>();
    }
}

public record Request : IRequest;

public class Handler : IHandler<Request>
{
    public ValueTask<Response<Success>> HandleAsync(Request request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x).NotNull();
    }
}

public class DistributedMonolithExtensionsUnitTests
{
    [Fact]
    public void AddDistributedMonolithServicesAndHandlersFromAssemblyContaining_ShouldAddRelatedServices()
    {
        var services = new ServiceCollection();

        services.AddDistributedMonolithServicesAndHandlersFromAssemblyContaining<DistributedMonolithExtensionsUnitTests>();

        services.Should().Contain(x => x.ServiceType == typeof(ISender) && x.ImplementationType == typeof(ReflectionSender));
        services.Should().Contain(x => x.ServiceType == typeof(IPublisher) && x.ImplementationType == typeof(ReflectionPublisher));
        services.Should().Contain(x => x.ServiceType == typeof(IEventQueue) && x.ImplementationType == typeof(InMemoryEventQueue));
        services.Should().Contain(x => x.ServiceType == typeof(IEventQueueWriter) && x.ImplementationFactory != null);
        services.Should().Contain(x => x.ServiceType == typeof(IEventQueueReader) && x.ImplementationFactory != null);
        services.Should().Contain(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(BackgroundEventListenerService));
        services.Should().Contain(x => x.ServiceType == typeof(IHandler<Request, Success>) && x.ImplementationType == typeof(Handler));
        services.Should().Contain(x => x.ServiceType == typeof(ISimpleEndpointDefinition) && x.ImplementationType == typeof(Endpoint));
        services.Should().Contain(x => x.ServiceType == typeof(Dependency));
        services.Should().Contain(x => x.ServiceType == typeof(IValidator<Request>) && x.ImplementationType == typeof(RequestValidator));

    }
}