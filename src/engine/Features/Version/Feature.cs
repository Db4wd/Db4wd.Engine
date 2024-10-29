using Db4Wd.Engine;
using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Version;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        logger.LogInformation("Engine runtime: {version}", EngineHost.CurrentVersion);

        foreach (var (key, value) in extension.Properties)
        {
            logger.LogInformation("{key}: {value}", new KeyToken(key), value);
        }
        
        return Task.FromResult(0);
    }
}