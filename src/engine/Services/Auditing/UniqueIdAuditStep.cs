using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class UniqueIdAuditStep(ILogger<UniqueIdAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var groups = context
            .Sources
            .GroupBy(source => source.MigrationId)
            .Where(group => group.Count() > 1)
            .ToArray();

        if (groups.Length == 0)
        {
            return Task.FromResult(0);
        }

        foreach (var group in groups)
        {
            var list = string.Join(Environment.NewLine, group.Select(s => $"  -> {s.Context.TruncateEnd(75)}"));
            logger.LogError("Migration id {id} found in multiple sources:\n{list}",
                group.Key,
                list);
        }

        return Task.FromResult(groups.Length);
    }

    /// <inheritdoc />
    public override string ToString() => "Unique migration id";
}