using Dapper;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class NextSequenceId
{
    public static async Task<int> QueryAsync(NpgsqlConnection connection)
    {
        return await connection.QuerySingleAsync<int>(
            $"select nextval('{Constants.SchemaName}.id_sequence');");
    }
}