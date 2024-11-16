using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Services;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Audit;

public sealed class Feature(IDatabaseExtension extension,
    ISourceFileManager sourceManager,
    ISourceAuditor sourceAuditor,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);

        // Gather data
        var appliedEntries = await metadataContext.GetEntriesAsync(cancellationToken);
        logger.LogDebug("Loaded {count} migration entry/entries", appliedEntries.Count);

        var sourceReader = extension.GetSourceReader();
        var sources = await sourceManager.GetSourceHeadersInPathAsync(
            options.BasePath,
            extension.ResolveSearchPattern(options.SearchPattern, logger),
            sourceReader,
            ReadOnlyDictionary<string, string>.Empty,
            cancellationToken);

        logger.LogInformation("Matched {count} source(s)", sources.Count);

        // Build target sources (those which will be applied)
        var versionComparer = sourceReader.GetVersionComparer();
        var appliedIdentifiers = new HashSet<Guid>(appliedEntries.Select(entry => entry.MigrationId));
        var sourceTargets = sources
            .Where(source => !appliedIdentifiers.Contains(source.MigrationId))
            .OrderBy(source => source.DbVersion, versionComparer)
            .ToArray();
        
        // Audit
        var auditContext = new AuditingContext(
            SourceOperation.Migrate,
            sources,
            sourceTargets,
            appliedEntries,
            null,
            null,
            versionComparer);

        if (!await sourceAuditor.AuditAsync(auditContext, cancellationToken))
        {
            logger.LogError("Source audit failed");
            return -1;
        }

        logger.LogInformation("Validated the following:");
        
        foreach (var source in sources.OrderBy(source => source.Context))
        {
            var description = appliedIdentifiers.Contains(source.MigrationId)
                ? "applied"
                : "pending";
            
            logger.LogInformation("  -> {source} = {description}", source.Context, description);
        }
        
        logger.LogInformation("{ok} All sources validated", OkToken.Default);
        return 0;
    }
}