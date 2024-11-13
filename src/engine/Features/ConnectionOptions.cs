using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features;

public class ConnectionOptions : GlobalOptions
{
    internal new const string OptionGroup = "Connection options";
    
    public string? ConnectionString { get; set; }
    
    public string? Host { get; set; }
    
    public uint? Port { get; set; }
    
    public string? Database { get; set; }
    
    public string? UserId { get; set; }
    
    public string? Password { get; set; }

    public KeyValuePair<string, string>[] Properties { get; set; } = [];
}

internal static class ConnectionOptionsExtensions
{
    public static CliCommand<TOptions> AddConnectionOptions<TOptions>(
        this CliCommand<TOptions> command,
        CliScope scope = CliScope.Descendants) where TOptions : ConnectionOptions
    {
        var group = ConnectionOptions.OptionGroup;

        return command
            .AddOption(x => x.ConnectionString,
                ["--connection-string"],
                description: "Connection string used by the database driver",
                scope: scope,
                optionGroup: group)
            .AddOption(x => x.Host,
                ["--host"],
                description: "URI of the database server",
                scope: scope,
                optionGroup: group)
            .AddOption(x => x.Port,
                ["--port"],
                description: "Port to use in the underlying connection",
                scope: scope,
                optionGroup: group)
            .AddOption(x => x.Database,
                ["--database"],
                description: "The database to perform operations on",
                scope: scope,
                optionGroup: group,
                operandSyntax: "NAME")
            .AddOption(x => x.UserId,
                ["--user"],
                description: "Account name to use in the connection credential",
                scope: scope,
                optionGroup: group)
            .AddOption(x => x.Password,
                ["--password"],
                description: "Password to use in the connection credential",
                scope: scope,
                optionGroup: group)
            .AddOption(x => x.Properties,
                ["--prop"],
                description: "Additional property for the database driver to use",
                arity: Arity.ZeroOrMany,
                scope: scope,
                optionGroup: group,
                operandSyntax: "KEY=VALUE");
    }
}