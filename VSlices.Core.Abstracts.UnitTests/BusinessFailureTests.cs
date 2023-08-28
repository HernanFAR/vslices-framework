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
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName = "ErrorName";
        const string expErrorDetail = "ErrorDetail";
        var expErrors = new[] { new ValidationError(expErrorName, expErrorDetail) };

        var bus = new BusinessFailure
        {
            Kind = FailureKind.NotAuthenticatedUser,
            Title = expTitle,
            Detail = expDetail,
            Errors = expErrors
        };
        
        bus.Kind.Should().Be(FailureKind.NotAuthenticatedUser);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEquivalentTo(expErrors);
    }

    [Fact]
    public void UserNotAuthenticated_ShouldReturnKindNotAuthenticated()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.UserNotAuthenticated(title: expTitle, detail: expDetail);
        
        bus.Kind.Should().Be(FailureKind.NotAuthenticatedUser);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void UserNotAllowed_ShouldReturnKindNotAllowedUser()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.UserNotAllowed(title: expTitle, detail: expDetail);

        bus.Kind.Should().Be(FailureKind.NotAllowedUser);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void NotFoundResource_ShouldReturnKindNotFoundResource()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.NotFoundResource(title: expTitle, detail: expDetail);
        
        bus.Kind.Should().Be(FailureKind.NotFoundResource);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ConcurrencyError_ShouldReturnKindConcurrencyError()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.ConcurrencyError(title: expTitle, detail: expDetail);

        bus.Kind.Should().Be(FailureKind.ConcurrencyError);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ContractValidation_ShouldReturnKindContractValidationAndErrors()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName = "ErrorName";
        const string expErrorDetail = "ErrorDetail";
        var expErrors = new[] { new ValidationError(expErrorName, expErrorDetail) };
        
        var bus = BusinessFailure.Of.ContractValidation(title: expTitle, detail: expDetail, expErrors);
        
        bus.Kind.Should().Be(FailureKind.ContractValidation);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEquivalentTo(expErrors);
    }
    
    [Fact]
    public void ContractValidation_ShouldReturnKindContractValidationAndError()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName = "ErrorName";
        const string expErrorDetail = "ErrorDetail";
        var expError = new ValidationError(expErrorName, expErrorDetail);

        var bus = BusinessFailure.Of.ContractValidation(title: expTitle, detail: expDetail, expError);

        bus.Kind.Should().Be(FailureKind.ContractValidation);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEquivalentTo(new[] { expError });
    }
    
    [Fact]
    public void DomainValidation_ShouldReturnKindDomainValidationAndErrors()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName = "ErrorName";
        const string expErrorDetail = "ErrorDetail";
        var expErrors = new[] { new ValidationError(expErrorName, expErrorDetail) };

        var bus = BusinessFailure.Of.DomainValidation(title: expTitle, detail: expDetail, expErrors);

        bus.Kind.Should().Be(FailureKind.DomainValidation);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEquivalentTo(expErrors);
    }
    
    [Fact]
    public void DomainValidation_ShouldReturnKindDomainValidationAndError()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName = "ErrorName";
        const string expErrorDetail = "ErrorDetail";
        var expError = new ValidationError(expErrorName, expErrorDetail);

        var bus = BusinessFailure.Of.DomainValidation(title: expTitle, detail: expDetail, expError);

        bus.Kind.Should().Be(FailureKind.DomainValidation);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEquivalentTo(new[] { expError });
    }
    
    [Fact]
    public void DefaultError_ShouldReturnKindUnspecified()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.Unspecified(title: expTitle, detail: expDetail);
        
        bus.Kind.Should().Be(FailureKind.Unspecified);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void UnhandledException_ShouldReturnKindUnhandledExceptionAndErrors()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        var bus = BusinessFailure.Of.UnhandledException(title: expTitle, detail: expDetail);

        bus.Kind.Should().Be(FailureKind.UnhandledException);
        bus.Title.Should().Be(expTitle);
        bus.Detail.Should().Be(expDetail);
        bus.Errors.Should().BeEmpty();
    }
}
