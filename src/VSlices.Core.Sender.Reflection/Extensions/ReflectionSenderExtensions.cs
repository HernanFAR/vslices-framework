﻿using VSlices.Core.Abstracts.Sender;
using VSlices.Core.Sender.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class ReflectionSenderExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Add a reflection <see cref="ISender"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddReflectionSender(this IServiceCollection services)
    {
        services.AddSender<ReflectionSender>();

        return services;
    }
}
