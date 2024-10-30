using Db4Wd.Engine;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema.V1;

public sealed class MigrationConnector(
   NpgConnectionFactory connectionFactory,
   AgentContext agentContext,
   ILogger<MigrationConnector> logger) : PostgresConnector(connectionFactory, agentContext, logger, new Version(1,0,0))
{
   /// <inheritdoc />
   public override async Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
   {
      await Install.ExecuteAsync(connection, logger, cancellationToken);
   }
}