using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.NewEnvironment;

public sealed class Options : GlobalOptions
{
    public FileInfo OutputPath { get; set; } = default!;
    
    public bool Overwrite { get; set; }

    public KeyValuePair<string, string>[] Properties { get; set; } = [];
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "new-env",
            "Creates a new environment file");

        command
            .AddArgument(x => x.OutputPath,
                arity: Arity.One,
                description: "Path to the file to write",
                operandSyntax: "PATH")
            .AddSwitch(x => x.Overwrite,
                ["--overwrite"],
                description: "Whether to overwrite an existing file",
                validation: rule => rule.Must(
                    evaluator: (model, value) => value || !model.OutputPath.Exists,
                    message: "File exists (use --overwrite switch)"))
            .AddOption(x => x.Properties,
                ["--prop"],
                arity: Arity.ZeroOrMany,
                description: "A key/value pair that describes a property to include in the file",
                operandSyntax: "KEY=VALUE");

        builder.AddHandler(command);
    }
}