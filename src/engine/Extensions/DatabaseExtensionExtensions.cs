using Microsoft.Extensions.Logging;

namespace DbForward.Extensions;

internal static class DatabaseExtensionExtensions
{
    internal static async Task<bool> CheckMigrationsInitializedAsync(
        this IDatabaseExtension extension,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (await extension.IsSchemaInitializedAsync(cancellationToken))
            return true;
        
        logger.LogError("Migrations have not been initialized on the target database.");
        return false;
    }

    internal static string ResolveSearchPattern(
        this IDatabaseExtension extension,
        string? pattern,
        ILogger logger)
    {
        if (!string.IsNullOrWhiteSpace(pattern))
            return pattern;

        pattern = $"**/*{extension.DefaultFileExtension}";
        logger.LogInformation("Using default search pattern {pattern}", pattern);
        return pattern;
    }
}