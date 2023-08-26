using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Presentation.AspNetCore.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public class Dependency { }
    public class Dependency2 { }

    public class Endpoint : IEndpointDefinition
    {
        public void DefineEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/test", Test);
        }

        public static Task<IResult> Test(HttpContext context) => Task.FromResult<IResult>(EmptyHttpResult.Instance);

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddScoped<Dependency>();
        }
    }
    
    public class Endpoint2 : IEndpointDefinition
    {
        public void DefineEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/test2", Test);
        }

        public static Task<IResult> Test(HttpContext context) => Task.FromResult<IResult>(EmptyHttpResult.Instance);

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddScoped<Dependency2>();
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

    [Fact]
    public void AddEndpointDefinition_ShouldAddSimpleEndpointAndDependencies()
    {
        var services = new ServiceCollection();

        services.AddEndpointDefinition<Endpoint>();

        services
            .Where(e => e.ServiceType == typeof(Dependency))
            .Where(e => e.ImplementationType == typeof(Dependency))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

        services
            .Where(e => e.ServiceType == typeof(ISimpleEndpointDefinition))
            .Where(e => e.ImplementationType == typeof(Endpoint))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddSimpleEndpointDefinition_ShouldAddSimpleEndpoint()
    {
        var services = new ServiceCollection();

        services.AddSimpleEndpointDefinition<Endpoint>();
        
        services
            .Where(e => e.ServiceType == typeof(ISimpleEndpointDefinition))
            .Where(e => e.ImplementationType == typeof(Endpoint))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

        services.Any(e => e.ServiceType == typeof(Dependency))
            .Should().BeFalse();

    }

    [Fact]
    public void AddEndpointDefinitionsFrom_ShouldAddSimpleEndpoint()
    {
        var services = new ServiceCollection();

        services.AddEndpointDefinitionsFrom<Endpoint>();
        
        services
            .Where(e => e.ServiceType == typeof(ISimpleEndpointDefinition))
            .Where(e => e.ImplementationType == typeof(Endpoint))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

        services
            .Where(e => e.ServiceType == typeof(Dependency))
            .Where(e => e.ImplementationType == typeof(Dependency))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

        services
            .Where(e => e.ServiceType == typeof(ISimpleEndpointDefinition))
            .Where(e => e.ImplementationType == typeof(Endpoint2))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

        services
            .Where(e => e.ServiceType == typeof(Dependency2))
            .Where(e => e.ImplementationType == typeof(Dependency2))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
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
}
