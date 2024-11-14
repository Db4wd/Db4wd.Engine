using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Status;

public sealed class Options : ConnectionOptions
{
    public DirectoryInfo BasePath { get; set; } = default!;
    
    public string? SearchPattern { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "status",
            "Gets the most current migration status of the target database");

        command
            .AddConnectionOptions(CliScope.Self)
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
                operandSyntax: "PATTERN");

        builder.AddHandler(command);
    }
}