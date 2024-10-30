using Dapper;
using Db4Wd.Engine;
using Db4Wd.Extensions;
using Db4Wd.Models;
using Db4Wd.Utilities;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema;

public abstract class PostgresConnector(
    NpgConnectionFactory connectionFactory,
    AgentContext agentContext,
    ILogger logger,
    Version version) : IPostgresConnector
{
    protected readonly NpgConnectionFactory ConnectionFactory = connectionFactory;
    protected readonly AgentContext AgentContext = agentContext;
    
    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<LockInfo>> GetLocksAsync(CancellationToken cancellationToken)
    {
        return await Shared.LockQuery.GetLocksAsync(ConnectionFactory, AgentContext, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<LockResult> TryReleaseLockAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await Shared.DeleteLocks.ExecuteAsync(ConnectionFactory, id, logger, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IMigrationSourceReader CreateSourceReader() => new PostgresSourceReader();

    /// <inheritdoc />
    public async Task WriteTemplateMigrationAsync(
        TextWriter writer, 
        KeyValuePair<string, string>[] metadata, 
        CancellationToken cancellationToken)
    {
        await Shared.WriteTemplate.WriteAsync(
            ConnectionFactory,
            AgentContext,
            writer,
            metadata,
            cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<MigrationEntry[]> GetMigrationEntriesAsync(
        CancellationToken cancellationToken)
    {
        return await Shared.MigrationEntryQuery.QueryAsync(
            ConnectionFactory,
            AgentContext,
            cancellationToken);
    }

    /// <inheritdoc />
    public Version Version => version;

    /// <inheritdoc />
    public abstract Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken);
}