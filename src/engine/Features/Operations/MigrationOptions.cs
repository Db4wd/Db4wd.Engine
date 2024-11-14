using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace DbForward.Features.Operations;

public class MigrationOptions : ConnectionOptions
{
    public DirectoryInfo BasePath { get; set; } = default!;
    
    public string? SearchPattern { get; set; }
    
    public Guid? TargetId { get; set; }
    
    public string? TargetDbVersion { get; set; }
    
    public LogLevel StatementLogLevel { get; set; }

    public KeyValuePair<string, string>[] Tokens { get; set; } = [];
}

internal static class MigrationOptionsExtensions
{
    public static CliCommand<TOptions> AddCommonMigrationOptions<TOptions>(this CliCommand<TOptions> command,
        CliScope scope = CliScope.Descendants) where TOptions : MigrationOptions
    {
        command
            .AddOption(x => x.BasePath,
                ["--base-path"],
                scope: scope,
                description: "Path where migration sources can be found",
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                validation: rule => rule.Exists(),
                operandSyntax: "PATH")
            .AddOption(x => x.SearchPattern,
                ["--search-pattern"],
                scope: scope,
                description:
                "File system glob pattern used to match files (defaults to recursive search using the default file extension",
                operandSyntax: "PATTERN")
            .AddOption(x => x.StatementLogLevel,
                ["--statement-log-level"],
                scope: scope,
                description:
                "Log level at which directive statements are logged (debug, info, error). Failed directives " +
                "are always logged",
                defaultProvider: () => LogLevel.Debug,
                operandSyntax: "LEVEL")
            .AddOption(x => x.Tokens,
                ["--token"],
                scope: scope,
                arity: Arity.ZeroOrMany,
                description: "Key/value pair that represents a token and value to replace in source files",
                operandSyntax: "KEY=VALUE");

        return command;
    }
}