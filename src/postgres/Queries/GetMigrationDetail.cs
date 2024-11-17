using Dapper;
using DbForward.Models;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class GetMigrationDetail
{
    internal static async Task<MigrationDetail?> QueryCurrentAsync(NpgsqlConnection connection, TimeSpan tzOffset)
    {
        const string sql = 
            $"""
             select m.migrationid
             from {Constants.SchemaName}.migrations_view m
             join {Constants.SchemaName}.log l on (l.logid = m.logid)
             order by l.dateapplied desc
             limit 1; 
             """;

        var currentId = await connection.QuerySingleOrDefaultAsync<Guid?>(sql);

        return currentId != null
            ? await QueryAsync(connection, currentId.Value, tzOffset)
            : null;
    }

    internal static async Task<MigrationDetail?> QueryAsync(NpgsqlConnection connection, Guid id, TimeSpan tzOffset)
    {
        const string sql =
            $"""
             select category, key, value
             from {Constants.SchemaName}.metrics
             where {Constants.SchemaName}.is_uuid_match(migrationid, @id);
             
             select key, value
             from {Constants.SchemaName}.metadata
             where {Constants.SchemaName}.is_uuid_match(migrationid, @id);

             select l.dateapplied, l.agent, l.host, l.dateapplied,
                    m.dbversion, m.logid, m.sourcepath, m.sourcefile, m.sha,
                    m.migrationid
             from {Constants.SchemaName}.migrations_view m
             join {Constants.SchemaName}.log l on (l.logid = m.logid)
             where {Constants.SchemaName}.is_uuid_match(m.migrationid, @id);
             """;

        var multiReader = await connection.QueryMultipleAsync(sql, new { id });

        var metrics = await multiReader.ReadAsync();
        var metricsDictionary = new Dictionary<string, string>();
        var opTagsDictionary = new Dictionary<string, string>();

        foreach (var record in metrics)
        {
            if (record.category == "metric")
                metricsDictionary.Add(record.key, record.value);
            else
                opTagsDictionary.Add(record.key, record.value);
        }
        
        var metadataDictionary = new Dictionary<string, string>();
        foreach (var record in await multiReader.ReadAsync())
        {
            metadataDictionary[record.key] = record.value;
        }

        var migration = await multiReader.ReadSingleOrDefaultAsync();

        if (migration == null)
            return null;

        return new MigrationDetail(
            migration.migrationid,
            migration.dbversion,
            migration.dateapplied.Add(tzOffset),
            migration.logid,
            migration.sourcepath,
            migration.sourcefile,
            migration.sha,
            migration.agent,
            migration.host,
            metricsDictionary,
            opTagsDictionary,
            metadataDictionary);
    }
}