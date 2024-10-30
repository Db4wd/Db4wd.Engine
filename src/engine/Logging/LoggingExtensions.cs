using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Logging;

public static class LoggingExtensions
{
    public static void LogMigrationsNotInitialized(this ILogger logger)
    {
        logger.LogError("Migrations have not been initialized on the target database.");
    }

    public static void LogConnectorPropertyHints(this ILogger logger, ConnectorProperties connectorProperties)
    {
        if (!connectorProperties.IsUpdateable)
            return;
        
        logger.LogInformation("Connector can be updated to version {version}",
            connectorProperties.NewestVersion);
    }
}