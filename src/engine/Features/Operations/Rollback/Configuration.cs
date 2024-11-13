using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Operations.Rollback;

public sealed class Options : MigrationOptions
{
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "rollback",
            "Rollback migrations on the target database");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddCommonMigrationOptions(CliScope.Self)
            .AddOption(x => x.TargetId,
                ["--target-id"],
                description: "The migration to rollback the target database to",
                operandSyntax: "ID")
            .AddOption(x => x.TargetDbVersion,
                ["--target-version"],
                description: "The version to rollback the target database to",
                operandSyntax: "VERSION",
                validation: rule => rule.Must(
                    evaluator: (model, value) =>
                    {
                        var (x, y) = (model.TargetId == null, value == null);
                        return x != y;
                    },
                    message: "This or --target-id option required for rollback"));

        builder.AddHandler(command);
    }
}