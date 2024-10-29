using Db4Wd.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres;

public sealed class NpgConnectionFactory(IConfiguration configuration, ILogger<NpgConnectionFactory> logger)
{
    private readonly Lazy<NpgsqlConnectionStringBuilder> _lazyConnectionBuilder =
        new(() => BuildConnectionString(configuration, logger));

    private int _logConnect;

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken cancellationToken)
    {
        var builder = _lazyConnectionBuilder.Value;
        var connectionString = builder.ToString();
        var connection = new NpgsqlConnection(connectionString);
        var logInfo = _logConnect++ == 0;

        if (logInfo)
        {
            logger.LogDebug("Opening connection to Postgres...");
        }
        
        await connection.OpenAsync(cancellationToken);

        if (logInfo)
        {
            logger.LogInformation("Connected to {host}/{db}", builder.Host, builder.Database);
        }

        return connection;
    }
    
    private static NpgsqlConnectionStringBuilder BuildConnectionString(
        IConfiguration configuration,
        ILogger<NpgConnectionFactory> logger)
    {
        logger.LogDebug("Creating NpgConnectionStringBuilder");
        var builder = new NpgsqlConnectionStringBuilder(configuration["ConnectionString"]);

        foreach (var section in configuration.GetChildren())
        {
            if (section.Key.Equals("ENV_FILE", StringComparison.OrdinalIgnoreCase))
                continue;
            
            TrySetValue(builder, section.Key, section.Value);
            logger.LogDebug("Set property: {prop}", section.Key);
        }

        return builder;
    }

    private static void TrySetValue(NpgsqlConnectionStringBuilder builder, 
        string key, 
        string? value)
    {
        try
        {
            builder[key] = value;
        }
        catch
        {
            throw new ApplicationException($"Could not set NpgSql connection property '{key}'");
        } 
    }
}