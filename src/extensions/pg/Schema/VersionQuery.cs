using Dapper;
using Npgsql;

namespace Db4Wd.Postgres.Schema;

public static class VersionQuery
{
    public static async Task<Version?> GetInstalledVersionAsync(NpgsqlConnection connection)
    {
        var versionTableInstalled = await connection.QuerySingleAsync<bool>(
            """
            select exists (
                select *
                from information_schema.tables
                where table_schema=@schema and table_name=@table
            );
            """,
            new { schema = Constants.SchemaName, table = "versions" });

        if (!versionTableInstalled)
        {
            return null;
        }

        var versionInfo = await connection.QuerySingleOrDefaultAsync(
            $"""
            select major, minor, revision
            from { Constants.SchemaName }.versions
            order by major desc, minor desc, revision desc
            limit 1; 
            """);

        return versionInfo != null
            ? new Version(versionInfo.major, versionInfo.minor, versionInfo.revision)
            : null;
    }
}