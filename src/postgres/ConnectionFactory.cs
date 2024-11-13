using DbForward.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DbForward.Postgres;

/// <summary>
/// Creates connection instances.
/// </summary>
public interface IConnectionFactory
{
    /// <summary>
    /// Creates a connection.
    /// </summary>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns <see cref="NpgsqlConnection"/></returns>
    Task<NpgsqlConnection> CreateAsync(CancellationToken cancellationToken);
}

internal sealed class DefaultConnectionFactory(IConfiguration configuration, 
    ILogger<DefaultConnectionFactory> logger) : IConnectionFactory
{
    private readonly Lazy<NpgsqlConnectionStringBuilder> lazyBuilder = new(() => CreateBuilder(configuration, logger));
    private int logOnce = 0;

    /// <inheritdoc />
    public async Task<NpgsqlConnection> CreateAsync(CancellationToken cancellationToken)
    {
        var builder = lazyBuilder.Value;
        var connection = new NpgsqlConnection(builder.ToString());

        await connection.OpenAsync(cancellationToken);

        if (logOnce++ == 0)
        {
            logger.LogInformation("Connected to {host}/{database}", builder.Host, builder.Database);
        }

        return connection;
    }

    private static NpgsqlConnectionStringBuilder CreateBuilder(
        IConfiguration configuration, 
        ILogger<DefaultConnectionFactory> logger)
    {
        var builder = new NpgsqlConnectionStringBuilder();

        foreach (var section in configuration.GetChildren())
        {
            builder[section.Key] = section.Value;
            logger.LogDebug("Added connection property {key}", section.Key);
        }
        
        // Basic checks
        string?[] requiredProperties =
        [
            builder.ConnectionString,
            $"{builder.Host}{builder.Database}{builder.Username}{builder.Password}{builder.Passfile}"
        ];

        if (requiredProperties.All(string.IsNullOrWhiteSpace))
        {
            throw new LoggerCallbackException(log => log.LogError(
                """
                Couldn't initiate connection, missing required properties:
                  {cs}
                  {composite}
                """,
                "-> ConnectionString",
                "-> or Host;Database;UserName;Password[;Port]"));
        }

        return builder;
    }
}