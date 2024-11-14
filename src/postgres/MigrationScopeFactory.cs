using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using Microsoft.Extensions.Logging;

namespace DbForward.Postgres;

public interface IMigrationScopeFactory
{
    Task<IMigrationScope> CreateAsync(
        SourceOperation operation,
        OperationTracker operationTracker,
        CancellationToken cancellationToken);
}

internal sealed class MigrationScopeFactory(IConnectionFactory connectionFactory,
    ILoggerFactory loggerFactory)
    : IMigrationScopeFactory
{
    /// <inheritdoc />
    public async Task<IMigrationScope> CreateAsync(SourceOperation operation, 
        OperationTracker operationTracker,
        CancellationToken cancellationToken)
    {
        var connection = await connectionFactory.CreateAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);

        return new PostgresMigrationScope(
            connection,
            transaction,
            operationTracker,
            loggerFactory.CreateLogger<PostgresMigrationScope>());
    }
}