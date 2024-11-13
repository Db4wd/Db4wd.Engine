using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.List;

public sealed class Options : ConnectionOptions
{
    public int Limit { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "list",
            "List migrations applied to the target database");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddOption(x => x.Limit,
                ["--limit"],
                defaultProvider: () => 10,
                description: "Max number of migrations to show",
                operandSyntax: "COUNT");

        builder.AddHandler(command);
    }
}