using Dapper;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class SchemaInitialized
{
    internal static async Task<bool> QueryAsync(
        NpgsqlConnection connection, 
        CancellationToken cancellationToken)
    {
        var count = await connection.QuerySingleAsync<int>(
            """
            select count(*)
            from information_schema.tables
            where table_schema=@SchemaName
            """,
            new { Constants.SchemaName });

        return count switch
        {
            0 => false,
            5 => true,
            _ => throw new InvalidOperationException("Migration metadata schema corrupted")
        };
    }
}