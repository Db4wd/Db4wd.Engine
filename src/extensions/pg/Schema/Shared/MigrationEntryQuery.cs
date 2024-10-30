using Dapper;
using Db4Wd.Engine;
using Db4Wd.Models;

namespace Db4Wd.Postgres.Schema.Shared;

public static class MigrationEntryQuery
{
    public static async Task<IReadOnlyCollection<MigrationEntry>> QueryAsync(
        NpgConnectionFactory connectionFactory,
        AgentContext context,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);

        var results = await connection.QueryAsync(
            $"""
            select m.migrationid, 
                   b.path, b.filename, b.sha,
                   l.dateapplied, l.dbversionid
            from {Constants.SchemaName}.migrations m
            join {Constants.SchemaName}.log l on l.migrationid = m.migrationid
            join {Constants.SchemaName}.blobs b on b.migrationid = m.migrationid
            """);

        return results
            .Select(rec => new MigrationEntry(
                rec.migrationid,
                rec.dbversionid,
                rec.dateapplied.Add(context.TimeZoneOffset),
                rec.path,
                rec.filename,
                rec.sha))
            .ToArray();
    }
}