using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.Presentation;

namespace VSlices.Core.Abstracts.UnitTests.Extensions;

public class WebApplicationExtensionsTests
{
    public class WebAppDummy : IApplicationBuilder, IEndpointRouteBuilder
    {
        public WebAppDummy(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            DataSources = new List<EndpointDataSource>();
        }

        public IApplicationBuilder CreateApplicationBuilder()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ServiceProvider { get; }

        public ICollection<EndpointDataSource> DataSources { get; }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder New()
        {
            throw new NotImplementedException();
        }

        public RequestDelegate Build()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ApplicationServices { get; set; }
        public IFeatureCollection ServerFeatures { get; }
        public IDictionary<string, object?> Properties { get; }
    }

    public class Endpoint : IEndpointDefinition
    {
        public const string ApiRoute = "api/test";

        public void DefineEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(ApiRoute, Test);
        }

        public static Task Test(HttpContext context) => Task.FromResult<IResult>(EmptyHttpResult.Instance);

        public static void DefineDependencies(IServiceCollection services)
        {

        }
    }

    [Fact]
    public void UseEndpointDefinitions_ShouldCallMethods()
    {
        var services = new ServiceCollection();

        services.AddEndpointDefinition<Endpoint>();

        var provider = services.BuildServiceProvider();

        var webAppDummy = new WebAppDummy(provider);

        webAppDummy.UseEndpointDefinitions();

        var dataSources = webAppDummy.DataSources.First();

        var addedEndpoint = (RouteEndpoint)dataSources.Endpoints[0];

        if (addedEndpoint.RequestDelegate is null) throw new ArgumentNullException(nameof(addedEndpoint.RequestDelegate));

        addedEndpoint.RequestDelegate.Method.Should().BeSameAs(typeof(Endpoint).GetMethod(nameof(Endpoint.Test)));
        addedEndpoint.RoutePattern.RawText.Should().Be(Endpoint.ApiRoute);
    }
}
