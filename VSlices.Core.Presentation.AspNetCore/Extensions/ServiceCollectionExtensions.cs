using VSlices.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IEndpointDefinition"/> as <see cref="ISimpleEndpointDefinition"/> to the service collection.
    /// </summary>
    /// <typeparam name="T">The endpoint definition to be added</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime of the ISimpleEndpointDefinition</param>
    /// <returns></returns>
    public static IServiceCollection AddEndpointDefinition<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : IEndpointDefinition
    {
        services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), typeof(T), lifetime));
        T.DefineDependencies(services);

        return services;
    }
}
