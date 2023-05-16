using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core.Interfaces;

public interface ISimpleEndpointDefinition
{
    void DefineEndpoint(IEndpointRouteBuilder builder);

}

public interface IEndpointDefinition : ISimpleEndpointDefinition
{
    static abstract void DefineDependencies(IServiceCollection services);

}