using Db4Wd.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Locks.List;

public sealed class Options : Locks.Options
{
}

public sealed class Configuration : IFeatureConfiguration<Locks.Options>
{
    /// <inheritdoc />
    public void Configure(CliCommand<Locks.Options> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "list",
            "List stale resource locks");

        builder.AddHandler(command);
    }
}