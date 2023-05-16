using Sample.Core.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class EndpointRouteBuilderExtensions
{
    public static void UseEndpointDefinitions(this IEndpointRouteBuilder builder)
    {
        var endpointDefinitions = builder.ServiceProvider
            .GetServices<ISimpleEndpointDefinition>();

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.DefineEndpoint(builder);
        }
    }
}
