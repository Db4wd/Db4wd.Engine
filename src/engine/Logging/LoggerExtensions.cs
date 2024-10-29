using Microsoft.Extensions.Logging;

namespace Db4Wd.Logging;

public static class LoggerExtensions
{
    public static void LogMigrationsNotInitialized(this ILogger logger)
    {
        logger.LogError("Migrations have not been initialized on the target database.");
    }
}