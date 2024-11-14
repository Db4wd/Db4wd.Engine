using Vertical.Cli.Configuration;

namespace DbForward.Features;

public interface IFeatureConfiguration<TParentOptions> where TParentOptions : GlobalOptions
{
    /// <summary>
    /// Configures a command.
    /// </summary>
    /// <param name="parent">The parent command.</param>
    /// <param name="builder">Feature builder used to configure sub commands and handlers.</param>
    void Configure(CliCommand<TParentOptions> parent, FeatureBuilder builder);
}