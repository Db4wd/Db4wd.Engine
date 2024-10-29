using Db4Wd.Extensions;
using Npgsql;

namespace Db4Wd.Postgres.Schema;

public interface IPostgresConnector : IMigrationConnector
{
    /// <summary>
    /// Gets the version.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Installs the connector management schema.
    /// </summary>
    /// <param name="connection">The connection instance.</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    Task InstallAsync(NpgsqlConnection connection, CancellationToken cancellationToken);
}