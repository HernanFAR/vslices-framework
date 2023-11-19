using VSlices.Core.Abstracts.Events;
using VSlices.Core.Events.EventQueue.InMemory;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class InMemoryEventQueueExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Add a in memory <see cref="IEventQueue"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="configAction">Configuration for the implementation</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddInMemoryEventQueue(this IServiceCollection services, 
        Action<InMemoryEventQueueConfiguration>? configAction = null)
    {
        var configuration = new InMemoryEventQueueConfiguration();

        configAction?.Invoke(configuration);

        services.AddEventQueue<InMemoryEventQueue>();
        services.AddSingleton(configuration);

        return services;
    }
}
