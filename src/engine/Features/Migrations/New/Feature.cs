using System.Text;
using Db4Wd.Extensions;
using Db4Wd.Logging;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Db4Wd.Features.Migrations.New;

public sealed class Feature(IExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (await extension.InitializeAsync(cancellationToken) is not { IsInitialized: true } connectorProperties)
        {
            logger.LogMigrationsNotInitialized();
            return -1;
        }

        Stream stream = options.Path != null
            ? File.Open(options.Path.FullName, FileMode.CreateNew)
            : new MemoryStream();

        await using var writer = new StreamWriter(stream);

        var connector = connectorProperties.GetInstance();
        await connector.WriteTemplateMigrationAsync(writer, options.Metadata, cancellationToken);
        await writer.FlushAsync(cancellationToken);

        switch (stream)
        {
            case FileStream:
                logger.LogInformation("{ok} Wrote new migration file {path}", OkToken.Default, options.Path);
                break;
            
            case MemoryStream memoryStream:
                var text = Encoding.UTF8.GetString(memoryStream.ToArray());
                AnsiConsole.MarkupLineInterpolated($"[grey46]{text}[/]");
                break;
        }

        return 0;
    }
}