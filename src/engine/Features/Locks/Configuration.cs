using Db4Wd.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Locks;

public abstract class Options : GlobalOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "locks",
            "View and manage system locks");
        
        builder.Configure(command);
    }
}