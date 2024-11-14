using Dapper;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class NextVersionId
{
    public static async Task<DateTime> QueryAsync(NpgsqlConnection connection)
    {
        return await connection.QuerySingleAsync<DateTime>("select current_timestamp;");
    }
}