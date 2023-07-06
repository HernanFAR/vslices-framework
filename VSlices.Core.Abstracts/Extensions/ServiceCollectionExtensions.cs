using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Sender;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSender<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : ISender
    {
        services.Add(new ServiceDescriptor(typeof(ISender), typeof(T), lifetime));

        return services;
    }

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
