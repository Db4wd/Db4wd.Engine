using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class HashCodeAuditStep(IFileSystem fileSystem, ILogger<HashCodeAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public async Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        if (context.SourceLookup is not { } lookup)
        {
            logger.LogError("Hash code audit abandoned.");
            return 1;
        }

        var errors = 0;
        
        foreach (var entry in context.AppliedEntries)
        {
            if (!lookup.TryGetValue(entry.MigrationId, out var source))
                continue;

            var hashCode = await fileSystem.ComputeShaAsync(source.Context);

            if (hashCode == entry.Sha)
                continue;
            
            logger.LogError("Source {source} content changed since migration applied (hashes differ)",
                source.Context);

            errors++;
        }

        return errors;
    }

    /// <inheritdoc />
    public override string ToString() => "Source content hash";
}