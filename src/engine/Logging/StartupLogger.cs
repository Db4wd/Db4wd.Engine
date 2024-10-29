using Microsoft.Extensions.Logging;

namespace Db4Wd.Logging;

public sealed class StartupLogger
{
    private readonly List<Action<ILogger>> _logActions = [];
    
    public void Log(Action<ILogger> log)
    {
        _logActions.Add(log);
    }

    public void Flush(ILogger logger)
    {
        foreach (var entry in _logActions)
        {
            entry(logger);
        }
    }
}