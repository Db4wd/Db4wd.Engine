using System.Diagnostics.CodeAnalysis;
using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema;

public sealed class SchemaManager(
    NpgConnectionFactory connectionFactory,
    IEnumerable<IPostgresConnector> connectors,
    ILogger<SchemaManager> logger)
{
    internal async Task<ConnectorProperties> GetConnectorPropertiesAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);

        var installedVersion = await VersionQuery.GetInstalledVersionAsync(connection);
        var managedInstance = GetConnectorInstance(installedVersion);
        var result = new ConnectorProperties(
            installedVersion,
            connectors.Select(impl => impl.Version).ToArray(),
            managedInstance);
        
        logger.LogDebug("Built connector properties {instance}", result);

        return result;
    }

    internal IEnumerable<Version> ConnectorVersions => connectors.Select(impl => impl.Version);
    
    internal async Task<ConnectorInstallation> UpdateManagementVersionAsync(
        ConnectorProperties connectorProperties, 
        Version version,
        CancellationToken cancellationToken)
    {
        if (!TryGetMigrationConnector(version, out var candidateConnector))
            return ConnectorInstallation.VersionNotSupported;

        if (candidateConnector.Version < connectorProperties.InstalledVersion)
            return ConnectorInstallation.LowerVersionNotSupported;

        if (candidateConnector.Version == connectorProperties.InstalledVersion)
            return ConnectorInstallation.NoActionTaken;

        var installCount = 0;
        
        logger.LogDebug("Validated requested management version {version}", version);

        await using var connection = await connectionFactory.CreateAsync(cancellationToken);

        foreach (var connector in connectors.OrderBy(impl => impl.Version))
        {
            if (connectorProperties.InstalledVersion >= connector.Version)
            {
                logger.LogDebug("Skipped older version {version}", connector.Version);
                continue;
            }

            logger.LogDebug("Installing management version {version}...", connector.Version);

            try
            {
                await connector.InstallAsync(connection, cancellationToken);
            }
            catch (NpgsqlException exception) when (exception.SqlState == "23505")
            {
                return ConnectorInstallation.LockRejected;
            }
            catch (NpgsqlException exception) when (exception.InnerException is TimeoutException)
            {
                return ConnectorInstallation.LockAcquisitionTimeout;
            }

            installCount++;
            
            logger.LogDebug("Manager version {version} installed", connector.Version);

            if (connector.Version == version)
                break;
        }

        return (connectorProperties, installCount) switch
        {
            { connectorProperties.IsInitialized: false, installCount: > 0 } => ConnectorInstallation.Initialized,
            { connectorProperties.IsInitialized: true, installCount: > 0 } => ConnectorInstallation.Updated,
            _ => ConnectorInstallation.NoActionTaken
        };
    }

    private IPostgresConnector? GetConnectorInstance(Version? version)
    {
        return version == null
            ? null
            : connectors.FirstOrDefault(impl => impl.Version == version) ?? throw IncompatibleVersion(version);
    }

    private bool TryGetMigrationConnector(Version? version, [NotNullWhen(true)] out IPostgresConnector? connector)
    {
        return (connector = connectors.FirstOrDefault(impl => impl.Version == version)) != null;
    }

    private static NotSupportedException IncompatibleVersion(Version version) => new(
        $"Migration schema installed on target database {version} is not compatible with this version of pg4.");
}