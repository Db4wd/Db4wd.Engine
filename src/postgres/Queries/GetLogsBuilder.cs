using System.Text;
using Dapper;

namespace DbForward.Postgres.Queries;

internal sealed class GetLogsBuilder(TimeSpan tzOffset)
{
    private readonly DynamicParameters parameters = new();
    private readonly List<string> logExpressions = [];
    private readonly List<string> migrationExpressions = [];
    private readonly List<string> metadataExpressions = [];

    internal (string Sql, DynamicParameters Parameters) Build(int limit)
    {
        var sql = new StringBuilder();
        sql.AppendLine("select lv.logid, lv.migrationid, lv.dateapplied, lv.operation, lv.agent, lv.host");
        sql.AppendLine($"from {Constants.SchemaName}.log_view lv");

        if (migrationExpressions.Count > 0)
        {
            sql.AppendLine($"join {Constants.SchemaName}.migrations_view mv on (mv.logid = lv.logid)");
        }

        if (metadataExpressions.Count > 0)
        {
            if (migrationExpressions.Count == 0)
            {
                sql.AppendLine($"join {Constants.SchemaName}.migrations_view mv on (mv.logid = lv.logid)");
            }
            sql.AppendLine($"join {Constants.SchemaName}.metadata md on (md.migrationid = mv.migrationid)");
        }

        var expressions = logExpressions
            .Concat(migrationExpressions)
            .Concat(metadataExpressions)
            .ToArray();

        if (expressions.Length > 0)
        {
            sql.AppendLine("where");

            var boolOperator = string.Empty;
            foreach (var expression in expressions)
            {
                sql.AppendLine($"  {boolOperator}{expression}");
                boolOperator = "and ";
            }
        }

        sql.AppendLine("order by lv.dateapplied desc");
        sql.AppendLine("limit @limit");

        parameters.Add("limit", limit);

        return (sql.ToString(), parameters);
    }

    internal GetLogsBuilder TryAddParameter(string parameter, object? value)
    {
        if (value == null)
            return this;

        var sqlParameter = $"@{parameter}";

        switch (sqlParameter)
        {
            case "@migrationid":
                AddLogViewParameter(sqlParameter, $"{sqlParameter} in (lv.migrationid, lv.partialid)", value);
                break;
            
            case "@rangestart":
                AddLogViewParameter(sqlParameter, $"lv.dateapplied >= {sqlParameter}", ((DateTime)value).Subtract(tzOffset));
                break;
            
            case "@rangeend":
                AddLogViewParameter(sqlParameter, $"lv.dateapplied <= {sqlParameter}", ((DateTime)value).Subtract(tzOffset));
                break;
            
            case "@operation":
                AddLogViewParameter(sqlParameter, $"lower(lv.operation) = lower({sqlParameter})", value);
                break;
            
            case "@agent":
                AddLogViewParameter(sqlParameter, $"lower(lv.agent) like lower({sqlParameter})", $"%{value}%");
                break;
            
            case "@host":
                AddLogViewParameter(sqlParameter, $"lower(lv.host) like lower({sqlParameter})", $"%{value}%");
                break;
            
            case "@dbversion":
                AddMigrationsViewParameter(sqlParameter, $"mv.dbversion = {sqlParameter}", value);
                break;
            
            case "@sourcepath":
                AddMigrationsViewParameter(sqlParameter, $"lower(mv.sourcepath) like lower({sqlParameter})",
                    $"%{value}%");
                break;
            
            case "@sourcefile":
                AddMigrationsViewParameter(sqlParameter, $"lower(mv.sourcefile) like lower({sqlParameter})",
                    $"%{value}%");
                break;
            
            case "@sha":
                AddMigrationsViewParameter(sqlParameter, $"mv.sha like {sqlParameter})", $"%{value}");
                break;
            
            case "@metadatakey":
                AddMetadataParameter(sqlParameter, $"lower(md.key) = lower({sqlParameter})", value);
                break;
            
            case "@metadatavalue":
                AddMetadataParameter(sqlParameter, $"lower(md.value) like lower({sqlParameter})",
                    $"%{value}%");
                break;
            
            default:
                throw new InvalidOperationException($"Unsupported column {parameter}");
        }

        return this;
    }
    
    private void AddLogViewParameter(string sqlParameter, string expression, object value)
    {
        logExpressions.Add(expression);
        parameters.Add(sqlParameter, value);
    }

    private void AddMetadataParameter(string sqlParameter, string expression, object value)
    {
        metadataExpressions.Add(expression);
        parameters.Add(sqlParameter, value);
    }

    private void AddMigrationsViewParameter(string sqlParameter, string expression, object value)
    {
        migrationExpressions.Add(expression);
        parameters.Add(sqlParameter, value);
    }
}