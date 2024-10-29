using Db4Wd.Extensions;
using Db4Wd.Features.Shared;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.UpdateConnector;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : 
    SchemaManagementFeature<Options>(extension, logger)
{
    /// <inheritdoc />
    public override async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (await Extension.InitializeAsync(cancellationToken) is not { IsInitialized: true } connectorProperties)
        {
            logger.LogMigrationsNotInitialized();
            return -1;
        }

        var targetVersion = options.VersionId ?? connectorProperties.NewestVersion;

        return await HandleCoreAsync(connectorProperties, targetVersion, cancellationToken);
    }
}