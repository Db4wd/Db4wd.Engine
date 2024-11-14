using Microsoft.Extensions.Logging;

namespace DbForward.Services.Auditing;

public sealed class UniqueFileNameAuditStep(ILogger<UniqueFileNameAuditStep> logger) : ISourceAuditStep
{
    /// <inheritdoc />
    public Task<int> AuditAsync(AuditingContext context, CancellationToken cancellationToken)
    {
        var duplicateFileNames = context
            .Sources
            .GroupBy(source => Path.GetFileName(source.Context))
            .Where(group => group.Count() > 1)
            .ToArray();

        if (duplicateFileNames.Length == 0)
            return Task.FromResult(0);

        foreach (var group in duplicateFileNames)
        {
            var list = string.Join(Environment.NewLine, group.Select(source => $"  -> {source.Context}"));
            logger.LogError("Duplicate file names detected:\n{list}", list);
        }

        return Task.FromResult(duplicateFileNames.Length);
    }
}