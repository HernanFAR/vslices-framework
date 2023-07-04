using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace VSlices.Core.Presentation.AspNetCore;

public interface ISimpleEndpointDefinition
{
    void DefineEndpoint(IEndpointRouteBuilder builder);

}

public interface IEndpointDefinition : ISimpleEndpointDefinition
{
    static abstract void DefineDependencies(IServiceCollection services);

}