using VSlices.Core.Abstracts.BusinessLogic;

namespace VSlices.Core.Abstracts.Configurations;

/// <summary>
/// Represents what to do in the background event listener when a problem occurs in the event <see cref="IHandler{TRequest}"/>.
/// </summary>
public enum MoveActions
{
    /// <summary>
    /// Move to the last position in the queue
    /// </summary>
    MoveLast,
    /// <summary>
    /// Inmediate retry the event <see cref="IHandler{TRequest}"/>
    /// </summary>
    InmediateRetry
}

/// <summary>
/// Represents the configuration for the background event listener
/// </summary>
public class BackgroundEventListenerConfiguration
{
    /// <summary>
    /// Represents what to do then a exception occurs in the event <see cref="IHandler{TRequest}"/>.
    /// </summary>
    public MoveActions ActionInException { get; set; } = MoveActions.MoveLast;

    /// <summary>
    /// Represents the maximum number of retries for the event <see cref="IHandler{TRequest}"/>.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

}
