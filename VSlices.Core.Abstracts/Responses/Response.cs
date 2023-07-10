namespace VSlices.Core.Abstracts.Responses;

/// <summary>
/// Represents a response from a process.
/// </summary>
/// <typeparam name="TResponse">The expected response in success case</typeparam>
public readonly struct Response<TResponse>
{
    private readonly BusinessFailure? _businessFailure;
    private readonly TResponse? _successValue;

    /// <summary>
    /// Indicates if process was successful
    /// </summary>
    public bool IsSuccess => _businessFailure == null;

    /// <summary>
    /// Indicates if process failed
    /// </summary>
    public bool IsFailure => _businessFailure != null;

    /// <summary>
    /// The success response of the process, throws <see cref="InvalidOperationException"/> if accessed on failure
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TResponse SuccessValue => _successValue ?? throw new InvalidOperationException(nameof(_successValue));

    /// <summary>
    /// The failure response of the process, throws <see cref="InvalidOperationException"/> if accessed on success
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public BusinessFailure BusinessFailure => _businessFailure ?? throw new InvalidOperationException(nameof(_businessFailure));

    /// <summary>
    /// Creates a new instance of <see cref="Response{TResponse}"/> with a success value
    /// </summary>
    /// <param name="successValue">The success value of the process</param>
    public Response(TResponse successValue)
    {
        _successValue = successValue;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Response{TResponse}"/> with a failure value
    /// </summary>
    /// <param name="businessFailure">A struct with the failure detail</param>
    public Response(BusinessFailure businessFailure)
    {
        _businessFailure = businessFailure;
    }

    public static implicit operator Response<TResponse>(BusinessFailure businessFailure) => new(businessFailure);
    public static implicit operator Response<TResponse>(TResponse businessFailure) => new(businessFailure);
}
