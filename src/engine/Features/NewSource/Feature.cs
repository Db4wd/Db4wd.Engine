using DbForward.Extensions;
using DbForward.Logging;
using DbForward.Services;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.NewSource;

public sealed class Feature(IDatabaseExtension extension,
    IFileSystem fileSystem,
    ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (!await extension.CheckMigrationsInitializedAsync(logger, cancellationToken))
            return -1;
        
        await using var textWriter = fileSystem.CreateWriter(options.OutputPath.FullName);
        await extension.WriteTemplateSource(textWriter, 
            new Dictionary<string, string>(options.Metadata), 
            cancellationToken);
        
        logger.LogInformation("{ok} wrote new template file {path}",
            OkToken.Default,
            options.OutputPath.FullName);

        return 0;
    }
}