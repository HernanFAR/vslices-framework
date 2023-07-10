using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Sender;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="ISender"/> implementation to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <typeparam name="T">Implementation of the <see cref="ISender"/></typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">The lifetime of the <see cref="ISender"/> implemented</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSender<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : ISender
    {
        services.Add(new ServiceDescriptor(typeof(ISender), typeof(T), lifetime));

        return services;
    }

    /// <summary>
    /// Adds a open <see cref="IPipelineBehavior{TRequest,TResponse}"/> to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="type">The implementation type of the open behavior</param>
    /// <param name="lifetime">The lifetime of the <see cref="ISender"/> implemented</param>
    /// <returns>Service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddPipelineBehavior(this IServiceCollection services,
        Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var implementsPipelineBehavior = type.GetInterfaces()
            .Where(x => x.IsGenericType)
            .Any(x => x.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        if (!implementsPipelineBehavior)
        {
            throw new InvalidOperationException(
                $"{type.FullName} does not implement {typeof(IPipelineBehavior<,>).FullName}");
        }

        services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), type, lifetime));

        return services;
    }

    /// <summary>
    /// Adds the dependencies defined in the <see cref="IUseCaseDependencyDefinition"/> implementations
    /// </summary>
    /// <typeparam name="TAnchor"></typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddCoreDependenciesFrom<TAnchor>(this IServiceCollection services)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => typeof(IUseCaseDependencyDefinition).IsAssignableFrom(e))
            .Where(e => e is { IsAbstract: false, IsInterface: false });

        foreach (var definerType in definerTypes)
        {
            var defineDependenciesMethod = definerType.GetMethod(nameof(IUseCaseDependencyDefinition.DefineDependencies));

            if (defineDependenciesMethod is null)
            {
                throw new InvalidOperationException($"{definerType.FullName} does not implement {nameof(IUseCaseDependencyDefinition)}");
            }

            defineDependenciesMethod.Invoke(null, new object?[] { services });
        }

        return services;
    }
}
