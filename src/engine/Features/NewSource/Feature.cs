using System.Diagnostics;
using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.NewSource;

public sealed class Feature(
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;
        
        var versionId = await extension.GetSourceReader().CreateVersionId();
        var templateName = $"{versionId}{extension.DefaultFileExtension}";
        var path = Path.Combine(options.BasePath.FullName, templateName);

        await using (var textWriter = fileSystem.CreateWriter(path))
        {
            await extension.WriteTemplateSource(textWriter,
                versionId,
                new Dictionary<string, string>(options.Metadata),
                cancellationToken);

            await textWriter.FlushAsync(cancellationToken);
        }

        logger.LogInformation("{ok} wrote new template file {path}",
            OkToken.Default,
            path);

        if (options.Edit)
        {
            LiveEditing.TryEditFile(path, logger);
        }

        return 0;
    }
}