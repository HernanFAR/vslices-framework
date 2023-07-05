namespace VSlices.Core.Abstracts.Responses;

public enum FailureKind
{
    NotAuthenticatedUser,
    NotAllowedUser,
    NotFoundResource,
    ConcurrencyError,
    ContractValidation,
    DomainValidation,
    DefaultError,
    UnhandledException
}

public readonly record struct BusinessFailure(FailureKind Kind, string[] Errors)
{
    public static class Of
    {
        public static BusinessFailure UserNotAuthenticated() => new(FailureKind.NotAuthenticatedUser, Array.Empty<string>());
        public static BusinessFailure UserNotAllowed() => new(FailureKind.NotAllowedUser, Array.Empty<string>());
        public static BusinessFailure NotFoundResource(string[] errors) => new(FailureKind.NotFoundResource, errors);
        public static BusinessFailure NotFoundResource() => new(FailureKind.NotFoundResource, Array.Empty<string>());
        public static BusinessFailure ConcurrencyError(string[] errors) => new(FailureKind.ConcurrencyError, errors);
        public static BusinessFailure ConcurrencyError() => new(FailureKind.ConcurrencyError, Array.Empty<string>());
        public static BusinessFailure ContractValidation(string[] errors) => new(FailureKind.ContractValidation, errors);
        public static BusinessFailure ContractValidation(string error) => new(FailureKind.ContractValidation, new[] { error });
        public static BusinessFailure DomainValidation(string[] errors) => new(FailureKind.DomainValidation, errors);
        public static BusinessFailure DomainValidation(string error) => new(FailureKind.DomainValidation, new[] { error });
        public static BusinessFailure DefaultError(string[] errors) => new(FailureKind.DefaultError, errors);
        public static BusinessFailure DefaultError(string error) => new(FailureKind.DefaultError, new[] { error });
        public static BusinessFailure DefaultError() => new(FailureKind.DefaultError, Array.Empty<string>());
        public static BusinessFailure UnhandledException() => new(FailureKind.UnhandledException, Array.Empty<string>());

    }
}

