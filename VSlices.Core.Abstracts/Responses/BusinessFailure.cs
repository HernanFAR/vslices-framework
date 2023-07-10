namespace VSlices.Core.Abstracts.Responses;

/// <summary>
/// Specifies different reasons for a business failure
/// </summary>
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

/// <summary>
/// Represents a failure in the given business logic
/// </summary>
/// <param name="Kind">Reason of the failure</param>
/// <param name="Errors">Error messages related to the failure</param>
public readonly record struct BusinessFailure(FailureKind Kind, string[] Errors)
{
    public static class Of
    {
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/>
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/> and no error messages</returns>
        public static BusinessFailure UserNotAuthenticated() => new(FailureKind.NotAuthenticatedUser, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/>
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/> and no error messages</returns>
        public static BusinessFailure UserNotAllowed() => new(FailureKind.NotAllowedUser, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/> and given error messages
        /// </summary>
        /// <param name="errors">The error messages related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/> and error messages</returns>
        public static BusinessFailure NotFoundResource(string[] errors) => new(FailureKind.NotFoundResource, errors);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/> and no error messages
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/> and no error messages</returns>
        public static BusinessFailure NotFoundResource() => new(FailureKind.NotFoundResource, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/> and given error messages
        /// </summary>
        /// <param name="errors">The error messages related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/> and no error messages</returns>
        public static BusinessFailure ConcurrencyError(string[] errors) => new(FailureKind.ConcurrencyError, errors);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/> and no error messages
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/> and no error messages</returns>
        public static BusinessFailure ConcurrencyError() => new(FailureKind.ConcurrencyError, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/> and given error messages
        /// </summary>
        /// <param name="errors">The error messages related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/> and given error messages</returns>
        public static BusinessFailure ContractValidation(string[] errors) => new(FailureKind.ContractValidation, errors);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/> and one error message
        /// </summary>
        /// <param name="error">The error message related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/> and one error message</returns>
        public static BusinessFailure ContractValidation(string error) => new(FailureKind.ContractValidation, new[] { error });

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/> and given error messages
        /// </summary>
        /// <param name="errors">The error messages related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/> and given error messages</returns>
        public static BusinessFailure DomainValidation(string[] errors) => new(FailureKind.DomainValidation, errors);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/> and one error message
        /// </summary>
        /// <param name="error">The error message related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/> and one error message</returns>
        public static BusinessFailure DomainValidation(string error) => new(FailureKind.DomainValidation, new[] { error });

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and given error messages
        /// </summary>
        /// <param name="errors">The error messages related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and given error messages</returns>
        public static BusinessFailure DefaultError(string[] errors) => new(FailureKind.DefaultError, errors);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and one error message
        /// </summary>
        /// <param name="error">The error message related to the failure</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and one error message</returns>
        public static BusinessFailure DefaultError(string error) => new(FailureKind.DefaultError, new[] { error });

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and no error messages
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DefaultError"/> and no error messages</returns>
        public static BusinessFailure DefaultError() => new(FailureKind.DefaultError, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.UnhandledException"/> and no error messages
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.UnhandledException"/> and no error messages</returns>
        public static BusinessFailure UnhandledException() => new(FailureKind.UnhandledException, Array.Empty<string>());

    }
}

