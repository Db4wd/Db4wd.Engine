using Dapper;
using Db4Wd.Engine;
using Db4Wd.Extensions;

namespace Db4Wd.Postgres.Schema.Shared;

public static class LockQuery
{
    public static async Task<IReadOnlyCollection<LockInfo>> GetLocksAsync(
        NpgConnectionFactory connectionFactory,
        AgentContext context,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateAsync(cancellationToken);
        
        var results = await connection.QueryAsync(
            $"""
            select leasetype, lockid, dateacquired, agent, host
            from {Constants.SchemaName}.locks; 
            """);

        return results
            .Select(obj => new LockInfo
            {
                Agent = obj.agent,
                Host = obj.host,
                Type = obj.leasetype,
                LockId = obj.lockid,
                DateAcquired = obj.dateacquired.Add(context.TimeZoneOffset)
            })
            .ToArray();
    }
}