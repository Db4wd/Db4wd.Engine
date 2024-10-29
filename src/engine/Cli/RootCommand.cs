using Db4Wd.Engine;
using Db4Wd.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Help;
using Vertical.Cli.Parsing;

namespace Db4Wd.Cli;

public static class RootCommand
{
    public static RootCommand<GlobalOptions> Build(EngineHost engine, string rootContext, string description)
    {
        var rootCommand = BuildRootCommand(rootContext, description);
        var services = new ServiceCollection()
            .Scan(types => types.FromAssemblies(typeof(RootCommand).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IFeatureConfiguration<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime())
            .AddSingleton<FeatureBuilder>()
            .AddSingleton(engine)
            .BuildServiceProvider();

        var builder = services.GetRequiredService<FeatureBuilder>();
        builder.Configure(rootCommand);

        return rootCommand;
    }

    private static RootCommand<GlobalOptions> BuildRootCommand(string rootContext, string description)
    {
        var command = new RootCommand<GlobalOptions>(rootContext, description);

        const string optionGroup = "Global options";

        command
            .AddOption(x => x.LogLevel,
                ["-v", "--verbosity"],
                scope: CliScope.Descendants,
                defaultProvider: () => LogLevel.Information,
                description: "Verbosity of logging output (trace, debug, info, warn, error)",
                operandSyntax: "LEVEL",
                optionGroup: optionGroup)
            .AddOption(x => x.TimeZoneOffset,
                ["--tz-offset"],
                scope: CliScope.Descendants,
                defaultProvider: () => TimeZoneInfo.Local.BaseUtcOffset,
                description: "Offset in hours used to display dates & times.",
                operandSyntax: "[-]HH",
                optionGroup: optionGroup)
            .AddSwitch(x => x.Debug,
                ["--debug"],
                scope: CliScope.Descendants,
                description: "Pause the program so a debugger can be attached",
                optionGroup: optionGroup)
            .AddOption(x => x.EnvironmentFile,
                ["--env"],
                scope: CliScope.Descendants,
                description: "Path to an environment file",
                operandSyntax: "PATH",
                optionGroup: optionGroup);

        command.AddHelpSwitch(optionGroup: optionGroup);

        command.ConfigureOptions(options =>
        {
            options.ArgumentPreProcessors =
            [
                ArgumentPreProcessors.AddResponseFileArguments,
                ArgumentPreProcessors.ReplaceEnvironmentVariables,
                ArgumentPreProcessors.ReplaceSpecialFolderPaths
            ];

            options.HelpProvider = new DefaultHelpProvider(new DefaultHelpOptions
            {
                OptionGroups =
                [
                    "Options",
                    "Connection options",
                    "Global options"
                ],
                NameComparer = new CliSymbolComparer()
            });

            options.ValueConverters.AddRange([
                new KeyValuePairConverter(),
                new VersionConverter()
            ]);
        });

        return command;
    }
}
