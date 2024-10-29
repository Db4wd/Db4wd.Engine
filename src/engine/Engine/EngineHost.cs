using Db4Wd.Cli;
using Db4Wd.Configuration;
using Db4Wd.Extensions;
using Db4Wd.Features;
using Db4Wd.Logging;
using Db4Wd.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Vertical.Cli;

namespace Db4Wd.Engine;

public sealed class EngineHost(
    IServiceCollection serviceCollection,
    IConfigurationBuilder configurationBuilder,
    string rootContext,
    string description) 
    : IEngineHost
{
    internal static readonly Version CurrentVersion = new(1, 0, 0);
    private readonly StartupLogger _startupLogger = new();
    
    internal async Task<int> ExecuteFeatureAsync<TOptions>(
        TOptions options,
        CancellationToken cancellationToken) 
        where TOptions : GlobalOptions
    {
        await using var services = BuildServiceProvider(options);

        await services.GetRequiredService<StartupActionAggregate>().PerformAllAsync();

        var feature = services.GetRequiredService<IFeature<TOptions>>();
        var logger = services.GetRequiredService<ILogger<EngineHost>>();
        
        TryEnterDebugMode(options.Debug);
        _startupLogger.Flush(logger);

        try
        {
            return await feature.HandleAsync(options, cancellationToken);
        }
        catch (ApplicationException exception)
        {
            logger.LogError("Error occurred: {message}", exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred");
        }

        return -1;
    }

    public async Task<int> ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default)
    {
        var rootCommand = RootCommand.Build(this, rootContext, description);

        try
        {
            return await rootCommand.InvokeAsync(arguments, cancellationToken);
        }
        catch (CommandLineException cliException)
        {
            AnsiConsole.MarkupLineInterpolated($"[red1]ERR:[/] [grey85]{cliException.Message}[/]");
            return -1;
        }
    }

    private ServiceProvider BuildServiceProvider<TOptions>(TOptions options) where TOptions : GlobalOptions
    {
        return serviceCollection
            .Scan(types => types.FromAssemblyOf<EngineHost>()
                .AddClasses(classes => classes.AssignableTo(typeof(IFeature<>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime())
            .AddSingleton(BuildConfiguration(options))
            .AddSingleton(new AgentContext(options.TimeZoneOffset))
            .AddConsoleLogging(options.LogLevel)
            .AddStartupServices()
            .BuildServiceProvider();
    }

    private IConfiguration BuildConfiguration<TOptions>(TOptions options)
    {
        if (Environment.GetEnvironmentVariable($"{rootContext.ToUpper()}_ENV_FILE") is { } path)
        {
            configurationBuilder.AddJsonFile(path);
        }
        
        if (options is GlobalOptions { EnvironmentFile: not null } globalOptions)
        {
            configurationBuilder.AddJsonFile(globalOptions.EnvironmentFile.FullName);
        }

        if (options is IConnectionOptions connectionOptions)
        {
            configurationBuilder.Add(new ConnectionOptionsSource(connectionOptions));
        }

        var configuration = configurationBuilder.Build();

        foreach (var provider in configuration.Providers)
        {
            _startupLogger.Log(logger => logger.LogDebug("Configuration provider added: {provider}", provider));
        }

        return configuration;
    }

    private static void TryEnterDebugMode(bool debug)
    {
        if (!debug)
            return;
        
        Console.Write("Press any key after attaching debugger...");
        Console.ReadKey(intercept: true);
        Console.WriteLine();
    }
}