using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace VSlices.Core.Presentation.AspNetCore.UnitTests;

public class AspNetCoreEndpointDefinitionTests
{
    public class RouteBuilder : IEndpointRouteBuilder
    {
        public RouteBuilder()
        {
            DataSources = new Collection<EndpointDataSource>();
        }

        public IApplicationBuilder CreateApplicationBuilder()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ServiceProvider => default!;
        public ICollection<EndpointDataSource> DataSources { get; }
    }

    public class EndpointSwaggerDocumentation : SwaggerDocumentation
    {
        public override string Name => "TestName";
        public override string[] Tags => new[] { "TestTag" };
        public override string Summary => "TestSummary";
        public override string MainConsumingContentType => MediaTypeNames.Application.Json;
        public override Response[] Responses => new[]
        {
            Response.WithStatusCode(StatusCodes.Status200OK, "TestResponse"),
            Response.WithJsonOf<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity, "TestResponse")
        };
    }

    public class Endpoint : AspNetCoreEndpointDefinition
    {
        public override Delegate DelegateHandler => Handler;
        public override string Route => "/api/test";
        public override HttpMethod HttpMethod => HttpMethod.Get;
        public static IResult Handler() => TypedResults.Ok();

    }

    public class Endpoint1 : AspNetCoreEndpointDefinition
    {
        public override Delegate DelegateHandler => Handler;
        public override string Route => "/api/test";
        public override HttpMethod HttpMethod => HttpMethod.Get;
        public override SwaggerDocumentation? SwaggerConfiguration => new EndpointSwaggerDocumentation();

        public static IResult Handler() => TypedResults.Ok();

    }

    [Fact]
    public void DefineEndpoint_ShouldAddCorrectEndpointMetadata()
    {
        // Arrange
        var builder = new RouteBuilder();
        var endpoint = new Endpoint();


        // Act
        endpoint.DefineEndpoint(builder);


        // Assert
        var endpointSource = builder.DataSources.First();

        var addedEndpoint = (RouteEndpoint)endpointSource.Endpoints[0];

        addedEndpoint.Metadata.First().Should().BeSameAs(endpoint.DelegateHandler.Method);
        addedEndpoint.Metadata
            .OfType<HttpMethodMetadata>()
            .First()
            .HttpMethods[0]
            .Should().BeSameAs(endpoint.HttpMethod.Method);
        addedEndpoint.RoutePattern.RawText.Should().Be(endpoint.Route);
    }

    [Fact]
    public void DefineEndpoint_ShouldAddCorrectEndpointMetadata_DetailWithSwaggerMetadata()
    {
        // Arrange
        var builder = new RouteBuilder();
        var endpoint = new Endpoint1();


        // Act
        endpoint.DefineEndpoint(builder);


        // Assert
        var endpointSource = builder.DataSources.First();

        var addedEndpoint = (RouteEndpoint)endpointSource.Endpoints[0];

        addedEndpoint.Metadata.First().Should().BeSameAs(endpoint.DelegateHandler.Method);
        addedEndpoint.Metadata
            .OfType<HttpMethodMetadata>()
            .First()
            .HttpMethods[0]
            .Should().BeSameAs(endpoint.HttpMethod.Method);
        addedEndpoint.Metadata
            .OfType<EndpointNameMetadata>()
            .First()
            .EndpointName
            .Should().BeSameAs(endpoint.SwaggerConfiguration.Name);
        addedEndpoint.Metadata
            .OfType<TagsAttribute>()
            .First()
            .Tags
            .Should().BeEquivalentTo(endpoint.SwaggerConfiguration.Tags);
        addedEndpoint.Metadata
            .OfType<EndpointSummaryAttribute>()
            .First()
            .Summary
            .Should().BeSameAs(endpoint.SwaggerConfiguration.Summary);
        addedEndpoint.Metadata
            .OfType<ConsumesAttribute>()
            .First()
            .ContentTypes
            .Should().ContainSingle(endpoint.SwaggerConfiguration.MainConsumingContentType);
        var responses = addedEndpoint.Metadata
            .OfType<SwaggerResponseAttribute>();
        
        responses.First()
            .StatusCode.Should().Be(endpoint.SwaggerConfiguration.Responses[0].HttpStatusCode);
        responses.First()
            .Description.Should().Be(endpoint.SwaggerConfiguration.Responses[0].Description);
        responses.First()
            .Type.Should().Be(endpoint.SwaggerConfiguration.Responses[0].Type);
        responses.First()
            .ContentTypes.Should().BeSameAs(endpoint.SwaggerConfiguration.Responses[0].ContentTypes);
        
        responses.Skip(1).First()
            .StatusCode.Should().Be(endpoint.SwaggerConfiguration.Responses[1].HttpStatusCode);
        responses.Skip(1).First()
            .Description.Should().Be(endpoint.SwaggerConfiguration.Responses[1].Description);
        responses.Skip(1).First()
            .Type.Should().Be(endpoint.SwaggerConfiguration.Responses[1].Type);
        responses.Skip(1).First()
            .ContentTypes.Should().BeEquivalentTo(endpoint.SwaggerConfiguration.Responses[1].ContentTypes);

        addedEndpoint.RoutePattern.RawText.Should().Be(endpoint.Route);
    }
}
