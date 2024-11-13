using Microsoft.Extensions.Logging;

namespace DbForward.Logging;

public sealed class LoggerCallbackException(Action<ILogger> @event) : Exception("LoggerCallbackException")
{
    internal void Log(ILogger logger) => @event(logger);
}