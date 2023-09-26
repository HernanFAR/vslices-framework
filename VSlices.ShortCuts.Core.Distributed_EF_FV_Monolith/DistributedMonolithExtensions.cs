using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace VSlices.ShortCuts.Core.AspNetFVEFDistributedMonolith;

public static class DistributedMonolithExtensions
{
    public static IServiceCollection AddDistributedMonolithServicesAndHandlersFrom<T>(this IServiceCollection services)
    {
        services
            .AddReflectionSender()
            .AddReflectionPublisher()
            .AddInMemoryEventQueue()
            .AddBackgroundEventListenerService()
            .AddHandlersFromAssemblyContaining<T>()
            .AddEndpointDefinitionsFromAssemblyContaining<T>()
            .AddValidatorsFromAssemblyContaining<T>();

        return services;
    }
}
