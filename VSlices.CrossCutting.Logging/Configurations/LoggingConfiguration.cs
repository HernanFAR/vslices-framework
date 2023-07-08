using System.Text.Json;

namespace VSlices.CrossCutting.Logging.Configurations;

public class LoggingConfiguration
{
    public ILoggingDescriber Describer { get; set; } = new DefaultLoggingDescriber();

    public JsonSerializerOptions? JsonOptions { get; set; }

    public bool SerializeAll { get; set; }

}
