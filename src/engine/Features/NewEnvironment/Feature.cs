using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.NewEnvironment;

public sealed class Feature(
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        await using (var textWriter = fileSystem.CreateWriter(options.OutputPath.FullName, overwrite: true))
        {
            await extension.WriteTemplateEnvironmentFileAsync(textWriter,
                new Dictionary<string, string>(options.Properties, StringComparer.OrdinalIgnoreCase),
                cancellationToken);
            await textWriter.FlushAsync(cancellationToken);
        }

        logger.LogInformation("{ok} Wrote file {path}", OkToken.Default, options.OutputPath);

        if (options.Edit)
        {
            LiveEditing.TryEditFile(options.OutputPath.FullName, logger);
        }

        return 0;
    }
}