using Application.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class EndpointRouteBuilderExtensions
{
    public static void UseEndpointDefinition(this IEndpointRouteBuilder builder)
    {
        var endpointDefinitions = builder.ServiceProvider
            .GetServices<IEndpointDefinition>();
        
        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.DefineEndpoint(builder);
        }
    }
}
