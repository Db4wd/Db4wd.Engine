using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Services;

public sealed class MigrationOperator(
    IExtension extension, 
    ILogger<MigrationOperator> logger)
{
    public async Task<int> ExecuteAsync(IMigrationOperatorOptions options, CancellationToken cancellationToken)
    {
        if (await extension.InitializeAsync(cancellationToken) is not { IsInitialized: true } connectorProperties)
        {
            logger.LogMigrationsNotInitialized();
            return -1;
        }

        LogConnectorPropertyHints(connectorProperties);

        return 0;
    }

    private void LogConnectorPropertyHints(ConnectorProperties connectorProperties)
    {
        if (!connectorProperties.UpdateVersions.Any())
            return;
        
        logger.LogInformation("Migration manager can be updated to version {version}",
            connectorProperties.NewestVersion);
    }
}