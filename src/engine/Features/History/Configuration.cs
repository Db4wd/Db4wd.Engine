using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.History;

public sealed class Options : ConnectionOptions
{
    public Guid Id { get; set; }
    
    public int Limit { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<GlobalOptions>
{
    /// <inheritdoc />
    public void Configure(CliCommand<GlobalOptions> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "history",
            "Gets the history of a migration id");

        command
            .AddConnectionOptions(CliScope.Self)
            .AddArgument(x => x.Id,
                arity: Arity.One,
                description: "The id of the migration to retrieve history for")
            .AddOption(x => x.Limit,
                ["--limit"],
                description: "Maximum number of records to return (defaults to 10).",
                defaultProvider: () => 10);

        builder.AddHandler(command);
    }
}