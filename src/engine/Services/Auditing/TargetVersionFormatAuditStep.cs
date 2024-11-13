using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class TargetVersionFormatAuditStep(ILogger<TargetVersionFormatAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var errors = 0;
        foreach (var target in context.Sources)
        {
            if (context.VersionComparer.IsValid(target.DbVersion))
                continue;

            errors++;
            logger.LogError("Source {source} has invalid dbVersion '{version}'",
                target.Context,
                target.DbVersion);
        }

        return Task.FromResult(errors);
    }

    /// <inheritdoc />
    public override string ToString() => "Version metadata format";
}