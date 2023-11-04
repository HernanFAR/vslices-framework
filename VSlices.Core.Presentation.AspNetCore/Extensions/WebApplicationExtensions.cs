using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

#pragma warning disable CS1591
public static class WebApplicationExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Uses the endpoint definitions to define the endpoints of the application.
    /// </summary>
    /// <param name="app">Endpoint route builder</param>
    public static void UseEndpointDefinitions(this IEndpointRouteBuilder app)
    {
        using var services = app.ServiceProvider.CreateScope();

        var endpoints = services.ServiceProvider.GetServices<ISimpleEndpointDefinition>();

        foreach (var endpoint in endpoints)
        {
            endpoint.DefineEndpoint(app);
        }
    }
}
