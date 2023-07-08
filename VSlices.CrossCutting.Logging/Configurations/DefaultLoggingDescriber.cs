namespace VSlices.CrossCutting.Logging.Configurations;

public interface ILoggingDescriber
{
    string Initial { get; }
    string InitialWithoutProperties { get; }

    string Success { get; }
    string SuccessWithoutProperties { get; }

    string Failure { get; }
}

public sealed class DefaultLoggingDescriber : ILoggingDescriber
{
    public string Initial => "Log hour: {0} | Starting handling of {1}, with the following properties: {2}.";
    public string InitialWithoutProperties => "Log hour: {0} | Starting handling of {1}.";

    public string Success => "Log hour: {0} | Finishing handling of {1}, response obtained correctly: {2}.";
    public string SuccessWithoutProperties => "Log hour: {0} | Finishing handling of {1}, response obtained correctly.";

    public string Failure => "Log hour: {0} | Finishing handling of {1}, response obtained with errors: {2}.";

}
