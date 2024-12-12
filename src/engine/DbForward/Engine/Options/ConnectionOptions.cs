using DbForward.Engine.Features;
using DbForward.Engine.Models;
using Vertical.Cli.Configuration;

namespace DbForward.Engine.Options;

public class ConnectionOptions : GlobalOptions, IConnectionOptions
{
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public uint? Port { get; set; }
    public string? Database { get; set; }
    public string? UserId { get; set; }
    public string? Password { get; set; }
    public KeyValuePair<string, string>[] Properties { get; set; } = [];
}

internal sealed class ConnectionOptionsConfiguration : IFeatureConfiguration
{
    public void Configure(FeatureContext context)
    {
        context.CliBuilder.MapModel<ConnectionOptions>(map => map
            .Option(x => x.ConnectionString, ["--connection-string"])
            .Option(x => x.Host, ["--host"])
            .Option(x => x.Port, ["--port"])
            .Option(x => x.Database, ["--database"])
            .Option(x => x.UserId, ["--user-id"])
            .Option(x => x.Password, ["--password"])
            .Option(x => x.Properties, ["--prop"], Arity.ZeroOrMany)
        );
    }
}