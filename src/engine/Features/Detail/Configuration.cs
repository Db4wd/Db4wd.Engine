using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Detail;

public sealed class Options : ConnectionOptions
{
    public Guid Id { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions> 
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "detail",
            "Gets detail about an applied migration");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddArgument(x => x.Id,
                arity: Arity.One,
                description: "The id of the migration to retrieve detail for");

        builder.AddHandler(command);
    }
}