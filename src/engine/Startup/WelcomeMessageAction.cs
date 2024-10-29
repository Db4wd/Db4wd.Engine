using Db4Wd.Engine;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Startup;

public sealed class WelcomeMessageAction(ILogger<WelcomeMessageAction> logger) : IStartupAction
{
    /// <inheritdoc />
    public int Priority => -1;

    /// <inheritdoc />
    public Task PerformAsync()
    {
        if (!TryDisplayMessage(EngineHost.CurrentVersion))
            return Task.CompletedTask;
        
        logger.LogInformation("DB4wd (database-forward) {version}", EngineHost.CurrentVersion);
        logger.LogInformation("Copyright (C) 2024 db4wd contributors");
        logger.LogInformation("Visit {url}", new Uri("https://gihtub.com/db4wd"));
        Console.WriteLine();

        return Task.CompletedTask;
    }

    private static bool TryDisplayMessage(Version version)
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".db4wd",
            "messages");
        
        var filePath = Path.Combine(path, $"welcome_{version}.txt");

        if (Path.Exists(filePath))
            return false;

        Directory.CreateDirectory(path);
        File.WriteAllText(filePath, $"displayed={DateTimeOffset.UtcNow}");
        return true;
    }
}