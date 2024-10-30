using Db4Wd.Services;

namespace Db4Wd.Features.UpdateConnector;

public sealed class Feature(SchemaManagementOperator managementOperator) : IFeature<Options> 
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        return await managementOperator.ApplyVersionAsync(
            properties => options.VersionId ?? properties.NewestVersion,
            cancellationToken);
    }
}