using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.UnitTests;

public class BusinessFailureTests
{
    [Fact]
    public void UserNotAuthenticated_ShouldReturnKindNotAuthenticatedUserAndEmptyErrors_DetailInitAccesors()
    {
        var bus = new BusinessFailure()
        {
            Errors = Array.Empty<string>(),
            Kind = FailureKind.NotAuthenticatedUser
        };
        
        bus.Kind.Should().Be(FailureKind.NotAuthenticatedUser);
        bus.Errors.Should().BeEmpty();
    }

    [Fact]
    public void UserNotAuthenticated_ShouldReturnKindNotAuthenticatedUserAndEmptyErrors()
    {
        var bus = BusinessFailure.Of.UserNotAuthenticated();
        
        bus.Kind.Should().Be(FailureKind.NotAuthenticatedUser);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void UserNotAllowed_ShouldReturnKindNotAllowedUserAndEmptyErrors()
    {
        var bus = BusinessFailure.Of.UserNotAllowed();
        
        bus.Kind.Should().Be(FailureKind.NotAllowedUser);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void NotFoundResource_ShouldReturnKindNotFoundResourceAndErrors()
    {
        var errors = new[] { "error1", "error2" };
        var bus = BusinessFailure.Of.NotFoundResource(errors);
        
        bus.Kind.Should().Be(FailureKind.NotFoundResource);
        bus.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void NotFoundResource_ShouldReturnKindNotFoundResourceAndEmptyErrors()
    {
        var bus = BusinessFailure.Of.NotFoundResource();
        
        bus.Kind.Should().Be(FailureKind.NotFoundResource);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ConcurrencyError_ShouldReturnKindConcurrencyErrorAndErrors()
    {
        var errors = new[] { "error1", "error2" };
        var bus = BusinessFailure.Of.ConcurrencyError(errors);
        
        bus.Kind.Should().Be(FailureKind.ConcurrencyError);
        bus.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void ConcurrencyError_ShouldReturnKindConcurrencyErrorAndEmptyErrors()
    {
        var bus = BusinessFailure.Of.ConcurrencyError();
        
        bus.Kind.Should().Be(FailureKind.ConcurrencyError);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ContractValidation_ShouldReturnKindContractValidationAndErrors()
    {
        var errors = new[] { "error1", "error2" };
        var bus = BusinessFailure.Of.ContractValidation(errors);
        
        bus.Kind.Should().Be(FailureKind.ContractValidation);
        bus.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void ContractValidation_ShouldReturnKindContractValidationAndErrorsWithOneError()
    {
        const string error = "error1";
        var bus = BusinessFailure.Of.ContractValidation(error);
        
        bus.Kind.Should().Be(FailureKind.ContractValidation);
        bus.Errors.Should().BeEquivalentTo(error);
    }
    
    [Fact]
    public void DomainValidation_ShouldReturnKindDomainValidationAndErrors()
    {
        var errors = new[] { "error1", "error2" };
        var bus = BusinessFailure.Of.DomainValidation(errors);
        
        bus.Kind.Should().Be(FailureKind.DomainValidation);
        bus.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void DomainValidation_ShouldReturnKindDomainValidationAndErrorsWithOneError()
    {
        const string error = "error1";
        var bus = BusinessFailure.Of.DomainValidation(error);
        
        bus.Kind.Should().Be(FailureKind.DomainValidation);
        bus.Errors.Should().BeEquivalentTo(error);
    }
    
    [Fact]
    public void DefaultError_ShouldReturnKindDefaultErrorAndErrors()
    {
        var errors = new[] { "error1", "error2" };
        var bus = BusinessFailure.Of.DefaultError(errors);
        
        bus.Kind.Should().Be(FailureKind.DefaultError);
        bus.Errors.Should().BeEquivalentTo(errors);
    }
    
    [Fact]
    public void DefaultError_ShouldReturnKindDefaultErrorAndErrorsWithOneError()
    {
        const string error = "error1";
        var bus = BusinessFailure.Of.DefaultError(error);
        
        bus.Kind.Should().Be(FailureKind.DefaultError);
        bus.Errors.Should().BeEquivalentTo(error);
    }

    [Fact]
    public void DefaultError_ShouldReturnKindDefaultErrorAndEmptyErrors()
    {
        var bus = BusinessFailure.Of.DefaultError();
        
        bus.Kind.Should().Be(FailureKind.DefaultError);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void UnhandledException_ShouldReturnKindUnhandledExceptionAndErrors()
    {
        var bus = BusinessFailure.Of.UnhandledException();
        
        bus.Kind.Should().Be(FailureKind.UnhandledException);
        bus.Errors.Should().BeEmpty();
    }
}
