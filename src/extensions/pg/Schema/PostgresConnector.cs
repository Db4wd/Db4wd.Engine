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
    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<LockInfo>> GetLocksAsync(CancellationToken cancellationToken)
    {
        return await Shared.LockQuery.GetLocksAsync(connectionFactory, agentContext, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LockResult> TryReleaseLockAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await Shared.DeleteLocks.ExecuteAsync(connectionFactory, id, logger, cancellationToken);
    }

    /// <inheritdoc />
    public IMigrationSourceReader CreateSourceReader() => new PostgresSourceReader();

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MigrationEntry>> GetMigrationEntriesAsync(
        CancellationToken cancellationToken)
    {
        return await Shared.MigrationEntryQuery.QueryAsync(
            connectionFactory,
            agentContext,
            cancellationToken);
    }

    /// <inheritdoc />
    public Version Version => version;

    /// <inheritdoc />
    public abstract Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken);
}