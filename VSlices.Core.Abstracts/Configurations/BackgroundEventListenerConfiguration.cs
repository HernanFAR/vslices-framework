namespace VSlices.Core.Abstracts.Configurations;

public enum MoveActions
{
    MoveLast,
    InmediateRetry
}

public class BackgroundEventListenerConfiguration
{
    public MoveActions ActionInException { get; set; } = MoveActions.MoveLast;

    public int MaxRetries { get; set; } = 3;

}
