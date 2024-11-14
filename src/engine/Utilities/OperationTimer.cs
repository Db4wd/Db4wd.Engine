using System.Diagnostics;

namespace DbForward.Utilities;

public interface IOperationTimer
{
    void Restart();

    TimeSpan Elapsed { get; }
}

public delegate IOperationTimer TimerProvider();

public sealed class StopwatchTimer : IOperationTimer
{
    private readonly Stopwatch instance = Stopwatch.StartNew();

    /// <inheritdoc />
    public void Restart() => instance.Restart();

    /// <inheritdoc />
    public TimeSpan Elapsed => instance.Elapsed;
}