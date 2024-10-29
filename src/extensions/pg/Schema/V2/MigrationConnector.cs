using System.Data;
using Dapper;
using Db4Wd.Engine;
using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema.V2;

public class MigrationConnector(
    NpgConnectionFactory connectionFactory, 
    AgentContext agentContext,
    ILogger<MigrationConnector> logger) : IPostgresConnector
{
    /// <inheritdoc />
    public Version Version => new("1.0.1");

    /// <inheritdoc />
    public async Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        await using var transaction = await connection.BeginTransactionAsync(
            IsolationLevel.Serializable, 
            cancellationToken);

        var lockId = Guid.NewGuid();

        await connection.ExecuteAsync(
            $"""
            insert into {Constants.SchemaName}.locks(leasetype, lockid, dateacquired, agent, host)
            values(@lease, @lockId, CURRENT_TIMESTAMP, @agent, @host);
            
            insert into {Constants.SchemaName}.versions(major, minor, revision, dateapplied)
            values(1, 0, 1, CURRENT_TIMESTAMP);
            
            delete from {Constants.SchemaName}.locks
            where lockid = @lockId;
            """,
            new
            {
                lease = Constants.SchemaUpdateLease,
                lockId,
                agent = agentContext.Agent,
                host = agentContext.Host
            },
            transaction,
            commandTimeout: 5);

        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LockInfo>> GetLocksAsync(CancellationToken cancellationToken)
    {
        return await Shared.LockQuery.GetLocksAsync(connectionFactory, agentContext, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<LockResult> TryReleaseLockAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await Shared.DeleteLocks.ExecuteAsync(connectionFactory, id, logger, cancellationToken);
    }
}