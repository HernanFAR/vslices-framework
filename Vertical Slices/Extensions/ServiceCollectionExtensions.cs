using Application.Interfaces;

namespace Vertical_Slices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointDefinitionsFrom<T>(this IServiceCollection services)
    {
        var installers = typeof(T).Assembly
            .ExportedTypes
            .Where(e => typeof(IEndpointDefinition).IsAssignableFrom(e))
            .Where(e => e is { IsAbstract: false, IsInterface: false })
            .ToList();

        foreach (var installer in installers)
        {
            services.AddSingleton(typeof(IEndpointDefinition), installer);
        }

        return services;
    }
}
