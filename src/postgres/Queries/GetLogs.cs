using System.Text;
using Dapper;
using DbForward.Models;
using Npgsql;

namespace DbForward.Postgres.Queries;

internal static class GetLogs
{
    internal static async Task<IList<MigrationHistory>> QueryAsync(NpgsqlConnection connection,
        LogSearchParameters parameters,
        TimeSpan tzOffset)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("select logid, migrationid, dateapplied, operation, agent, host");
        sqlBuilder.AppendLine($"from {Constants.SchemaName}.log_view");
        
        var qp = new DynamicParameters();
        qp.Add("limit", parameters.Limit);

        var operand = string.Empty;

        TryAddParameter(sqlBuilder, qp, "migrationid", "(migrationid = @migrationid or partialid = @migrationid)", 
            parameters.MigrationId, ref operand);
        TryAddParameter(sqlBuilder, qp, "rangestart", "dateapplied >= @rangestart", 
            parameters.RangeStart?.Add(tzOffset), ref operand);
        TryAddParameter(sqlBuilder, qp, "rangeend", "dateapplied <= @rangeend", 
            parameters.RangeEnd?.Add(tzOffset), ref operand);
        TryAddParameter(sqlBuilder, qp, "operation", "operation = @operation", parameters.Operation, ref operand);
        TryAddParameter(sqlBuilder, qp, "agent", "agent like @agent", 
            parameters.Agent != null ? $"%{parameters.Agent}%" : null, ref operand);
        TryAddParameter(sqlBuilder, qp, "host", "host like @host", 
            parameters.Host != null ? $"%{parameters.Host}" : null, ref operand);

        sqlBuilder.AppendLine("order by dateapplied desc");
        sqlBuilder.AppendLine("limit @limit;");

        var sql = sqlBuilder.ToString();

        var results = await connection.QueryAsync(
            sql,
            qp);

        return results.Select(dyn => new MigrationHistory(
                dyn.migrationid,
                dyn.logid,
                dyn.dateapplied.Add(tzOffset),
                dyn.operation,
                dyn.agent,
                dyn.host))
            .ToArray();
    }

    private static void TryAddParameter(StringBuilder sql,
        DynamicParameters qp,
        string parameter,
        string clause,
        object? value,
        ref string operand)
    {
        if (value == null)
            return;

        if (operand == string.Empty)
        {
            sql.AppendLine("where");
        }
        
        sql.AppendLine($"   {operand}{clause}");
        operand = "and ";
        qp.Add(parameter, value);
    }
}