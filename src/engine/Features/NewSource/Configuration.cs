using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.NewSource;

public sealed class Options : ConnectionOptions
{
    public FileInfo OutputPath { get; set; } = default!;

    public KeyValuePair<string, string>[] Metadata { get; set; } = [];
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "new",
            "Creates a new migration source file template");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddArgument(x => x.OutputPath,
                description: "Path and name of the new file to write",
                arity: Arity.One,
                operandSyntax: "PATH",
                validation: rule => rule.DoesNotExist("File already exists (will not overwrite)"))
            .AddOption(x => x.Metadata,
                ["--tag"],
                arity: Arity.ZeroOrMany,
                description: "A key/value pair that describes a metadata tag to include in the template",
                operandSyntax: "KEY=VALUE");

        builder.AddHandler(command);
    }
}