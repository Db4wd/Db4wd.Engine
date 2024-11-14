using Dapper;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DbForward.Postgres;

internal sealed class PostgresMigrationScope(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
    OperationTracker operationTracker,
    ILogger logger) : IMigrationScope
{
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
        await connection.DisposeAsync();
    }

    /// <inheritdoc />
    public async Task<OperationResponse> ApplyDirectiveAsync(StatementSectionDirective directive, 
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await connection.ExecuteAsync(
                directive.Text,
                transaction: transaction);

            operationTracker.Increment("postgres/rowsAffected", Math.Max(0, rows));

            return OperationResponse.Successful;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Operation failed");
            return OperationResponse.Aborted;
        }
    }

    /// <inheritdoc />
    public async Task CommitChangesAsync(MigrationOperationDetail detail, CancellationToken cancellationToken)
    {
        await Commands.SaveMigrationDetail.ExecuteAsync(connection, transaction, detail);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<OperationResponse> DiscardChangesAsync(CancellationToken cancellationToken)
    {
        // Never commit transaction
        return Task.FromResult(OperationResponse.Aborted);
    }
}