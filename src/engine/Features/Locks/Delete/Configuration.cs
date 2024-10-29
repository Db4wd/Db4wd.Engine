using Db4Wd.Cli;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Locks.Delete;

public sealed class Options : Locks.Options
{
    public Guid? LockId { get; set; }
    
    public bool All { get; set; }
}

public sealed class Configuration : IFeatureConfiguration<Locks.Options>
{
    /// <inheritdoc />
    public void Configure(CliCommand<Locks.Options> parent, FeatureBuilder builder)
    {
        var command = parent.AddSubCommand<Options>(
            "delete",
            "Delete stale resource locks");

        const string validationMessage = "--id or --all option required";
        
        command
            .AddOption(x => x.LockId,
                ["--id", "--lock-id"],
                description: "The id of the lock to release",
                operandSyntax: "UUID",
                validation: rule => rule.Must(
                        evaluator: static (model, id) => id.HasValue || model.All,
                        message: validationMessage))
            .AddSwitch(x => x.All,
                ["--all"],
                description: "Release all stale locks",
                validation: rule => rule.Must(
                    evaluator: static (model, all) => all || model.LockId.HasValue,
                    message: validationMessage));

        builder.AddHandler(command);
    }
}