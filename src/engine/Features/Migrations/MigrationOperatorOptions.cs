using Db4Wd.Services;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;

namespace Db4Wd.Features.Migrations;

public abstract class MigrationOperatorOptions : Options, IMigrationOperatorOptions
{
    /// <inheritdoc />
    public DirectoryInfo BasePath { get; set; } = default!;

    /// <inheritdoc />
    public string? MatchPattern { get; set; }
    
    public abstract MigrationOperatorMode OperatorMode { get; }

    /// <inheritdoc />
    public Guid? TargetMigrationId { get; set; }

    /// <inheritdoc />
    public int? TargetVersion { get; set; }

    /// <inheritdoc />
    public LogLevel StatementLogLevel { get; set; }
}

public static class BaseOptionsExtensions
{
    public static CliCommand<TOptions> AddBaseMigrationOptions<TOptions>(
        this CliCommand<TOptions> command,
        CliScope scope = CliScope.Self)
        where TOptions : MigrationOperatorOptions
    {
        command
            .AddOption(x => x.BasePath,
                ["--base-path"],
                scope: scope,
                description: "Path to the directory that contains migration files",
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                operandSyntax: "PATH")
            .AddOption(x => x.MatchPattern,
                ["--pattern"],
                scope: scope,
                description: "File system glob pattern used to match source files",
                operandSyntax: "GLOB")
            .AddOption(x => x.TargetMigrationId,
                ["--target-id"],
                scope: scope,
                description: "Id of the last migration source to apply in the sequence",
                operandSyntax: "UUID")
            .AddOption(x => x.TargetVersion,
                ["--target-version"],
                scope: scope,
                description: "Database version of the last migration source to apply in the sequence",
                operandSyntax: "ID")
            .AddOption(x => x.StatementLogLevel,
                ["--statement-logging"],
                scope: scope,
                description: "Log level applied to statement logging (trace, debug, info, warn, error)",
                operandSyntax: "LEVEL");

        return command;
    }
}