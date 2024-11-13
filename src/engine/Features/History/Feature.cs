using DbForward.Extensions;
using DbForward.Logging;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.History;

public sealed class Feature(IDatabaseExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var results = await metadataContext.GetHistoryAsync(options.Id, options.Limit, cancellationToken);

        switch (results.Count)
        {
            case 0:
                logger.LogError("Migration id not found");
                break;
            
            default:
                logger.LogInformation("{found} history = {@json}", new OkToken("Found"), results);
                break;
        }

        return 0;
    }
}