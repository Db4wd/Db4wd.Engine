using DbForward.Extensions;
using DbForward.Logging;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Detail;

public sealed class Feature(IDatabaseExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var result = await metadataContext.GetDetailAsync(options.Id, cancellationToken);

        switch (result)
        {
            case null:
                logger.LogError("Migration id not found");
                break;
            
            default:
                var displayResult = options.ShortSha
                    ? result with { Sha = result.Sha[^8..] }
                    : result;
                logger.LogInformation("{found} migration = {@migration}", new OkToken("Found"), displayResult);
                break;
        }

        return 0;
    }
}