using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Locks.List;

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

        var locks = await connectorProperties.GetInstance().GetLocksAsync(cancellationToken);

        if (locks.Count == 0)
        {
            logger.LogInformation("There are no visible locks in the target database.");
            logger.LogInformation("If you were notified of contention, it's possible that the lock has been released or is " +
                                  "was acquired in a {serializable} transaction.", "serializable");
            return 0;
        }
        
        logger.LogInformation("Locks = {@locks}", locks);
        return 0;
    }
}