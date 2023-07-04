using VSlices.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointDefinition<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : IEndpointDefinition
    {
        services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), typeof(T), lifetime));
        T.DefineDependencies(services);

        return services;
    }
}
