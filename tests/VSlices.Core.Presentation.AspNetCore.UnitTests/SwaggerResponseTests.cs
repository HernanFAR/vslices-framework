using System.Net;
using System.Net.Mime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace VSlices.Core.Presentation.AspNetCore.UnitTests;

public class SwaggerResponseTests
{
    public record Sample(string Name, int Age);

    [Fact]
    public void Instantiation_ShouldCreateWithCorrectValues_WithStatusCode()
    {
        // Arrange
        const int httpStatusCode = StatusCodes.Status200OK;
        const string description = "OK";


        // Act
        var response = SwaggerResponse.WithStatusCode(httpStatusCode, description);


        // Assert
        response.HttpStatusCode.Should().Be(httpStatusCode);
        response.Description.Should().Be(description);
        response.Type.Should().Be(typeof(void));
        response.ContentTypes.Should().BeNull();


    }

    [Fact]
    public void Instantiation_ShouldCreateWithCorrectValues_WithJsonOf()
    {
        // Arrange
        const int httpStatusCode = StatusCodes.Status200OK;
        const string description = "OK";


        // Act
        var response = SwaggerResponse.WithJson.Of<Sample>(httpStatusCode, description);


        // Assert
        response.HttpStatusCode.Should().Be(httpStatusCode);
        response.Description.Should().Be(description);
        response.Type.Should().Be(typeof(Sample));
        response.ContentTypes.Should().BeEquivalentTo(MediaTypeNames.Application.Json);


    }

    [Fact]
    public void Instantiation_ShouldCreateWithCorrectValues_WithProblemDetails()
    {
        // Arrange
        const int httpStatusCode = StatusCodes.Status400BadRequest;
        const string description = "Bad Request";


        // Act
        var response = SwaggerResponse.WithJson.OfProblemDetails(httpStatusCode, description);


        // Assert
        response.HttpStatusCode.Should().Be(httpStatusCode);
        response.Description.Should().Be(description);
        response.Type.Should().Be(typeof(HttpValidationProblemDetails));
        response.ContentTypes.Should().BeEquivalentTo(MediaTypeNames.Application.Json);


    }
}
