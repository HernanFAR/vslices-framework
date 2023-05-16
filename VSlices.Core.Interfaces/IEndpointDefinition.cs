using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace VSlices.Core.Interfaces;

public interface IEndpointDefinition
{
    void DefineEndpoint(IEndpointRouteBuilder builder);

    static abstract void DefineDependencies(IServiceCollection services);
    
}