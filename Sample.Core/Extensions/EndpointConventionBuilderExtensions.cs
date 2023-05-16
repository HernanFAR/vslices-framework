using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithSwaggerOperationInfo(this RouteHandlerBuilder builder,
        string summary, string description)
    {
        var operation = new SwaggerOperationAttribute(summary: summary, description: description);

        return builder.WithMetadata(operation);
    }
}
