using Microsoft.AspNetCore.Routing;
using VSlices.Core.Abstracts.Presentation;

namespace VSlices.Core.Presentation.AspNetCore;

public interface ISimpleEndpointDefinition
{
    void DefineEndpoint(IEndpointRouteBuilder builder);

}

public interface IEndpointDefinition : IUseCaseDependencyInjector
{

}