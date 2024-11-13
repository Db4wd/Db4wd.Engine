using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Status;

public sealed class Options : ConnectionOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "status",
            "Gets the most current migration status of the target database");

        command.AddConnectionOptions(CliScope.Self);

        builder.AddHandler(command);
    }
}