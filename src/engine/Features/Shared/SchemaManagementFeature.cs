using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Shared;

public abstract class SchemaManagementFeature<TOptions>(IExtension extension, ILogger logger) 
    : IFeature<TOptions> where TOptions : ConnectionOptions
{
    protected IExtension Extension => extension;
    
    /// <inheritdoc />
    public abstract Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);

    protected async Task<int> HandleCoreAsync(
        ConnectorProperties connectorProperties,
        System.Version targetVersion,
        CancellationToken cancellationToken)
    {
        var result = await extension.UpdateManagementVersionAsync(connectorProperties, targetVersion, cancellationToken);

        switch (result)
        {
            case ConnectorInstallation.Initialized:
                logger.LogInformation("{ok} Migrations now enabled on the target database", OkToken.Default);
                break;
            
            case ConnectorInstallation.Updated:
                logger.LogInformation("{ok} Management version updated to {version}",
                    OkToken.Default,
                    targetVersion);
                return 0;
            
            case ConnectorInstallation.NoActionTaken:
                logger.LogInformation("Migrations already enabled on the target database with version {version}",
                    targetVersion);
                break;
            
            case ConnectorInstallation.LockRejected:
                logger.LogWarning("Management update operation is already in progress on another machine " +
                                  "(use {command} for details).",
                    $"{extension.RootContext} locks --list");
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