using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.NewSource;

public sealed class Options : ConnectionOptions
{
    public DirectoryInfo BasePath { get; set; } = default!;

    public KeyValuePair<string, string>[] Metadata { get; set; } = [];
    
    public bool Edit { get; set; }
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
            .AddArgument(x => x.BasePath,
                description: "Path where the new source will be written to",
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                arity: Arity.One,
                operandSyntax: "PATH",
                validation: rule => rule.Exists("Given path does not exist"))
            .AddOption(x => x.Metadata,
                ["--tag"],
                arity: Arity.ZeroOrMany,
                description: "A key/value pair that describes a metadata tag to include in the template",
                operandSyntax: "KEY=VALUE")
            .AddSwitch(x => x.Edit,
                ["--edit"],
                description: "Opens the configured editor ($DBFWD_EDITOR) for the file");

        builder.AddHandler(command);
    }
}