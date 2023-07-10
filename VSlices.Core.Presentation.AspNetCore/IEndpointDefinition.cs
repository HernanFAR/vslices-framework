﻿using Microsoft.AspNetCore.Routing;
using VSlices.Core.Abstracts.Presentation;

namespace VSlices.Core.Presentation.AspNetCore;

/// <summary>
/// Defines a endpoint of a use case without dependencies, in the application.
/// </summary>
public interface ISimpleEndpointDefinition
{
    /// <summary>
    /// Defines the endpoint of the use case.
    /// </summary>
    /// <param name="builder">Endpoint route builder</param>
    void DefineEndpoint(IEndpointRouteBuilder builder);

}


/// <summary>
/// Defines an endpoint of a use case with dependencies, in the application.
/// </summary>
public interface IEndpointDefinition : ISimpleEndpointDefinition, IUseCaseDependencyDefinition
{

}