using Db4Wd.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Version;

public sealed class Options : GlobalOptions
{
    
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "version",
            "Get version information for the current extension");

        builder.AddHandler(command);
    }
}