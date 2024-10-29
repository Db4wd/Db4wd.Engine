using Dapper;
using Db4Wd.Extensions;
using Microsoft.Extensions.Logging;

namespace Db4Wd.Postgres.Schema.Shared;

internal static class DeleteLocks
{
    public static async Task<LockResult> ExecuteAsync(
        NpgConnectionFactory connectionFactory, 
        Guid? id,
        ILogger logger, 
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        
        var sql = id.HasValue ?
            $"""
             delete from {Constants.SchemaName}.locks
             where lockid = @id;
             """
            :
            $"delete from {Constants.SchemaName}.locks;";

        var displayId = id?.ToString() ?? "(all)";
        logger.LogDebug("Attempting release lock, id={id}", displayId);
        var affected = await connection.ExecuteAsync(sql, new { id });

        return affected == 1 ? LockResult.Released : LockResult.NoLocksAffected;
    }
}