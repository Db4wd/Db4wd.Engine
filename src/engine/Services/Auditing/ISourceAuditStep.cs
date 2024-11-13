namespace DbForward.Services.Auditing;

public interface ISourceAuditStep
{
    /// <summary>
    /// Audits sources defined in a context.
    /// </summary>
    /// <param name="context">Auditing context</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>The number of errors found</returns>
    Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken);
}