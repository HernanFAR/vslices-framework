using System.Text.Json;
using VSlices.CrossCutting.Logging.Attributes;

namespace VSlices.CrossCutting.Logging.Configurations;

/// <summary>
/// Configuration for the <see cref="LoggingBehavior{TRequest,TResponse}"/>
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Describer to use for the logging messages
    /// </summary>
    public ILoggingDescriber Describer { get; set; } = new DefaultLoggingDescriber();

    /// <summary>
    /// Options to use for the serialization of the logging pipeline
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; set; }

    /// <summary>
    /// Indicates if the logging should be done for all the requests, even if the class is decorated with <see cref="NoLoggableAttribute"/>
    /// </summary>
    public bool SerializeAll { get; set; }

}
