using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Source;

public sealed class Options : ConnectionOptions
{
    public Guid MigrationId { get; set; }
    
    public FileInfo? OutputPath { get; set; }
    
    public bool Overwrite { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "source",
            "Gets the content of the source file used to apply a migration");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddArgument(x => x.MigrationId,
                Arity.One,
                description: "The migration id to retrieve the source content of",
                operandSyntax: "ID")
            .AddOption(x => x.OutputPath,
                ["--out", "--output-path"],
                description: "Path to write the content to. If not given, the content is written to the console.",
                operandSyntax: "PATH",
                validation: rule => rule.Must(
                    evaluator: (model, value) => model.Overwrite || value is null || !File.Exists(value.FullName),
                    message: "Target file exists (use --overwrite switch)"))
            .AddSwitch(x => x.Overwrite,
                ["--overwrite"],
                description: "Overwrites an existing file");

        builder.AddHandler(command);
    }
}