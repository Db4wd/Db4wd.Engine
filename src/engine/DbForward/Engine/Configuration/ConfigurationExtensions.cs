using DbForward.Engine.Models;
using DbForward.Engine.Options;
using Microsoft.Extensions.Configuration;

namespace DbForward.Engine.Configuration;

internal static class ConfigurationExtensions
{
    internal static IConfigurationBuilder TryAddEnvironmentFile(this IConfigurationBuilder builder,
        object? options)
    {
        if (options is GlobalOptions { EnvironmentFile: not null } globalOptions)
        {
            builder.AddJsonFile(globalOptions.EnvironmentFile.FullName);
        }

        return builder;
    }

    internal static IConfigurationBuilder TryAddConnectionOptions(this IConfigurationBuilder builder,
        object? options)
    {
        if (options is IConnectionOptions connectionOptions)
        {
            builder.Add(new ConnectionOptionsSource(connectionOptions));
        }

        return builder;
    }
}