using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace DbForward.Features.Metadata;

public enum MetadataMode
{
    Insert,
    Delete
}

public sealed class Options : ConnectionOptions
{
    public MetadataMode Mode { get; set; }

    public KeyValuePair<string, string>[] Metadata { get; set; } = [];

    public DirectoryInfo BasePath { get; set; } = default!;
    
    public string? SearchPattern { get; set; }
    
    public bool Confirm { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "metadata",
            "Inserts or deletes metadata from the tracking schema.",
            remarks:
            [
                new HelpRemarks("Remarks",
                    [
                        "Use this command to repair corrupt metadata. This adds or deletes migration entries from the metadata tables without executing source directives against the target database.",
                        "Insert mode: adds metadata from the first pending source file that is the next higher version of the target database than what has already been applied.",
                        "Delete mode: deletes the metadata of the last migration that was applied."
                    ])
            ]);

        command
            .AddConnectionOptions(CliScope.Self)
            .AddArgument(x => x.Mode,
                description: "Operation to perform.",
                operandSyntax: "INSERT | DELETE")
            .AddOption(x => x.Metadata,
                ["--tag"],
                arity: Arity.ZeroOrMany,
                description: "Metadata tag to include in the operation's entry",
                operandSyntax: "KEY=VALUE")
            .AddOption(x => x.BasePath,
                ["--base-path"],
                description: "Path where migration sources can be found",
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                validation: rule => rule.Exists(),
                operandSyntax: "PATH")
            .AddOption(x => x.SearchPattern,
                ["--search-pattern"],
                description:
                "File system glob pattern used to match files (defaults to recursive search using the default file extension",
                operandSyntax: "PATTERN")
            .AddSwitch(x => x.Confirm,
                ["--confirm"],
                description: "Bypasses the confirmation prompt");

        builder.AddHandler(command);
    }
}