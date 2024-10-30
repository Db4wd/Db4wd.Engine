using Db4Wd.Cli;
using Db4Wd.Services;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Migrations.Rollback;

public sealed class Options : MigrationOperatorOptions
{
    /// <inheritdoc />
    public override MigrationOperatorMode OperatorMode => MigrationOperatorMode.Rollback;
}

public sealed class Configuration : IFeatureConfiguration<Migrations.Options>
{
    /// <inheritdoc />
    public void Configure(CliCommand<Migrations.Options> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "rollback",
            description: "Reverts applied migrations");

        command.AddBaseMigrationOptions();

        builder.AddHandler(command);
    }
}