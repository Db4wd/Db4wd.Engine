using DbForward.Extensions;
using DbForward.Logging;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DbForward.Features.Initialize;

public sealed class Feature(IDatabaseExtension extension, ILogger<Feature> logger) : IFeature<Options>
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
    {
        if (await extension.IsSchemaInitializedAsync(cancellationToken))
        {
            logger.LogInformation("Migrations already initialized on the target database.");
            return 0;
        }
        
        logger.LogInformation("Initializing migrations on target");
        var result = await extension.InitializeMigrationsAsync(cancellationToken);

        switch (result)
        {
            case SchemaInitialization.NotRequired:
                logger.LogInformation("Migrations have already been enabled on target database");
                return 0;
            
            case SchemaInitialization.Initialized:
                AnsiConsole.WriteLine();
                logger.LogInformation("{ok} Migrations initialized on the target database", OkToken.Default);
                return 0;
        }

        return -1;
    }
}