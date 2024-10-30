using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Migrations.Rollback;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}