using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class GetMigrationEntries
{
    internal static async Task<IList<MigrationEntry>> QueryAsync(
        NpgsqlConnection connection,
        TimeSpan tzOffset)
    {
        var results = await connection.QueryAsync(
            $"""
            select m.migrationid, m.dbversion,
                   l.dateapplied,
                   m.sourcepath,
                   m.sourcefile,
                   m.sha
            from {Constants.SchemaName}.migrations_view m
            join {Constants.SchemaName}.log l on (l.logid = m.logid)
            where not exists (
                select *
                from {Constants.SchemaName}.metadata md
                where md.migrationid = m.migrationid and md.key = @key                        
            )
            order by l.dateapplied;
            """,
            new { key = "pgfwd/internalMigration" });

        return results
            .Select(dyn => new MigrationEntry(
                dyn.migrationid,
                dyn.dbversion,
                dyn.dateapplied.Add(tzOffset),
                dyn.sourcepath,
                dyn.sourcefile,
                dyn.sha))
            .ToArray();
    }
}