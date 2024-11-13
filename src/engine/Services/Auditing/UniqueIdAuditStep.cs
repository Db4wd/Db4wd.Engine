using DbForward.Models;
using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class UniqueIdAuditStep(ILogger<UniqueIdAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var lookup = new Dictionary<Guid, SourceHeader>();
        var errors = 0;
        
        foreach (var source in context.Sources)
        {
            if (lookup.TryAdd(source.MigrationId, source))
                continue;

            errors++;
            
            logger.LogError(
                "Source {current} conflicts with {previous} with respect to migration {id}",
                source.Context,
                lookup[source.MigrationId].Context,
                source.MigrationId);
        }

        return Task.FromResult(errors);
    }

    /// <inheritdoc />
    public override string ToString() => "Unique migration id";
}