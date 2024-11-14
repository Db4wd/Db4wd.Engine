using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class TargetDbVersionAuditStep(ILogger<TargetDbVersionAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        if (context.TargetVersion == null
            || context.SourceTargets.Any(source => source.DbVersion == context.TargetVersion))
            return Task.FromResult(0);
        
        logger.LogError("Missing source for target dbVersion {version}", context.TargetVersion);
        return Task.FromResult(1);
    }

    /// <inheritdoc />
    public override string ToString() => "Target version";
}