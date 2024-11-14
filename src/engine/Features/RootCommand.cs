using DbForward.Converters;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace DbForward.Features;

internal static class RootCommand
{
    private sealed class SymbolComparer : IComparer<ICliSymbol?>
    {
        /// <inheritdoc />
        public int Compare(ICliSymbol? x, ICliSymbol? y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (ReferenceEquals(x, null))
                return -1;

            if (ReferenceEquals(y, null))
                return 1;

            return string.Compare(x.PrimaryIdentifier, y.PrimaryIdentifier, StringComparison.Ordinal);
        }
    }
    
    public static RootCommand<GlobalOptions> Create(string context, string description)
    {
        var command = new RootCommand<GlobalOptions>(
            context,
            description);

        command
            .AddOption(x => x.LogLevel,
                ["-v", "--log-level"],
                scope: CliScope.Descendants,
                description: "Verbosity of log output (debug or info)",
                defaultProvider: () => LogLevel.Information,
                validation: rule => rule.In([LogLevel.Debug, LogLevel.Information, LogLevel.Warning, LogLevel.Error]),
                operandSyntax: "DEBUG | INFO",
                optionGroup: GlobalOptions.OptionGroup)
            .AddOption(x => x.EnvironmentFile,
                ["--env"],
                scope: CliScope.Descendants,
                description: "Path to an environment file that contains connection settings",
                validation: rule => rule.ExistsOrIsNull(),
                operandSyntax: "PATH",
                optionGroup: GlobalOptions.OptionGroup)
            .AddSwitch(x => x.DebugMode,
                ["--debug"],
                scope: CliScope.Descendants,
                description: "Pauses the program so a debugger can be attached",
                optionGroup: GlobalOptions.OptionGroup)
            .AddSwitch(x => x.ShortSha,
                ["--short-sha"],
                scope: CliScope.Descendants,
                description: "Display the last eight hex characters of sha hash codes",
                optionGroup: GlobalOptions.OptionGroup);

        command.ConfigureOptions(options =>
        {
            options.ValueConverters.Add(new KeyValuePairConverter());

            options.HelpProvider = new DefaultHelpProvider(new DefaultHelpOptions
            {
                OptionGroups =
                [
                    "Options",
                    ConnectionOptions.OptionGroup,
                    GlobalOptions.OptionGroup,
                ],
                NameComparer = new SymbolComparer()
            });
        });

        command.AddHelpSwitch(optionGroup: GlobalOptions.OptionGroup);

        return command;
    }
}