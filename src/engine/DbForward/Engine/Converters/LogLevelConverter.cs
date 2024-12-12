using Microsoft.Extensions.Logging;
using Vertical.Cli.Conversion;

namespace DbForward.Engine.Converters;

public sealed class LogLevelConverter : ValueConverter<LogLevel>
{
    /// <inheritdoc />
    public override LogLevel Convert(string str) => str.ToLower() switch
    {
        "m" => LogLevel.Warning,
        "minimal" => LogLevel.Warning,
        "n" => LogLevel.Information,
        "normal" => LogLevel.Information,
        "d" => LogLevel.Debug,
        "detailed" => LogLevel.Debug,
        _ => throw new ArgumentException("invalid value")
    };
}