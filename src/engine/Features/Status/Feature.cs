using DbForward.Extensions;
using DbForward.Logging;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Status;

public sealed class Feature(IDatabaseExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var result = await metadataContext.GetCurrentDetailAsync(cancellationToken);

        switch (result)
        {
            case null:
                logger.LogInformation("No current migration found");
                return 0;
            
            default:
                var displayResult = options.ShortSha
                    ? result with { Sha = result.Sha[^8..] }
                    : result;
                
                logger.LogInformation("{ok} current migration =\n{@detail}", 
                    new OkToken("Found"),
                    displayResult);
                return 0;
        }
    }
}