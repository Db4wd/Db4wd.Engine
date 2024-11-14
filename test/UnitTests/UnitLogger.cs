using System.Text;
using Microsoft.Extensions.Logging;

namespace UnitTests;

public sealed class UnitLogger : ILogger
{
    public sealed class Provider : ILoggerProvider
    {
        public UnitLogger Logger { get; } = new();
        
        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) => Logger;

        /// <inheritdoc />
        public override string ToString() => Logger.ToString();
    }
    
    private readonly StringBuilder buffer = new();

    public void Clear() => buffer.Clear();

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        buffer.AppendLine($"{logLevel} {eventId} {formatter(state, exception)}");
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string ToString() => buffer.ToString();
}