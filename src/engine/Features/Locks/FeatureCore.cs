using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Locks;

public static class FeatureCore
{
    public static int LogUpdateResult(ILogger logger, string rootContext, ConnectorInstallation result)
    {
        switch (result)
        {
            case ConnectorInstallation.Initialized:
                logger.LogInformation("{ok} Migrations now enabled on the target database", OkToken.Default);
                break;
            
            case ConnectorInstallation.NoActionTaken:
                logger.LogInformation("Migrations already enabled on the target database");
                break;
            
            case ConnectorInstallation.LockRejected:
                logger.LogWarning("Management update operation is already in progress on another machine " +
                                  "(use {command} for details).",
                    $"{rootContext} locks --list");
                return -1;
            
            case ConnectorInstallation.LockAcquisitionTimeout:
                logger.LogWarning("Could not acquire management update lock {error}. It's possible " +
                                  "migrations are already in progress on another machine.",
                    "(operation timed out)");
                return -1;
        }

        return 0;
    }
}