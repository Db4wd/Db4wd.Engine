using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging;

namespace DbForward.Services;

public interface ISourceAuditor
{
    /// <summary>
    /// Audits sources prior to an operation.
    /// </summary>
    /// <param name="context">Auditing context</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><c>true</c> if the audit succeeded</returns>
    Task<bool> AuditAsync(AuditingContext context, CancellationToken cancellationToken);
}

internal sealed class SourceAuditor(IEnumerable<ISourceAuditStep> auditSteps,
    ILogger<SourceAuditor> logger) : ISourceAuditor
{
    /// <inheritdoc />
    public async Task<bool> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var errors = 0;

        foreach (var auditStep in auditSteps)
        {
            var count = await auditStep.AuditAsync(context, cancellationToken);
            errors += count;

            if (count > 0)
                continue;

            logger.LogDebug("{type} audit completed", auditStep);
        }

        return errors == 0;
    }
}