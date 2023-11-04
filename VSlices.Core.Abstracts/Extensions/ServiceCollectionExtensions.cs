using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Configurations;
using VSlices.Core.Abstracts.Event;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Sender;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class ServiceCollectionExtensions
#pragma warning restore CS1591
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
    /// Adds a <see cref="IPublisher"/> implementation to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <typeparam name="T">Implementation of the <see cref="IPublisher"/></typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">The lifetime of the <see cref="IPublisher"/> implemented</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddPublisher<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : IPublisher
    {
        services.Add(new ServiceDescriptor(typeof(IPublisher), typeof(T), lifetime));

        return services;
    }

    /// <summary>
    /// Adds a <see cref="IEventQueue"/> implementation to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <remarks>
    /// It also add it as <see cref="IEventQueueWriter"/> and <see cref="IEventQueueReader"/> implementation
    /// </remarks>
    /// <typeparam name="T">Implementation of the <see cref="IEventQueue"/></typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">The lifetime of the <see cref="IEventQueue"/> implemented</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddEventQueue<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where T : IEventQueue
    {
        services.Add(new ServiceDescriptor(typeof(IEventQueue), typeof(T), lifetime));
        services.AddSingleton<IEventQueueWriter>(s => s.GetRequiredService<IEventQueue>());
        services.AddSingleton<IEventQueueReader>(s => s.GetRequiredService<IEventQueue>());

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
    public static IServiceCollection AddCoreDependenciesFromAssemblyContaining<TAnchor>(this IServiceCollection services)
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

    /// <summary>
    /// Adds <see cref="IHandler{TRequest,TResponse}"/> implementations from the specified assembly of the <typeparamref name="TAnchor"/> type, to the service collection.
    /// </summary>
    /// <typeparam name="TAnchor">Anchor type to search</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddHandlersFromAssemblyContaining<TAnchor>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => e.GetInterfaces().Where(o => o.IsGenericType).Any(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>)))
            .Where(e => e is { IsAbstract: false, IsInterface: false })
            .Select(e => (e, e.GetInterfaces()
                .Where(o => o.IsGenericType)
                .Single(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>))));

        foreach (var (handlerType, handlerInterface) in definerTypes)
        {
            services.Add(new ServiceDescriptor(handlerInterface, handlerType, lifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds a hosted service that will listen for events in the background
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="configAct">Action to configure the service</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddBackgroundEventListenerService(this IServiceCollection services, Action<BackgroundEventListenerConfiguration>? configAct = null)
    {
        services.AddHostedService<BackgroundEventListenerService>();

        var config = new BackgroundEventListenerConfiguration();
        configAct?.Invoke(config);

        services.AddSingleton(config);

        return services;
    }
}
