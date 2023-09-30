using Core;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreDependencyInstaller
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        => services.AddDistributedMonolithServicesAndHandlersFromAssemblyContaining<Anchor>();
}
