using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Locks.Delete;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (await extension.InitializeAsync(cancellationToken) is not { IsInitialized: true } connectorProperties)
        {
            logger.LogMigrationsNotInitialized();
            return -1;
        }

        var result = await connectorProperties.GetInstance().TryReleaseLockAsync(
            options.LockId, 
            cancellationToken);

        switch (result)
        {
            case LockResult.Failed:
                logger.LogError(
                    "Failed to release lock. This typically indicates that it is not stale and locked in " +
                    "a pending transaction.");
                return -1;
            
            case LockResult.NoLocksAffected when options.LockId.HasValue:
                logger.LogError("Lock id not found (it may have already been released by the owning agent).");
                return -1;
            
            case LockResult.NoLocksAffected:
                logger.LogInformation("No locks found to release. It is possible one or more lock(s) were acquired " +
                                      "in a {serializable} transaction.", "serializable");
                return -1;
            
            default:
                logger.LogInformation("{ok} Lock(s) released successfully", OkToken.Default);
                return 0;
        }
    }
}