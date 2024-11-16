using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace UnitTests;

public sealed class JsonCaptureLogger : ILogger
{
    private readonly List<object> entries = new(32);
    
    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var values = new Dictionary<string, object>((IEnumerable<KeyValuePair<string, object>>)state!);
        var format = (string)values["{OriginalFormat}"];

        if (!format.Contains("{@"))
        {
            entries.Add(new
            {
                simple = formatter(state, null),
                exception = exception?.ToString() ?? "(none)"
            });
            return;
        }
        
        entries.Add(new
        {
            complex = values,
            exception = exception?.ToString() ?? "(none)"
        });
    }

    public void Clear() => entries.Clear();

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonSerializer.Serialize(entries, JsonOptions.Default);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public sealed class Provider : ILoggerProvider
    {
        private readonly JsonCaptureLogger logger = new();
        
        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) => logger;

        public string GetVerifiable() => logger.ToString();

        public void ClearLogger() => logger.Clear();
    }
}