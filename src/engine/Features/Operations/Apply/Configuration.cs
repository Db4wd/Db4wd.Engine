using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Operations.Apply;

public sealed class Options : MigrationOptions
{
    public KeyValuePair<string, string>[] Metadata { get; set; } = [];
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "apply",
            "Applies migrations to the target database");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddCommonMigrationOptions(CliScope.Self)
            .AddOption(x => x.TargetId,
                ["--target-id"],
                description: "The migration to update the target database to",
                operandSyntax: "ID")
            .AddOption(x => x.TargetDbVersion,
                ["--target-version"],
                description: "The version to update the target database to",
                operandSyntax: "VERSION",
                validation: rule => rule.Must(
                    evaluator: (model, value) =>
                    {
                        var (x, y) = (model.TargetId == null, value == null);
                        return (x && y) || x != y;
                    },
                    message: "Cannot use both --target-id and --target-version"))
            .AddOption(x => x.Metadata,
                ["--tag"],
                arity: Arity.ZeroOrMany,
                description: "Metadata tag to associate with the operation",
                operandSyntax: "KEY=VALUE");


        builder.AddHandler(command);
    }
}