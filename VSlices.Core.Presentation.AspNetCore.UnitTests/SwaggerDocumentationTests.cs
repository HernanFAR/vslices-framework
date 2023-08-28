using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace VSlices.Core.Presentation.AspNetCore.UnitTests;

public class SingletonSwaggerDocumentation : SwaggerDocumentation.WithSingleton<SingletonSwaggerDocumentation>
{
    public override string Name => nameof(Name);
    public override string[] Tags => new[] { nameof(Tags) };
    public override string Summary => nameof(Summary);
    public override Response[] Responses => new[]
    {
        Response.WithStatusCode(StatusCodes.Status200OK, nameof(StatusCodes.Status200OK))
    };
}

public class SwaggerDocumentationTests
{
    [Fact]
    public void Instantiation_ShouldHaveInstanceProperty()
    {
        // Act
        var swaggerDocumentation1 = SingletonSwaggerDocumentation.Instance;
        var swaggerDocumentation2 = SingletonSwaggerDocumentation.Instance;


        // Assert
        swaggerDocumentation1.Should().BeSameAs(swaggerDocumentation2);


    }
}
