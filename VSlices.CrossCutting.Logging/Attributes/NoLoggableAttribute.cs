namespace VSlices.CrossCutting.Logging.Attributes;

/// <summary>
/// Attribute to indicate that the class should not be logged
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NoLoggableAttribute : Attribute
{
}
