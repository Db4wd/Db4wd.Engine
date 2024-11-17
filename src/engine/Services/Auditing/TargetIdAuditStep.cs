using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class TargetIdAuditStep(ILogger<TargetIdAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        if (context.SourceTargets.Count == 0)
            return Task.FromResult(0);
        
        if (context.TargetId == null || context.SourceTargets.Any(source => PartialGuidEqualityComparer.Default
                .Equals(source.MigrationId, context.TargetId)))
            return Task.FromResult(0);

        logger.LogError("Source for migration {id} not found.", context.TargetId);
        return Task.FromResult(1);
    }

    /// <inheritdoc />
    public override string ToString() => "Target identifier";
}