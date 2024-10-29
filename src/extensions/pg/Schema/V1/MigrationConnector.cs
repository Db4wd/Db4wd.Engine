using Db4Wd.Engine;
using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema.V1;

public sealed class MigrationConnector(
   NpgConnectionFactory connectionFactory,
   AgentContext agentContext,
   ILogger<MigrationConnector> logger) : IPostgresConnector
{
    /// <inheritdoc />
    public Version Version => new(1, 0, 0);

    /// <inheritdoc />
    public async Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
       await Install.ExecuteAsync(connection, logger, cancellationToken);
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