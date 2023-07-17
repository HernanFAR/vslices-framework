using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// A response that an endpoint can return
/// </summary>
public readonly struct Response
{
    /// <summary>
    /// The associated HTTP status code
    /// </summary>
    public int HttpStatusCode { get; }

    /// <summary>
    /// A description of when the response is returned
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// The associated type to the response
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The content types that the response can be
    /// </summary>
    public string[]? ContentTypes { get; }

    private Response(int httpStatusCode, string? description, Type type, string[]? contentTypes)
    {
        HttpStatusCode = httpStatusCode;
        Description = description;
        Type = type;
        ContentTypes = contentTypes;
    }

    /// <summary>
    /// Creates a response without a type and content type, only a status code and description
    /// </summary>
    /// <param name="httpStatusCode">Associated HTTP status code</param>
    /// <param name="description">Description of when the response is returned</param>
    /// <returns>The created response with the given status code and description</returns>
    public static Response WithStatusCode(int httpStatusCode, string? description)
        => new(httpStatusCode, description, typeof(void), null);

    /// <summary>
    /// Creates a typed response with a <see cref="MediaTypeNames.Application.Json"/> content type, as well as a status code and description
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="httpStatusCode">Associated HTTP status code</param>
    /// <param name="description">Description of when the response is returned</param>
    /// <returns>The created response with the given status code and description</returns>
    public static Response WithJsonOf<T>(int httpStatusCode, string? description)
        => new(httpStatusCode, description, typeof(T), new[] { MediaTypeNames.Application.Json });
}

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
    public abstract Response[] Responses { get; }

    /// <summary>
    /// Defines the Swagger documentation for the endpoint
    /// </summary>
    /// <param name="routeBuilder"></param>
    public void DefineSwaggerDocumentation(RouteHandlerBuilder routeBuilder)
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
}
