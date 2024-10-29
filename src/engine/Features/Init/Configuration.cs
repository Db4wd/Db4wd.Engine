using Db4Wd.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Init;

public sealed class Options : ConnectionOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "init",
            "Initialization migration tracking in the target database");

        command.AddConnectionOptions();

        builder.AddHandler(command);
    }
}