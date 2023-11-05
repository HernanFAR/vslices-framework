using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// A response that an endpoint can return
/// </summary>
public readonly struct SwaggerResponse
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

    private SwaggerResponse(int httpStatusCode, string? description, Type type, string[]? contentTypes)
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
    /// <param name="description">Optional description of when the response is returned</param>
    /// <returns>The created response with the given status code and description</returns>
    public static SwaggerResponse WithStatusCode(int httpStatusCode, string? description = null)
        => new(httpStatusCode, description, typeof(void), null);

    /// <summary>
    /// Creates typed responses with a <see cref="MediaTypeNames.Application.Json"/> content type.
    /// </summary>
    public static class WithJson
    {
        /// <summary>
        /// Creates a typed response with a <see cref="MediaTypeNames.Application.Json"/> content type, as well as a status code and description
        /// </summary>
        /// <typeparam name="T">The type of the response</typeparam>
        /// <param name="httpStatusCode">Associated HTTP status code</param>
        /// <param name="description">Optional description of when the response is returned</param>
        /// <returns>The created response with the given status code and description</returns>
        public static SwaggerResponse Of<T>(int httpStatusCode, string? description = null)
            => new(httpStatusCode, description, typeof(T), new[] { MediaTypeNames.Application.Json });

        /// <summary>
        /// Creates a <see cref="HttpValidationProblemDetails"/> response with a <see cref="MediaTypeNames.Application.Json"/> content type, as well as a status code and description
        /// </summary>
        /// <param name="httpStatusCode">Associated HTTP status code</param>
        /// <param name="description">Optional description of when the response is returned</param>
        /// <returns>The created response with the given status code and description</returns>
        public static SwaggerResponse OfProblemDetails(int httpStatusCode, string? description = null)
            => new(httpStatusCode, description, typeof(HttpValidationProblemDetails), new[] { MediaTypeNames.Application.Json });
    }
}
