using Core;
using FluentValidation;
using VSlices.Core.Sender.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreDependencyInstaller
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        services.AddSender<ReflectionSender>()
            .AddHandlersFromAssemblyContaining<Anchor>()
            .AddEndpointDefinitionsFromAssemblyContaining<Anchor>()
            .AddValidatorsFromAssembly(typeof(Anchor).Assembly);

        return services;
    }
}
