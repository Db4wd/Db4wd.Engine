using DbForward.Engine.Features;
using DbForward.Engine.Models;
using Vertical.Cli.Configuration;

namespace DbForward.Engine.Options;

public class NewEnvironmentOptions : ConnectionOptions, INewEnvironmentOptions
{
    public FileInfo OutputFile { get; set; } = default!;
    
    public bool Overwrite { get; set; }
    
    public bool Edit { get; set; }
}

internal sealed class NewEnvironmentOptionsConfiguration : IFeatureConfiguration
{
    /// <inheritdoc />
    public void Configure(FeatureContext context)
    {
        context.CliBuilder.MapModel<NewEnvironmentOptions>(map => map
            .Option(x => x.OutputFile, ["--out"],
                arity: Arity.One,
                validation: rule => rule.Must(
                    evaluator: static (opt, file) => opt.Overwrite || !file.Exists,
                    message: "Target file exists (use --overwrite option)"))
            .Switch(x => x.Overwrite, ["--overwrite"])
            .Switch(x => x.Edit, ["--edit"]));
    }
}