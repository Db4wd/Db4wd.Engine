using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Models;
using DbForward.Services;
using DbForward.Services.Auditing;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DbForward.Features.Metadata;

public sealed class Feature(
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    IAgentContext agentContext,
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

        var sourceReader = extension.GetSourceReader();
        var appliedEntries = await metadataContext.GetEntriesAsync(cancellationToken);
        var sources = await sourceManager.GetSourceHeadersInPathAsync(
            options.BasePath,
            extension.ResolveSearchPattern(options.SearchPattern, logger),
            sourceReader,
            ReadOnlyDictionary<string, string>.Empty,
            cancellationToken);
        
        var metadataOperation = options.Mode == MetadataMode.Delete
            ? SourceOperation.Rollback
            : SourceOperation.Migrate;
        
        var entryIdSet = new HashSet<Guid>(appliedEntries.Select(entry => entry.MigrationId));
        var versionComparer = sourceReader.GetVersionComparer();
        var pendingSource = options.Mode == MetadataMode.Insert
            ? sources
                .Where(source => !entryIdSet.Contains(source.MigrationId))
                .MinBy(source => source.DbVersion, versionComparer)
            : sources
                .Where(source => entryIdSet.Contains(source.MigrationId))
                .MaxBy(source => source.DbVersion, versionComparer);

        if (pendingSource == null)
        {
            logger.LogError("There is no pending source to insert.");
            return -1;
        }

        if (!await sourceAuditor.AuditAsync(new AuditingContext(
                metadataOperation,
                sources,
                [pendingSource],
                appliedEntries,
                null,
                null,
                versionComparer), cancellationToken))
        {
            logger.LogError("Source audit failed; abandoning operation");
            return -1;
        }

        if (!options.Confirm)
        {
            AnsiConsole.WriteLine();
            logger.LogInformation(
                """
                Are you sure you want to {mode} metadata in the tracking schema for the following source:
                  -> Path: {path}
                  -> Migration id: {id}
                  -> DbVersion: {version}
                """,
                options.Mode.ToString().ToLower(),
                pendingSource.Context,
                pendingSource.MigrationId,
                pendingSource.DbVersion);

            AnsiConsole.WriteLine();
            if (!AnsiConsole.Confirm("Confirm"))
                return 0;
        }

        var tracker = new OperationTracker();
        tracker.SetTag("feature/mode", "manual_adjustment");

        var metadataFinal = pendingSource
            .Metadata
            .MergeReplace(options.Metadata);
        
        await using var migrationScope = await extension.CreateMigrationScopeAsync(
            metadataOperation,
            tracker,
            cancellationToken);

        await migrationScope.CommitChangesAsync(new MigrationOperationDetail(
                pendingSource.MigrationId,
                pendingSource.DbVersion,
                metadataOperation,
                Path.GetDirectoryName(pendingSource.Context) ?? "/",
                Path.GetFileName(pendingSource.Context),
                agentContext.Agent,
                agentContext.Host,
                await fileSystem.CompressAsync(pendingSource.Context, cancellationToken),
                tracker,
                metadataFinal),
            cancellationToken);

        AnsiConsole.WriteLine();
        logger.LogInformation("{ok} Tracking metadata updated (applied {path}, operation={op})", 
            OkToken.Default,
            Path.GetFileName(pendingSource.Context),
            options.Mode);

        var currentVersion = await metadataContext.GetCurrentVersionAsync(cancellationToken);
        logger.LogInformation("{ok} Metadata reflects version {version}",
            OkToken.Default,
            currentVersion);
        
        return 0;
    }
}