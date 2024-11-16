using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Models;
using DbForward.Services;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Operations;

public abstract class OperationCoreFeature<TOptions>(
    SourceOperation operation,
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    IAgentContext agentContext,
    ISourceFileManager sourceManager,
    ISourceAuditor sourceAuditor,
    IMigrationOperator migrationOperator,
    ILogger logger) : IFeature<TOptions> where TOptions : MigrationOptions
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        var entities = $"{operation.ToString().ToLower()}s";

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);

        // Gather data
        var appliedEntries = await metadataContext.GetEntriesAsync(cancellationToken);
        logger.LogDebug("Loaded {count} migration entry/entries", appliedEntries.Count);

        var sourceReader = extension.GetSourceReader();
        var tokens = new Dictionary<string, string>(options.Tokens);
        var sources = await sourceManager.GetSourceHeadersInPathAsync(
            options.BasePath,
            extension.ResolveSearchPattern(options.SearchPattern, logger),
            sourceReader,
            tokens,
            cancellationToken);

        logger.LogDebug("Matched {count} source(s)", sources.Count);

        // Build target sources (those which will be applied)
        var versionComparer = sourceReader.GetVersionComparer();
        var appliedIdentifiers = new HashSet<Guid>(appliedEntries.Select(entry => entry.MigrationId));
        var sourceTargets = GetSourceTargets(options, sources, appliedIdentifiers, versionComparer).ToArray();
        
        logger.LogDebug("Identified {count} pending source(s)", sourceTargets.Length);

        if (sourceTargets.Length == 0)
        {
            logger.LogInformation("There are no pending {entities} to apply; database is up-to-date.", entities);
            return 0;
        }
        
        // Audit
        var auditContext = new AuditingContext(
            operation,
            sources,
            sourceTargets,
            appliedEntries,
            options.TargetId,
            options.TargetDbVersion,
            versionComparer);

        if (!await sourceAuditor.AuditAsync(auditContext, cancellationToken))
        {
            logger.LogError("Source audit failed; abandoning migrations.");
            return -1;
        }
        
        logger.LogInformation("Sources validated");

        var currentVersion = await metadataContext.GetCurrentVersionAsync(cancellationToken);
        logger.LogDebug("Current version returned {version}", currentVersion);
        
        var migrationParameters = new MigrationParameters(
            operation,
            agentContext,
            fileSystem,
            sourceReader,
            extension.CreateMigrationScopeAsync,
            currentVersion,
            sourceTargets,
            tokens,
            GetOptionalMetadata(options),
            options.StatementLogLevel);
        
        var result = await migrationOperator.ApplyAsync(migrationParameters, cancellationToken);
        var updatedVersion = await metadataContext.GetCurrentVersionAsync(cancellationToken);
        var (preVersion, newVersion) = (currentVersion ?? "pristine", updatedVersion ?? "pristine");
        
        switch (result)
        {
            case { Response: OperationResponse.Successful }:
                logger.LogInformation("{ok} {op} successful, target updated from {pre} -> {new}",
                    OkToken.Default,
                    operation,
                    preVersion,
                    newVersion);
                return 0;
                
            case not null when preVersion == newVersion:
                logger.LogError("{op} failed; target is still at version {version}", operation, preVersion);
                break;
            
            default:
                logger.LogError("{op} failed; target updated from {pre} -> {new} prior to error",
                    operation,
                    preVersion,
                    newVersion);
                break;
        }

        if (operation == SourceOperation.Rollback)
            return -1;

        if (result!.Response == OperationResponse.Aborted)
        {
            logger.LogInformation("No rollback actions will be taken (extension strategy = {strategy})",
                "abort");
            return -1;
        }

        if (result.FailedSource == null)
        {
            logger.LogError("The failed source was not reported by the migration operation");
            return -1;
        }

        var rollbackParameters = migrationParameters with
        {
            Operation = SourceOperation.Rollback,
            SourceTargets = [result.FailedSource]
        };
        
        await migrationOperator.ApplyAsync(rollbackParameters, cancellationToken);
        logger.LogInformation("Rollback completed");
        return -1;
    }

    protected abstract Dictionary<string, string> GetOptionalMetadata(TOptions options);
    
    protected abstract IEnumerable<SourceHeader> GetSourceTargets(
        TOptions options,
        IList<SourceHeader> sources, 
        HashSet<Guid> appliedEntryIds,
        IDbVersionComparer versionComparer);
}