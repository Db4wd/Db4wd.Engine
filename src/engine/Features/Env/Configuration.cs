using Db4Wd.Cli;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Env;

public sealed class Options : GlobalOptions
{
    public DirectoryInfo? BasePath { get; set; }

    public string Name { get; set; } = default!;
    
    public bool Overwrite { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "env",
            "Create a new environment files");

        command
            .AddOption(x => x.BasePath,
                ["--path"],
                description: "The directory in which to write the new file (defaults to HOME/.db4wd/env/<extension>)",
                operandSyntax: "PATH")
            .AddArgument(x => x.Name,
                arity: Arity.One,
                description: "Name of the new environment")
            .AddSwitch(x => x.Overwrite,
                ["--overwrite"],
                description: "Whether to overwrite an existing file");

        builder.AddHandler(command);
    }
}