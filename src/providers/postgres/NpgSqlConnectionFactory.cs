using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DbForward.Postgres;

internal sealed class NpgSqlConnectionFactory
{
    private readonly IConfiguration configuration;
    private readonly ILogger<NpgSqlConnectionFactory> logger;
    private readonly Lazy<NpgsqlConnectionStringBuilder> lazyConnectionStringBuilder;
    private bool logOnce = true;

    public NpgSqlConnectionFactory(
        IConfiguration configuration,
        ILogger<NpgSqlConnectionFactory> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
        lazyConnectionStringBuilder = new Lazy<NpgsqlConnectionStringBuilder>(CreateConnectionBuilder);
    }

    public async Task<NpgsqlConnection> GetInitializedConnectionAsync(CancellationToken cancellationToken)
    {
        var connectionBuilder = lazyConnectionStringBuilder.Value;
        var connection = new NpgsqlConnection(connectionBuilder.ToString());

        if (logOnce)
        {
            logOnce = false;
            logger.LogInformation("Connecting to {host}/{database}",
                connectionBuilder.Host,
                connectionBuilder.Database);
        }

        await connection.OpenAsync(cancellationToken);
        return connection;
    }
    
    private NpgsqlConnectionStringBuilder CreateConnectionBuilder()
    {
        var npgBuilder = new NpgsqlConnectionStringBuilder();

        foreach (var section in configuration.GetChildren())
        {
            if (section.Value is not { } value)
                continue;

            TryAddProperty(npgBuilder, section.Key, value);
        }
        
        return npgBuilder;
    }

    private void TryAddProperty(NpgsqlConnectionStringBuilder npgBuilder, string key, string value)
    {
        try
        {
            npgBuilder[key] = value;
        }
        catch (Exception exception)
        {
            logger.LogError("Failed to set npg connection property {key}: {message}",
                key,
                exception.Message);
            throw new OperationCanceledException();
        }
    }
}