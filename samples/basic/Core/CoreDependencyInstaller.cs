using Core;
using VSlices.ShortCuts.Core.AspNetFVEFDistributedMonolith;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreDependencyInstaller
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        => services.AddDistributedMonolithServicesAndHandlersFrom<Anchor>();
}
