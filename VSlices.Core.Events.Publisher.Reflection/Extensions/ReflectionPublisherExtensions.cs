using VSlices.Core.Abstracts.Event;
using VSlices.Core.Events.Publisher.Reflection;
using VSlices.Core.Events.Publisher.Reflection.Strategies;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class ReflectionPublisherExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Add a reflection <see cref="IPublisher"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>Default strategy is <see cref="AwaitInParallelStrategy"/></remarks>
    /// <param name="services">Service Collection</param>
    /// <param name="strategy">Strategy</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddReflectionPublisher(this IServiceCollection services, IPublishingStrategy? strategy = null)
    {
        strategy ??= new AwaitInParallelStrategy();

        services.AddPublisher<ReflectionPublisher>();
        services.AddSingleton(strategy);

        return services;
    }
}
