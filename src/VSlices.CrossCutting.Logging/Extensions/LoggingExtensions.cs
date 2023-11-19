﻿using VSlices.CrossCutting.Logging;
using VSlices.CrossCutting.Logging.Configurations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class LoggingExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Add the default logging behavior to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configAction">Setups the <see cref="LoggingConfiguration"/></param>
    /// <param name="lifetime">Service lifetime</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Action<LoggingConfiguration>? configAction = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddLoggingBehavior(typeof(LoggingBehavior<,>), configAction, lifetime);
    }

    /// <summary>
    /// Add a custom logging behavior to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="loggingBehaviorType">The specific logging behavior to add</param>
    /// <param name="configAction">Setups the <see cref="LoggingConfiguration"/></param>
    /// <param name="lifetime">Service lifetime</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Type loggingBehaviorType,
        Action<LoggingConfiguration>? configAction = null, 
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var configuration = new LoggingConfiguration();

        configAction?.Invoke(configuration);

        services.AddSingleton(configuration);
        services.AddPipelineBehavior(loggingBehaviorType, lifetime);

        return services;
    }
}
