using Vertical.Cli.Configuration;

namespace DbForward.Features.About;

public sealed class Options : GlobalOptions
{
    
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "about",
            "Displays information about the application");

        builder.AddHandler(command);
    }
}