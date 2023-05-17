namespace VSlices.Core.Abstracts.Responses;

public enum FailureKind
{
    UserNotAllowed,
    NotFoundResource,
    ConcurrencyError,
    Validation
}

public readonly record struct BusinessFailure(FailureKind Kind, string[] Errors)
{
    public static class Of
    {
        public static BusinessFailure NotAllowedUser(string[] errors) => new(FailureKind.UserNotAllowed, errors);
        public static BusinessFailure NotAllowedUser() => new(FailureKind.UserNotAllowed, Array.Empty<string>());
        public static BusinessFailure NotFoundResource(string[] errors) => new(FailureKind.NotFoundResource, errors);
        public static BusinessFailure NotFoundResource() => new(FailureKind.NotFoundResource, Array.Empty<string>());
        public static BusinessFailure ConcurrencyError(string[] errors) => new(FailureKind.ConcurrencyError, errors);
        public static BusinessFailure ConcurrencyError() => new(FailureKind.ConcurrencyError, Array.Empty<string>());
        public static BusinessFailure Validation(string[] errors) => new(FailureKind.Validation, errors);
    }
}

