using Db4Wd.Cli;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Migrations;

public abstract class Options : ConnectionOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "migrations",
            "Manage migrations in the target database");

        command.AddConnectionOptions(CliScope.Descendants);
        
        builder.Configure(command);
    }
}