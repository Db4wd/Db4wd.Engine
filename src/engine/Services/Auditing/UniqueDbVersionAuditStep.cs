using DbForward.Models;
using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class UniqueDbVersionAuditStep(ILogger<UniqueDbVersionAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var lookup = new Dictionary<string, SourceHeader>();
        var errors = 0;
        
        foreach (var source in context.Sources)
        {
            if (lookup.TryAdd(source.DbVersion, source))
                continue;

            errors++;
            
            logger.LogError(
                "Source {current} conflicts with {previous} with respect to dbVersion {version}",
                source.Context,
                lookup[source.DbVersion].Context,
                source.DbVersion);
        }

        return Task.FromResult(errors);
    }

    /// <inheritdoc />
    public override string ToString() => "Unique db version";
}