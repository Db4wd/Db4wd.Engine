using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class FindMetadataTag
{
    internal static async Task<IList<MigrationEntry>> QueryAsync(NpgsqlConnection connection,
        KeyValuePair<string, string> tag,
        TimeSpan tzOffset,
        int limit)
    {
        const string sql =
            $"""
             select m.migrationid, m.dbversion, m.sourcepath, m.sourcefile, m.sha, l.dateapplied
             from {Constants.SchemaName}.migrations_view
             join {Constants.SchemaName}.log l on (l.logid = m.logid)
             join {Constants.SchemaName}.metadata md on (md.migrationid = m.migrationid)
             where md.key = @key and md.value like @value
             order by l.dateapplied desc
             limit @limit;
             """;

        var results = await connection.QueryAsync(sql, new
        {
            key = tag.Key,
            value = tag.Value,
            limit
        });

        return results.Select(dyn => new MigrationEntry(
                dyn.migrationid,
                dyn.dbversion,
                dyn.dateapplied.Add(tzOffset),
                dyn.sourcepath,
                dyn.sourcefile,
                dyn.sha))
            .ToArray();
    }
}