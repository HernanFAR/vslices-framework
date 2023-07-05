namespace VSlices.Core.Abstracts.Responses;

public readonly struct Response<TResponse>
{
    private readonly BusinessFailure? _businessFailure;
    private readonly TResponse? _successValue;

    public bool IsSuccess => _businessFailure == null;
    public bool IsFailure => _businessFailure != null;
    public TResponse SuccessValue => _successValue ?? throw new InvalidOperationException(nameof(_successValue));
    public BusinessFailure BusinessFailure => _businessFailure ?? throw new InvalidOperationException(nameof(_businessFailure));

    public Response(TResponse successValue)
    {
        _successValue = successValue;
    }

    public Response(BusinessFailure businessFailure)
    {
        _businessFailure = businessFailure;
    }

    public static implicit operator Response<TResponse>(BusinessFailure businessFailure) => new(businessFailure);
    public static implicit operator Response<TResponse>(TResponse businessFailure) => new(businessFailure);
}
