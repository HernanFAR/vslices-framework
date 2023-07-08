using VSlices.CrossCutting.Logging;
using VSlices.CrossCutting.Logging.Configurations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Action<LoggingConfiguration>? configAction = null)
    {
        return services.AddLoggingBehavior(typeof(LoggingBehavior<,>), configAction);
    }

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
