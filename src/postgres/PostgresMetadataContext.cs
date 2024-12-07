using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;

namespace DbForward.Postgres;

internal sealed class PostgresMetadataContext(
    IConnectionFactory connectionFactory,
    IAgentContext agentContext) : IMetadataContext
{
    /// <inheritdoc />
    public async Task<IList<MigrationEntry>> GetEntriesAsync(
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetMigrationEntries.QueryAsync(connection, agentContext.TimeZoneOffset);
    }

    /// <inheritdoc />
    public async Task<MigrationDetail?> GetDetailAsync(Guid migrationId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetMigrationDetail.QueryAsync(connection, migrationId, agentContext.TimeZoneOffset);
    }

    /// <inheritdoc />
    public async Task<MigrationDetail?> GetCurrentDetailAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetMigrationDetail.QueryCurrentAsync(connection, agentContext.TimeZoneOffset);
    }

    /// <inheritdoc />
    public async Task<string?> GetCurrentVersionAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetCurrentVersion.QueryAsync(connection);
    }

    /// <inheritdoc />
    public async Task<CompressedBlobInfo?> GetBlobAsync(Guid migrationId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetBlob.QueryAsync(connection, migrationId);
    }

    /// <inheritdoc />
    public async Task<IList<MigrationHistory>> GetLogEntriesAsync(
        LogSearchParameters parameters, 
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        return await Queries.GetLogs.QueryAsync(connection, parameters, agentContext.TimeZoneOffset);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}