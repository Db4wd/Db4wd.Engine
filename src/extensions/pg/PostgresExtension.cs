using System.Text.Json;
using Db4Wd.Extensions;
using Db4Wd.Postgres.Schema;

namespace Db4Wd.Postgres;

public sealed class PostgresExtension(SchemaManager schemaManager) : IExtension
{
    private readonly JsonSerializerOptions _environmentFileJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal const string ApplicationId = "pg4";

    /// <inheritdoc />
    public string RootContext => ApplicationId;

    /// <inheritdoc />
    public string DefaultFileExtension => ".sql";

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

    /// <inheritdoc />
    public async Task WriteEnvironmentTemplateAsync(TextWriter writer, CancellationToken cancellationToken)
    {
        var template = new
        {
            host = "localhost",
            port = "5432",
            database = "db4wd",
            username = "postgres",
            password = "(secret)",
            timeout = "30",
            commandTimeout = "30"
        };

        var json = JsonSerializer.Serialize(template, _environmentFileJsonOptions);
        await writer.WriteLineAsync(json);
    }
}