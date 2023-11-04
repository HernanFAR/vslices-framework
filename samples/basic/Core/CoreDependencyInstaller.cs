using Core;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreDependencyInstaller
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        return services
            .AddReflectionSender()
            .AddReflectionPublisher()
            .AddInMemoryEventQueue()
            .AddBackgroundEventListenerService()
            .AddHandlersFromAssemblyContaining<Anchor>()
            .AddEndpointDefinitionsFromAssemblyContaining<Anchor>()
            .AddValidatorsFromAssemblyContaining<Anchor>();
    }
}
