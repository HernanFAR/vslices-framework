using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// Base class for Swagger documentation.
/// </summary>
public abstract class SwaggerDocumentation
{
    /// <summary>
    /// The name of the endpoint
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Tags associated with the endpoint
    /// </summary>
    public abstract string[] Tags { get; }

    /// <summary>
    /// A short summary of what the endpoint does
    /// </summary>
    public abstract string Summary { get; }

    /// <summary>
    /// A longer description of what the endpoint does
    /// </summary>
    public virtual string Description => string.Empty;

    /// <summary>
    /// The main content type that the endpoint consumes
    /// </summary>
    /// <remarks>Usually <see cref="MediaTypeNames.Application.Json"/> if the endpoint is not a <see cref="HttpMethod.Get"/></remarks>
    public virtual string? MainConsumingContentType => null;

    /// <summary>
    /// Other content types that the endpoint consumes
    /// </summary>
    public virtual string[] OtherConsumingContentTypes => Array.Empty<string>();

    /// <summary>
    /// The responses that the endpoint can return
    /// </summary>
    public abstract SwaggerResponse[] Responses { get; }

    /// <summary>
    /// Defines the Swagger documentation for the endpoint
    /// </summary>
    /// <param name="routeBuilder"></param>
    internal void DefineSwaggerDocumentation(RouteHandlerBuilder routeBuilder)
    {
        var builder = routeBuilder
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description);

        if (MainConsumingContentType is not null)
        {
            builder.WithMetadata(new ConsumesAttribute(MainConsumingContentType, OtherConsumingContentTypes));
        }

        foreach (var producesInfo in Responses)
        {
            builder.WithMetadata(new SwaggerResponseAttribute(
                producesInfo.HttpStatusCode, producesInfo.Description, producesInfo.Type, producesInfo.ContentTypes));
        }
    }

    /// <summary>
    /// Base class for Swagger documentation, with a singleton property
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance. Must be a subclass of <see cref="SwaggerDocumentation"/></typeparam>
    public abstract class WithSingleton<T> : SwaggerDocumentation
        where T : SwaggerDocumentation, new()
    {
        /// <summary>
        /// The singleton instance of the Swagger documentation
        /// </summary>
        public static readonly T Instance = new();
    }
}
