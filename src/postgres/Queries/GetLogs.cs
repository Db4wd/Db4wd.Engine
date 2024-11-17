using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class GetLogs
{
    internal static async Task<IList<MigrationHistory>> QueryAsync(
        NpgsqlConnection connection,
        LogSearchParameters parameters,
        TimeSpan tzOffset)
    {
        var builder = new GetLogsBuilder(tzOffset);

        builder.TryAddParameter("migrationid", parameters.MigrationId);
        builder.TryAddParameter("rangestart", parameters.RangeStart);
        builder.TryAddParameter("rangeend", parameters.RangeEnd);
        builder.TryAddParameter("operation", parameters.Operation);
        builder.TryAddParameter("agent", parameters.Agent);
        builder.TryAddParameter("host", parameters.Host);
        builder.TryAddParameter("dbversion", parameters.DbVersion);
        builder.TryAddParameter("sourcepath", parameters.SourcePath);
        builder.TryAddParameter("sourcefile", parameters.SourceFile);
        builder.TryAddParameter("sha", parameters.Sha);
        builder.TryAddParameter("metadatakey", parameters.MetadataKey);
        builder.TryAddParameter("metadatavalue", parameters.MetadataValue);

        var (query, sqlParameters) = builder.Build(parameters.Limit);
        var results = await connection.QueryAsync(query, sqlParameters);

        return results.Select(dyn => new MigrationHistory(
                dyn.migrationid,
                dyn.logid,
                dyn.dateapplied.Add(tzOffset),
                dyn.operation,
                dyn.agent,
                dyn.host))
            .ToArray();
    }
}