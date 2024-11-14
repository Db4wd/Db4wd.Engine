using Dapper;
using DbForward.Services;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal class GetCurrentVersion
{
    internal static async Task<string?> QueryAsync(NpgsqlConnection connection)
    {
        var versions = (await connection.QueryAsync<string>(
            $"""
             select dbversion
             from {Constants.SchemaName}.migrations_view; 
             """)).ToArray();

        return versions.Max(ConventionalDbVersionComparer.Instance);
    }
}