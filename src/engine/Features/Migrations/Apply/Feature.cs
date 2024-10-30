using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Migrations.Apply;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        logger.LogInformation("Migrating...");
        return 0;
    }
}