using DbForward.Utilities;

namespace UnitTests;

public sealed class StaticTimer : IOperationTimer
{
    /// <inheritdoc />
    public void Restart()
    {
    }

    /// <inheritdoc />
    public TimeSpan Elapsed => TimeSpan.FromMilliseconds(50);
}