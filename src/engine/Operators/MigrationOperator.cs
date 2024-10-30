using Db4Wd.Extensions;
using Db4Wd.Logging;
using Db4Wd.Services;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Operators;

public sealed class MigrationOperator(
    IExtension extension, 
    ISourceFileLoader sourceFileLoader,
    SourceAuditingOperator auditingOperator,
    ILogger<MigrationOperator> logger)
{
    public async Task<int> ExecuteAsync(IMigrationOperatorOptions options, CancellationToken cancellationToken)
    {
        if (await extension.InitializeAsync(cancellationToken) is not { IsInitialized: true } connectorProperties)
        {
            logger.LogMigrationsNotInitialized();
            return -1;
        }

        logger.LogConnectorPropertyHints(connectorProperties);

        var connector = connectorProperties.GetInstance();
        var sourceReader = connector.CreateSourceReader();
        var sourceFiles = await sourceFileLoader.GetSourceFilesAsync(sourceReader,
            options.BasePath,
            options.MatchPattern ?? $"**/*{extension.DefaultFileExtension}",
            cancellationToken);
        var entries = await connector.GetMigrationEntriesAsync(cancellationToken);

        await auditingOperator.AuditSourcesAsync(sourceFiles, entries, cancellationToken);

        var appliedIds = new HashSet<Guid>(entries.Select(entry => entry.Id));
        var pendingSources = sourceFiles
            .Where(source => !appliedIds.Contains(source.MigrationId))
            .OrderBy(source => source.DbVersion)
            .ToArray();

        if (options.TargetMigrationId != null &&
            pendingSources.All(source => source.MigrationId != options.TargetMigrationId))
        {
            logger.LogError("Target migration id {id} is in the pending input set", options.TargetMigrationId);
            return -1;
        }
        
        
        
        return 0;
    }
}