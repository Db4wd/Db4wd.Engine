using Microsoft.Extensions.Logging;

namespace DbForward.Engine.Logging;

public sealed class StartupLogger : ILogger
{
    public static readonly ILogger Instance = new StartupLogger();

    private StartupLogger()
    {
    }
    
    private readonly List<Action<ILogger>> logActions = [];
    
    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        logActions.Add(logger => logger.Log(logLevel, eventId, state, exception, formatter));
    }

    public void FlushTo(ILogger logger)
    {
        foreach (var action in logActions)
        {
            action(logger);
        }
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => throw new NotImplementedException();
}