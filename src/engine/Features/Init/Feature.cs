using Db4Wd.Extensions;
using Db4Wd.Features.Shared;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Init;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : 
    SchemaManagementFeature<Options>(extension, logger)
{
    /// <inheritdoc />
    public override async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        var connectorInfo = await Extension.InitializeAsync(cancellationToken);
        var targetVersion = connectorInfo.InstalledVersion ?? connectorInfo.NewestVersion;

        return await HandleCoreAsync(connectorInfo, targetVersion, cancellationToken);
    }
}