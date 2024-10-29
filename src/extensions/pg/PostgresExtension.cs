using Db4Wd.Extensions;
using Db4Wd.Postgres.Schema;

namespace Db4Wd.Postgres;

public sealed class PostgresExtension(SchemaManager schemaManager) : IExtension
{
    /// <inheritdoc />
    public string RootContext => "pg4";

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Properties { get; } = new Dictionary<string, string>
    {
        ["driver"] = "Npgsql",
        ["npgsql version"] = "8.0.5",
        ["supported schema"] = schemaManager.ConnectorVersions.Max()!.ToString()
    };

    /// <inheritdoc />
    public async Task<ConnectorProperties> InitializeAsync(CancellationToken cancellationToken)
    {
        return await schemaManager.GetConnectorPropertiesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ConnectorInstallation> UpdateManagementVersionAsync(
        ConnectorProperties connectorProperties, 
        Version version,
        CancellationToken cancellationToken)
    {
        return await schemaManager.UpdateManagementVersionAsync(connectorProperties, version, cancellationToken);
    }
}