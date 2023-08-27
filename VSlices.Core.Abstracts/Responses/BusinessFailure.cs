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
    Unspecified,
    /// <summary>
    /// Error used when an unhandled exception occurs
    /// </summary>
    UnhandledException
}

/// <summary>
/// Represents a validation error
/// </summary>
/// <param name="Name">
/// Name of the property that failed validation
/// </param>
/// <param name="Detail">
/// A human-readable explanation specific to this occurrence of the error
/// </param>
public readonly record struct ValidationError(string Name, string Detail);

/// <summary>
/// Represents a failure in the given business logic. Based on <see href="https://datatracker.ietf.org/doc/html/rfc9457"/>
/// </summary>
/// <param name="Kind">
/// Failure kind. See <see cref="FailureKind"/> for more information
/// </param>
/// <param name="Title">
/// A short, human-readable summary of the problem type
/// </param>
/// <param name="Detail">
/// A human-readable explanation specific to this occurrence of the problem
/// </param>
/// <param name="Errors">
/// A list of validation errors, if any
/// </param>
public readonly record struct BusinessFailure(FailureKind Kind, string? Title, string? Detail, ValidationError[] Errors)
{
    /// <summary>
    /// Shortcut to create a <see cref="BusinessFailure"/> with specified <see cref="FailureKind"/> 
    /// </summary>
    public static class Of
    {
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAuthenticatedUser"/>, and the specified title and detail</returns>
        public static BusinessFailure UserNotAuthenticated(string? title = null, string? detail = null) 
            => new(FailureKind.NotAuthenticatedUser, title, detail, null);

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotAllowedUser"/>, and the specified title and detail</returns>
        public static BusinessFailure UserNotAllowed(string? title = null, string? detail = null) 
            => new(FailureKind.NotAllowedUser, title, detail, Array.Empty<ValidationError>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.NotFoundResource"/>, and the specified title and detail</returns>
        public static BusinessFailure NotFoundResource(string? title = null, string? detail = null) 
            => new(FailureKind.NotFoundResource, title, detail, Array.Empty<ValidationError>());
    
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ConcurrencyError"/>, and the specified title and detail</returns>
        public static BusinessFailure ConcurrencyError(string? title = null, string? detail = null)
            => new(FailureKind.ConcurrencyError, title, detail, Array.Empty<ValidationError>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <param name="errors">Related validation errors</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/>, and the specified title and detail</returns>
        public static BusinessFailure ContractValidation(string? title = null, string? detail = null, ValidationError[]? errors = null) 
            => new(FailureKind.ContractValidation, title, detail, errors ?? Array.Empty<ValidationError>());
    
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <param name="error">Related validation error</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.ContractValidation"/>, and the specified title and detail</returns>
        public static BusinessFailure ContractValidation(string? title = null, string? detail = null, ValidationError? error = null)
            => new(FailureKind.ContractValidation, title, detail, error is null ? Array.Empty<ValidationError>() : new[]{ error.Value });

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <param name="errors">Related validation errors</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/>, and the specified title and detail</returns>
        public static BusinessFailure DomainValidation(string? title = null, string? detail = null, ValidationError[]? errors = null)
            => new(FailureKind.DomainValidation, title, detail, errors ?? Array.Empty<ValidationError>());
    
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <param name="error">Related validation error</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.DomainValidation"/>, and the specified title and detail</returns>
        public static BusinessFailure DomainValidation(string? title = null, string? detail = null, ValidationError? error = null)
            => new(FailureKind.DomainValidation, title, detail, error is null ? Array.Empty<ValidationError>() : new[] { error.Value });
    
        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.Unspecified"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.Unspecified"/>, and the specified title and detail</returns>
        public static BusinessFailure Unspecified(string? title = null, string? detail = null) 
            => new(FailureKind.Unspecified, title, detail, Array.Empty<ValidationError>());

        /// <summary>
        /// Creates a <see cref="BusinessFailure"/> with <see cref="FailureKind.UnhandledException"/>
        /// </summary>
        /// <param name="title">Title of the problem</param>
        /// <param name="detail">Detail of the problem</param>
        /// <returns>A <see cref="BusinessFailure"/> with <see cref="FailureKind.UnhandledException"/>, and the specified title and detail</returns>
        public static BusinessFailure UnhandledException(string? title = null, string? detail = null) 
            => new(FailureKind.UnhandledException, title, detail, Array.Empty<ValidationError>());

    }
}

