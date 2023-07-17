using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using VSlices.Core.Abstracts.Presentation;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// Defines a endpoint of a use case without dependencies
/// </summary>
public interface ISimpleEndpointDefinition
{
    /// <summary>
    /// Defines the endpoint of the use case.
    /// </summary>
    /// <param name="builder">Endpoint route builder</param>
    void DefineEndpoint(IEndpointRouteBuilder builder);

}


/// <summary>
/// Defines an endpoint of a use case with dependencies
/// </summary>
public interface IEndpointDefinition : ISimpleEndpointDefinition, IUseCaseDependencyDefinition
{

}

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
    public virtual void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        var routeBuilder = builder.MapMethods(Route, new[] { HttpMethod.ToString() }, DelegateHandler);

        SwaggerConfiguration?.DefineSwaggerDocumentation(routeBuilder);
    }
}
