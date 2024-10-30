using Db4Wd.Services;

namespace Db4Wd.Features.Init;

public sealed class Feature(SchemaManagementOperator managementOperator) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        return await managementOperator.ApplyVersionAsync(
            properties => properties.InstalledVersion ?? properties.NewestVersion,
            cancellationToken);
    }
}