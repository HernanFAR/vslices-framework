using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// Defines a endpoint of a use case without dependencies
/// </summary>
/// <remarks>If you need to specify dependencies, implement <see cref="IEndpointDefinition"/> too</remarks>
public abstract class AspNetCoreEndpointDefinition : ISimpleEndpointDefinition
{
    /// <summary>
    /// The delegate executed when the endpoint is matched
    /// </summary>
    public abstract Delegate DelegateHandler { get; }

    /// <summary>
    /// The route pattern
    /// </summary>
    public abstract string Route { get; }

    /// <summary>
    /// HTTP method that the endpoint will match
    /// </summary>
    public abstract HttpMethod HttpMethod { get; }

    /// <summary>
    /// A swagger documentation definition for the endpoint
    /// </summary>
    public virtual SwaggerDocumentation? SwaggerConfiguration => null;

    /// <inheritdoc />
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        var routeBuilder = builder.MapMethods(Route, new[] { HttpMethod.ToString() }, DelegateHandler);

        BuildRoute(routeBuilder);
    }

    /// <summary>
    /// Builds the route with the given route builder
    /// </summary>
    /// <remarks>By defaults, it adds the swagger configuration.</remarks>
    /// <param name="routeBuilder">RouteHandlerBuilder</param>
    protected virtual void BuildRoute(RouteHandlerBuilder routeBuilder)
    {
        SwaggerConfiguration?.DefineSwaggerDocumentation(routeBuilder);
    }
}
