using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Initialize;

public sealed class Options : ConnectionOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            ["init", "initialize"],
            "Initializes migrations in the target database");

        command.AddConnectionOptions(CliScope.Self);

        builder.AddHandler(command);
    }
}