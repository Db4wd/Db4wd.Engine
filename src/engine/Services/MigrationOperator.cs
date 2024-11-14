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

internal sealed class MigrationOperator(TimerProvider timerProvider, ILogger<MigrationOperator> logger) : IMigrationOperator
{
    /// <inheritdoc />
    public async Task<MigrationResult> ApplyAsync(MigrationParameters parameters, CancellationToken cancellationToken)
    {
        var totalElapsed = TimeSpan.Zero;
        var appliedSources = new List<SourceHeader>();
        var timer = timerProvider();

        foreach (var (source, iteration) in parameters.SourceTargets.Select((s,i) => (s,i + 1)))
        {
            timer.Restart();
            
            logger.LogInformation("Applying {source} ({id} of {count}, operation={op})...",
                source.Context.TruncateEnd(50),
                iteration,
                parameters.SourceTargets.Count,
                parameters.Operation);

            var response = await ApplySourceDirectivesAsync(parameters, source, cancellationToken);
            var sourceElapsed = timer.Elapsed;
            
            switch (response)
            {
                case OperationResponse.Successful:
                    logger.LogInformation("Source applied successfully ({ms:F0}ms)", sourceElapsed.TotalMilliseconds);
                    totalElapsed += sourceElapsed;
                    appliedSources.Add(source);
                    break;
                
                default:
                    logger.LogError("Failed to apply source (rollback mode={mode})", response);
                    return new MigrationResult(response, appliedSources.ToArray(), source);
            }
        }
        
        logger.LogInformation("Operation completed in ({ms:F0}ms)", totalElapsed.TotalMilliseconds);
        return new MigrationResult(OperationResponse.Successful, appliedSources.ToArray());
    }

    private async Task<OperationResponse> ApplySourceDirectivesAsync(
        MigrationParameters parameters,
        SourceHeader source,
        CancellationToken cancellationToken)
    {
        var directives = await ReadDirectivesAsync(parameters, source, cancellationToken);
        var tracker = new OperationTracker();
        
        logger.LogDebug("Read {count} directive(s) from source", directives.Count);

        if (directives.Count == 0)
        {
            logger.LogWarning("Source {source} has no directives for operation", source.Context.TruncateEnd(50));
            return OperationResponse.Successful;
        }

        await using var scope = await parameters.ScopeProvider(parameters.Operation, tracker, cancellationToken);
        var timer = timerProvider();

        foreach (var (directive, id) in directives.Select((i,d) => (i,d)))
        {
            timer.Restart();
            
            var response = await Operation.TryExecuteAsync(
                async () => await scope.ApplyDirectiveAsync(directive, cancellationToken),
                directive.Text,
                logger,
                parameters.StatementLogLevel);
            
            if (response != OperationResponse.Successful)
                return response;
            
            logger.LogDebug("Applied directive ({id} of {count}), took {ms:F0}ms",
                id + 1,
                directives.Count,
                timer.Elapsed.TotalMilliseconds);
        }

        tracker.Increment("operator/elapsedMs", (long)timer.Elapsed.TotalMilliseconds);
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