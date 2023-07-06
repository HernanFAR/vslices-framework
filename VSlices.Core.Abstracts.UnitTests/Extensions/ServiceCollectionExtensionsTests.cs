using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using static VSlices.Core.Abstracts.UnitTests.Extensions.ServiceCollectionExtensionsTests.DependencyDefinition1;

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

    public class Behavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class DependencyDefinition1 : IUseCaseDependencyDefinition
    {
        public class A { }
        public class B { }

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddTransient<A>();
            services.AddTransient<B>();
        }
    }

    public class DependencyDefinition2 : IUseCaseDependencyDefinition
    {
        public class C { }

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddTransient<C>();
        }
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

        services.AddCoreDependenciesFrom<Anchor>();
        
        services.Any(e => e.ImplementationType == typeof(DependencyDefinition1.A))
            .Should().BeTrue();
        services.Any(e => e.ImplementationType == typeof(DependencyDefinition1.B))
            .Should().BeTrue();
        services.Any(e => e.ImplementationType == typeof(DependencyDefinition2.C))
            .Should().BeTrue();
    }
}
