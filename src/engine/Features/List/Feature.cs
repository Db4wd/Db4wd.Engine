using DbForward.Extensions;
using DbForward.Logging;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DbForward.Features.List;

public sealed class Feature(IDatabaseExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var results = (await metadataContext.GetEntriesAsync(cancellationToken))
            .OrderByDescending(entry => entry.DateApplied)
            .Take(options.Limit)
            .ToArray();

        if (results.Length == 0)
        {
            logger.LogInformation("No migrations have been applied; target is in {pristine} state",
                "pristine");
            return 0;
        }

        var foreground = AnsiConsole.Foreground;
        try
        {
            AnsiConsole.WriteLine();
            
            var maxFileLength = results.Max(result => result.SourceFile.Length) + 2;
            var formatString = $"{{0,-{maxFileLength}}}{{1,-23}}{{2, -10}}{{3}}";
            var header = string.Format(formatString, "File", "Date applied", "Sha", "Id");
            var separator = new string('-', maxFileLength + 69);
            logger.LogInformation("{header}", new OkToken(header));
            logger.LogInformation("{header}", new OkToken(separator));
            
            
            foreach (var entry in results)
            {
                var detail = string.Format(formatString,
                    entry.SourceFile,
                    entry.DateApplied,
                    entry.Sha[^8..],
                    entry.MigrationId);
                logger.LogInformation("{detail}", detail);
            }

            return 0;
        }
        finally
        {
            AnsiConsole.Foreground = foreground;
        }
    }
}