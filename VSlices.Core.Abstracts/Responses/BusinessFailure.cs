namespace VSlices.Core.Abstracts.Responses;

/// <summary>
/// Specifies different reasons for a business failure
/// </summary>
public enum FailureKind
{
    /// <summary>
    /// Error used with authentication errors
    /// </summary>
    NotAuthenticatedUser,
    /// <summary>
    /// Error used with authorization errors
    /// </summary>
    NotAllowedUser,
    /// <summary>
    /// Error used when a resource is not found
    /// </summary>
    NotFoundResource,
    /// <summary>
    /// Error used when a concurrency error occurs
    /// </summary>
    ConcurrencyError,
    /// <summary>
    /// Error used when a contract validation fails
    /// </summary>
    ContractValidation,
    /// <summary>
    /// Error used when a domain validation fails
    /// </summary>
    DomainValidation,
    /// <summary>
    /// Error used when a not specified error occurs
    /// </summary>
    DefaultError,
    /// <summary>
    /// Error used when an unhandled exception occurs
    /// </summary>
    UnhandledException
}

/// <summary>
/// Represents a failure in the given business logic
/// </summary>
/// <param name="Kind">Reason of the failure</param>
/// <param name="Errors">Error messages related to the failure</param>
public readonly record struct BusinessFailure(FailureKind Kind, string[] Errors)
{
    /// <summary>
    /// Shortcut to create a <see cref="BusinessFailure"/> with specified <see cref="FailureKind"/> 
    /// </summary>
    public static class Of
    {
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/>
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/> and no error messages</returns>
        public static BusinessFailure UserNotAuthenticated() => new(FailureKind.NotAuthenticatedUser, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/> and and error message
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/> and one error message</returns>
        public static BusinessFailure UserNotAuthenticated(string error) => new(FailureKind.NotAuthenticatedUser, new[] { error });

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/>
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/> and no error messages</returns>
        public static BusinessFailure UserNotAllowed() => new(FailureKind.NotAllowedUser, Array.Empty<string>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/> and one error message
        /// </summary>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/> and one error message</returns>
        public static BusinessFailure UserNotAllowed(string error) => new(FailureKind.NotAllowedUser, new []{ error });

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

