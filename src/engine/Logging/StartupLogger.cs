using Microsoft.Extensions.Logging;

namespace DbForward.Logging;

internal static class StartupLogger
{
    private static readonly List<Action<ILogger>> LogActions = [];
    
    public static void Log(LogLevel level, string message, params object[] args)
    {
        LogActions.Add(logger => logger.Log(level, message, args));
    }

    public static void Flush(ILogger logger)
    {
        foreach (var action in LogActions)
        {
            action(logger);
        }
    }
}