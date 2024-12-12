using DbForward.Engine.Converters;
using DbForward.Engine.Features;
using Microsoft.Extensions.Logging;
using Vertical.Cli.Validation;

namespace DbForward.Engine.Options;

public class GlobalOptions
{
    public LogLevel LogLevel { get; set; }
    
    public FileInfo? EnvironmentFile { get; set; }
    
    public bool NoTerminal { get; set; }
}

internal sealed class GlobalOptionsConfiguration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context)
    {
        context.CliBuilder.MapModel<GlobalOptions>(map => map
            .Option(x => x.LogLevel, ["-v", "--verbosity"])
            .Option(x => x.EnvironmentFile, ["--env"], validation: rule => rule.ExistsOrNull())
            .Switch(x => x.NoTerminal, ["--no-terminal"]));

        context.CliBuilder.AddConverters(
        [
            new LogLevelConverter(),
            new KeyValuePairConverter()
        ]);
    }
}