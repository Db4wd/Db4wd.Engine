using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class GetMigrationHistory
{
    internal static async Task<IList<MigrationHistory>> QueryAsync(
        NpgsqlConnection connection,
        Guid id, 
        int limit,
        TimeSpan tzOffset)
    {
        var results = await connection.QueryAsync(
            $"""
             select logid, migrationid, dateapplied, operation, agent, host
             from {Constants.SchemaName}.log
             where migrationid = @id
             order by dateapplied desc
             limit {limit};
             """,
            new { id });

        return results
            .Select(dyn => new MigrationHistory(
                dyn.migrationid,
                dyn.logid,
                dyn.dateapplied.Add(tzOffset),
                dyn.operation,
                dyn.agent,
                dyn.host))
            .ToList();
    }
}