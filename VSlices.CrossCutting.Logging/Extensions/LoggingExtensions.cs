using VSlices.CrossCutting.Logging;
using VSlices.CrossCutting.Logging.Configurations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingExtensions
{
    /// <summary>
    /// Add the default logging behavior to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configAction">Setups the <see cref="LoggingConfiguration"/></param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Action<LoggingConfiguration>? configAction = null)
    {
        return services.AddLoggingBehavior(typeof(LoggingBehavior<,>), configAction);
    }

    /// <summary>
    /// Add a custom logging behavior to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="loggingBehaviorType">The specific logging behavior to add</param>
    /// <param name="configAction">Setups the <see cref="LoggingConfiguration"/></param>
    /// <returns>Service collection</returns>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Type loggingBehaviorType,
        Action<LoggingConfiguration>? configAction = null)
    {
        var configuration = new LoggingConfiguration();

        configAction?.Invoke(configuration);

        services.AddSingleton(configuration);
        services.AddPipelineBehavior(loggingBehaviorType);

        return services;
    }
}
