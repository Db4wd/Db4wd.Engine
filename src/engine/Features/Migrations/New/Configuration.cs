
using Db4Wd.Cli;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Migrations.New;

public sealed class Options : Migrations.Options
{
    public KeyValuePair<string, string>[] Metadata { get; set; } = [];

    public FileInfo? Path { get; set; } = default!;
}

public sealed class Configuration : IFeatureConfiguration<Migrations.Options>
{
    /// <inheritdoc />
    public void Configure(CliCommand<Migrations.Options> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "new",
            description: "Creates a new migration file");

        command
            .AddOption(x => x.Metadata,
                ["--tag"],
                arity: Arity.ZeroOrMany,
                description: "A key/value metadata tag to include in the generated file",
                operandSyntax: "KEY=VALUE")
            .AddOption(x => x.Path,
                ["--out"],
                description: "The path to write the file to",
                operandSyntax: "PATH",
                validation: rule => rule.DoesNotExistOrIsNull());

        builder.AddHandler(command);
    }
}