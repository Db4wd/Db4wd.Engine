using Db4Wd.Cli;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.UpdateConnector;

public sealed class Options : ConnectionOptions
{
    public System.Version? VersionId { get; set; }
    
    public bool Latest { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "update-connector",
            "Update the connector management version");

        command
            .AddConnectionOptions()
            .AddOption(x => x.VersionId,
                ["--version"],
                description: "The management version to update to",
                operandSyntax: "SEMVER",
                validation: rule => rule.Must(
                    evaluator: static (model, version) => version != null || model.Latest,
                    message: "required when --latest switch is not used"))
            .AddSwitch(x => x.Latest,
                ["--latest"],
                description: "Update to the newest version");

        builder.AddHandler(command);
    }
}