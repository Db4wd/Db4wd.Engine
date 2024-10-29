using Db4Wd.Extensions;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features;

public class ConnectionOptions : GlobalOptions, IConnectionOptions
{
    public const string OptionGroup = "Connection options";
    
    /// <inheritdoc />
    public string? ConnectionString { get; set; }

    /// <inheritdoc />
    public string? Host { get; set; }

    /// <inheritdoc />
    public uint? Port { get; set; }

    /// <inheritdoc />
    public string? Database { get; set; }

    /// <inheritdoc />
    public string? UserId { get; set; }

    /// <inheritdoc />
    public string? Password { get; set; }

    /// <inheritdoc />
    public Dictionary<string, string> Properties => new(PropertyValues, StringComparer.OrdinalIgnoreCase);

    public KeyValuePair<string, string>[] PropertyValues { get; set; } = [];
}

public static class ConnectionOptionExtensions
{
    public static CliCommand<TOptions> AddConnectionOptions<TOptions>(
        this CliCommand<TOptions> command,
        CliScope scope = CliScope.Self)
        where TOptions : ConnectionOptions
    {
        const string optionGroup = ConnectionOptions.OptionGroup;

        command
            .AddOption(x => x.ConnectionString,
                ["--connection-string"],
                scope: scope,
                description: "Composite connection string used by the database driver",
                optionGroup: optionGroup)
            .AddOption(x => x.Host,
                ["--host"],
                scope: scope,
                description: "Host to connect to",
                optionGroup: optionGroup)
            .AddOption(x => x.Port,
                ["--port"],
                scope: scope,
                description: "Port used by the database driver",
                optionGroup: optionGroup)
            .AddOption(x => x.Database,
                ["--db", "-database"],
                scope: scope,
                description: "Name of the subject database",
                optionGroup: optionGroup)
            .AddOption(x => x.UserId,
                ["--user"],
                scope: scope,
                description: "User account used in the connection credential",
                optionGroup: optionGroup)
            .AddOption(x => x.Password,
                ["--passwrod"],
                scope: scope,
                description: "Password used in the connection credential",
                optionGroup: optionGroup)
            .AddOption(x => x.PropertyValues,
                ["--prop"],
                scope: scope,
                arity: Arity.ZeroOrMany,
                description: "Additional property used by the database driver",
                operandSyntax: "KEY=VALUE",
                optionGroup: optionGroup);

        return command;
    }
}