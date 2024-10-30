using Dapper;
using Db4Wd.Engine;

namespace Db4Wd.Postgres.Schema.Shared;

public static class WriteTemplate
{
    public static async Task WriteAsync(
        NpgConnectionFactory connectionFactory,
        AgentContext agentContext,
        TextWriter writer, 
        KeyValuePair<string, string>[] metadata, 
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        var headerInfo = await connection.QuerySingleAsync(
            $"""
             select
                 nextval('{Constants.SchemaName}.id_sequence') as dbversion,
                 extract(epoch from CURRENT_TIMESTAMP) as epoch;
             """);

        await writer.WriteLineAsync(
            $$"""
            -- {head}
            -- {id: {{Guid.NewGuid()}}}
            -- {dbVersion: {{headerInfo.dbversion}}}
            -- {epoch: {{headerInfo.epoch}}}
            -- {metadata.author={{agentContext.Agent}}}
            -- {metadata.date={{DateTime.Now}}} 
            """);

        foreach (var (key, value) in metadata)
        {
            await writer.WriteLineAsync($"-- {{metadata.{key}={value}}}");
        }

        await writer.WriteLineAsync(
            """
            -- {~head}
            
            -- The following directives are available (enclose in curly brackets):
            --   begin transaction: start a transaction
            --   commit: commit ongoing transaction
            --   batch: execute all statements up to here
            --   up/~up: start and end the migration statements section
            --   down/~down: start and end the vrollback statements section
            
            -- Notes:
            --   1 - Do not remove the id & dbversion head directives
            --   2 - Do not write idempotent migration statements
            --   3 - Do write idempotent rollback statements
            --   4 - Any lines starting with -- not matching any directives are ignored
            --   5 - Whitespace lines are ignored
            --   6 - Leverage transactions (Postgres supports both DDL and DML transactions)
            
            -- {up}
            --   TODO: Insert migration statements
            -- {~up}
            
            -- {down}
            --   TODO: Insert idempotent rollback statements here such that the effects of the
            --   above migration statements are reversed in order.
            -- {~down}
            """);
    }
}