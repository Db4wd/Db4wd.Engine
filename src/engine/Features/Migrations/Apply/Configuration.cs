using Db4Wd.Cli;
using Db4Wd.Services;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Migrations.Apply;

public sealed class Options : MigrationOperatorOptions
{
    /// <inheritdoc />
    public override MigrationOperatorMode OperatorMode => MigrationOperatorMode.Apply;
}

public sealed class Configuration : IFeatureConfiguration<Migrations.Options>
{
    /// <inheritdoc />
    public void Configure(CliCommand<Migrations.Options> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "apply",
            description: "Applies pending migrations");

        command.AddBaseMigrationOptions();

        builder.AddHandler(command);
    }
}