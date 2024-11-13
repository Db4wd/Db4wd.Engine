using Microsoft.Extensions.Logging;

namespace DbForward.Features.About;

public sealed class Feature(ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        logger.LogInformation("DBForward V1.0.3");
        
        return Task.FromResult(0);
    }
}