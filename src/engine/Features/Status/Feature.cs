using System.Collections.ObjectModel;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DbForward.Features.Status;

public sealed class Feature(IDatabaseExtension extension,
    ISourceFileManager sourceManager,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;
        
        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var detail = await metadataContext.GetCurrentDetailAsync(cancellationToken);
        var entries = await metadataContext.GetEntriesAsync(cancellationToken);
        var sourceReader = extension.GetSourceReader();
        var versionComparer = extension.GetDbVersionComparer();
        var sources = await sourceManager.GetSourceHeadersInPathAsync(
            options.BasePath,
            extension.ResolveSearchPattern(options.SearchPattern, logger),
            sourceReader,
            ReadOnlyDictionary<string, string>.Empty,
            cancellationToken);
        var currentVersion = entries.MaxBy(entry => entry.DbVersion, versionComparer)?.DbVersion;
        var pendingSources = sources
            .Where(source => versionComparer.Compare(source.DbVersion, currentVersion) > 0)
            .OrderBy(source => source.DbVersion, versionComparer)
            .ToArray();
        
        switch (detail)
        {
            case null:
                logger.LogInformation("No current migration found");
                break;
            
            default:
                var displayResult = options.ShortSha
                    ? detail with { Sha = detail.Sha[^8..] }
                    : detail;

                logger.LogInformation("{ok} current migration =\n{@detail}",
                    new OkToken("Found"),
                    detail);
                break;
        }

        if (pendingSources.Length == 0)
            return 0;

        AnsiConsole.WriteLine();
        
        foreach (var source in pendingSources)
        {
            logger.LogInformation("Pending: {source}", source.Context.TruncateEnd(75));
        }
        
        return 0;
    }
}