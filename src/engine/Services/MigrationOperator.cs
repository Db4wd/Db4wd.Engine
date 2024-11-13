using System.Diagnostics;
using DbForward.Constants;
using DbForward.Models;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Services;

public interface IMigrationOperator
{
    /// <summary>
    /// Applies migration actions.
    /// </summary>
    /// <param name="parameters">Parameters that control the migration</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="MigrationResult"/></returns>
    Task<MigrationResult> ApplyAsync(MigrationParameters parameters, CancellationToken cancellationToken);
}

internal sealed class MigrationOperator(ILogger<MigrationOperator> logger) : IMigrationOperator
{
    /// <inheritdoc />
    public async Task<MigrationResult> ApplyAsync(MigrationParameters parameters, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        var elapsed = TimeSpan.Zero;
        var appliedSources = new List<SourceHeader>();
        var data = new OperationTracker();
        var currentVersion = parameters.CurrentVersion;

        foreach (var (source, iteration) in parameters.SourceTargets.Select((s,i) => (s,i + 1)))
        {
            timer.Restart();
            
            logger.LogInformation("Applying {source} ({id} of {count}, operation={op})...",
                source.Context,
                iteration,
                parameters.SourceTargets.Count,
                parameters.Operation);

            var response = await ApplySourceDirectivesAsync(parameters, source, data, cancellationToken);
            
            switch (response)
            {
                case OperationResponse.Successful:
                    logger.LogInformation("Source applied successfully ({ms}ms)", timer.ElapsedMilliseconds);
                    elapsed += timer.Elapsed;
                    appliedSources.Add(source);
                    currentVersion = source.DbVersion;
                    break;
                
                default:
                    logger.LogError("Failed to apply source (rollback mode={mode})", response);
                    return new MigrationResult(response, appliedSources.ToArray(), currentVersion, source);
            }
        }
        
        logger.LogInformation("Operation completed in ({ms:F0}ms)", elapsed.TotalMilliseconds);
        return new MigrationResult(OperationResponse.Successful, appliedSources.ToArray(), currentVersion);
    }

    private async Task<OperationResponse> ApplySourceDirectivesAsync(
        MigrationParameters parameters,
        SourceHeader source,
        OperationTracker tracker,
        CancellationToken cancellationToken)
    {
        var directives = await ReadDirectivesAsync(parameters, source, cancellationToken);
        
        logger.LogDebug("Read {count} directive(s) from source", directives.Count);

        if (directives.Count == 0)
        {
            logger.LogWarning("Source {source} has no directives for operation", source.Context);
            return OperationResponse.Successful;
        }

        await using var scope = await parameters.ScopeProvider(parameters.Operation, tracker, cancellationToken);
        var timer = Stopwatch.StartNew();

        foreach (var (directive, id) in directives.Select((i,d) => (i,d)))
        {
            timer.Restart();
            
            var response = await Operation.TryExecuteAsync(
                async () => await scope.ApplyDirectiveAsync(directive, tracker, cancellationToken),
                directive.Text,
                logger,
                parameters.StatementLogLevel);
            

            if (response != OperationResponse.Successful)
                return response;
            
            logger.LogDebug("Applied directive ({id} of {count}), took {ms}ms",
                id + 1,
                directives.Count,
                timer.ElapsedMilliseconds);
        }

        tracker.Increment("operator/elapsedMs", timer.ElapsedMilliseconds);
        tracker.Increment("operator/directiveCount", directives.Count);
        
        var compressionInfo = await parameters.FileSystem.CompressAsync(source.Context, cancellationToken);
        var migrationDetail = new MigrationOperationDetail(
            source.MigrationId,
            source.DbVersion,
            parameters.Operation,
            Path.GetDirectoryName(source.Context) ?? "/",
            Path.GetFileName(source.Context),
            parameters.AgentContext.Agent,
            parameters.AgentContext.Host,
            compressionInfo,
            tracker,
            source.Metadata.MergeReplace(parameters.Metadata));

        await scope.CommitChangesAsync(migrationDetail, cancellationToken);
        
        return OperationResponse.Successful;
    }

    private static async Task<IList<StatementSectionDirective>> ReadDirectivesAsync(
        MigrationParameters parameters,
        SourceHeader source,
        CancellationToken cancellationToken)
    {
        using var textReader = parameters.FileSystem.CreateReader(source.Context);
        var sourceReader = parameters.SourceReader;
        return parameters.Operation switch
        {
            SourceOperation.Migrate => await sourceReader.ReadMigrationDirectivesAsync(
                textReader,
                source.Context,
                parameters.Tokens,
                cancellationToken),
            _ => await sourceReader.ReadRollbackDirectivesAsync(
                textReader,
                source.Context,
                parameters.Tokens,
                cancellationToken)
        };
    }
}