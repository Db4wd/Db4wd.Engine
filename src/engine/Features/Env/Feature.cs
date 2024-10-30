using Db4Wd.Extensions;
using Db4Wd.Logging;
using Db4Wd.Utilities;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Features.Env;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        var basePath = options.BasePath ?? new DirectoryInfo(Path.Combine(Common.EnvDataPath, extension.RootContext));
        Directory.CreateDirectory(basePath.FullName);

        var fullPath = Path.Combine(basePath.FullName, $"{options.Name}.env");

        if (File.Exists(fullPath) && !options.Overwrite)
        {
            logger.LogWarning("Will not overwrite {path} (use {switch} to force)",
                fullPath,
                "--overwrite");
            return -1;
        }

        await using var textWriter = new StreamWriter(new FileStream(fullPath, FileMode.Create));
        await extension.WriteEnvironmentTemplateAsync(textWriter, cancellationToken);
        await textWriter.FlushAsync(cancellationToken);
        
        logger.LogInformation("{ok} Wrote new environment file: {path}", OkToken.Default, fullPath);
        return 0;
    }
}