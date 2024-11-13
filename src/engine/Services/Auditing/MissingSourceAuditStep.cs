using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class MissingSourceAuditStep(ILogger<MissingSourceAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        if (context.SourceLookup is not { } sourceLookup)
        {
            logger.LogError("Missing source audit abandoned");
            return Task.FromResult(1);
        }

        var errors = 0;
        foreach (var entry in context.AppliedEntries)
        {
            if (sourceLookup.ContainsKey(entry.MigrationId)) 
                continue;

            errors++;
            logger.LogError("Missing source {source} for migration {id}",
                entry.SourceFile,
                entry.SourceFile);
        }

        return Task.FromResult(errors);
    }

    /// <inheritdoc />
    public override string ToString() => "Missing sources";
}