using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.CrossCutting.Validation.FluentValidation.UnitTests;

public class ValidationExtensionTests
{
    [Fact]
    public void AddFluentValidationBehavior_ShouldAddFluentValidationBehavior()
    {
        // Arrange
        var services = new ServiceCollection();


        // Act
        services.AddFluentValidationBehavior();


        // Assert
        services.Should().Contain(e => 
            e.ServiceType == typeof(IPipelineBehavior<,>) &&
            e.ImplementationType == typeof(FluentValidationBehavior<,>));


    }
}
