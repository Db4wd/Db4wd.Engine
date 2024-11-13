using System.Text;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Models;
using DbForward.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DbForward.Features.Source;

public sealed class Feature(IDatabaseExtension extension,
    IFileSystem fileSystem,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;

        await using var metadataContext = await extension.CreateMetadataContextAsync(cancellationToken);
        var blobInfo = await metadataContext.GetBlobAsync(options.MigrationId, cancellationToken);

        if (blobInfo == null)
        {
            logger.LogError("Migration id not found");
            return 0;
        }

        return options.OutputPath != null
            ? await WriteToFileAsync(blobInfo, options.OutputPath, cancellationToken)
            : await WriteToConsole(blobInfo, cancellationToken);
    }

    private async Task<int> WriteToFileAsync(
        CompressedBlobInfo blobInfo, 
        FileInfo path,
        CancellationToken cancellationToken)
    {
        await using var textWriter = fileSystem.CreateWriter(path.FullName, overwrite: true);
        await fileSystem.DecompressAsync(textWriter, blobInfo, cancellationToken);
        logger.LogInformation("Wrote file {path}", path);
        return 0;
    }

    private async Task<int> WriteToConsole(CompressedBlobInfo blobInfo, CancellationToken cancellationToken)
    {
        await using var memoryStream = new MemoryStream();
        await using var textWriter = new StreamWriter(memoryStream);
        await fileSystem.DecompressAsync(textWriter, blobInfo, cancellationToken);
        var foreground = AnsiConsole.Foreground;

        try
        {
            logger.LogInformation("{ok} Displaying content of {path} (sha={sha}):",
                OkToken.Default,
                Path.GetFileName(blobInfo.Path),
                blobInfo.Sha[^8..]);
            AnsiConsole.Foreground = Color.Grey58;
            AnsiConsole.WriteLine(Encoding.UTF8.GetString(memoryStream.ToArray()));
            return 0;
        }
        finally
        {
            AnsiConsole.Foreground = foreground;
        }
    }
}