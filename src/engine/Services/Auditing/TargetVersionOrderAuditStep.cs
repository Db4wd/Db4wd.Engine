using DbForward.Constants;
using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class TargetVersionOrderAuditStep(ILogger<TargetVersionOrderAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        if (context.Operation == SourceOperation.Rollback)
            return Task.FromResult(0);

        var comparer = context.VersionComparer;
        var highestAppliedVersion = context
            .AppliedEntries
            .MaxBy(entry => entry.DbVersion, context.VersionComparer)?
            .DbVersion;
        var versionSet = new HashSet<string>(context.AppliedEntries.Select(entry => entry.DbVersion));

        if (highestAppliedVersion == null)
            return Task.FromResult(0);

        var errors = 0;
        foreach (var target in context.SourceTargets)
        {
            if (!versionSet.Contains(target.DbVersion))
                continue;
            
            if (comparer.Compare(target.DbVersion, highestAppliedVersion) > 0)
                continue;

            errors++;
            logger.LogError("Source {source} version {target} is lower than current version {current}. Migration files " +
                            "must be applied in order.",
                target.Context,
                target.DbVersion,
                highestAppliedVersion);
        }

        return Task.FromResult(errors);
    }

    /// <inheritdoc />
    public override string ToString() => "Pending sources insert order";
}