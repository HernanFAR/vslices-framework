using VSlices.CrossCutting.Validation.FluentValidation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ValidationExtensions
{
    /// <summary>
    /// Adds the FluentValidationBehavior to the pipeline.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Service lifetime</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddFluentValidationBehavior(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.AddPipelineBehavior(typeof(FluentValidationBehavior<,>), lifetime);
        return services;
    }
}
