using DbForward.Engine.Configuration;
using DbForward.Engine.Logging;
using DbForward.Engine.Options;
using DbForward.Engine.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Routing;

namespace DbForward.Engine.Features;

internal sealed class FeatureContext
{
    private readonly string appName;
    private readonly IEnumerable<Action<IConfigurationBuilder>> configurationActions;
    private readonly List<Action<IServiceCollection>> serviceCollectionActions;

    private FeatureContext(string appName,
        IEnumerable<Action<IConfigurationBuilder>> configurationActions,
        IEnumerable<Action<IServiceCollection>> serviceCollectionActions)
    {
        this.appName = appName;
        this.configurationActions = configurationActions;
        this.serviceCollectionActions = serviceCollectionActions.ToList();
        CliBuilder = new CliApplicationBuilder(appName);
    }
    
    public CliApplicationBuilder CliBuilder { get; }

    public CliApplication Build() => CliBuilder.Route(appName).Build();

    public void AddRoute(string route)
    {
        CliBuilder.Route($"{CliBuilder.ApplicationName} {route}");
    }

    public void AddFeature<TModel, TFeature>(string route) 
        where TModel : class
        where TFeature : class, IAsyncCallSite<TModel>
    {
        var path = $"{appName} {route}";
        serviceCollectionActions.Add(services => services.AddKeyedSingleton<IAsyncCallSite<TModel>, TFeature>(path));
        CliBuilder.RouteAsync<TModel>(path, (model, token) => ExecuteFeatureAsync(path, model, token));
    }

    public static async Task<int> RunAsync(
        string appName,
        IEnumerable<Action<IConfigurationBuilder>> configurationActions,
        IEnumerable<Action<IServiceCollection>> serviceCollectionActions,
        string[] args,
        CancellationToken cancellationToken)
    {
        var configurations = typeof(FeatureContext)
            .Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true } && 
                           type.IsAssignableTo(typeof(IFeatureConfiguration)))
            .Select(Activator.CreateInstance)
            .Cast<IFeatureConfiguration>();

        var context = new FeatureContext(appName, configurationActions, serviceCollectionActions);
        
        foreach (var configuration in configurations)
            configuration.Configure(context);

        var app = context.Build();

        try
        {
            return await app.InvokeAsync(args, cancellationToken);
        }
        catch (CliArgumentException exception) when (exception.Error is CliArgumentError.IdentifierNotFound
                                                         or CliArgumentError.PathNotCallable
                                                         or CliArgumentError.PathNotFound)
        {
            AnsiConsole.MarkupLineInterpolated($"[red1]{exception.Message}[/]");
            AnsiConsole.WriteLine();
            await app.InvokeHelpAsync(exception.Route?.Path);
        }
        catch (CliArgumentException exception)
        {
            AnsiConsole.MarkupLineInterpolated($"[red1]{exception.Message}[/]");
        }
        finally
        {
            AnsiConsole.WriteLine();
        }

        return -1;
    }

    private async Task<int> ExecuteFeatureAsync<TModel>(
        string route,
        TModel options, 
        CancellationToken cancellationToken) where TModel : class
    {
        var (logLevel, noTerminal) = options is GlobalOptions globalOptions
            ? (globalOptions.LogLevel, globalOptions.NoTerminal)
            : (LogLevel.Information, false);

        var configuration = new ConfigurationBuilder()
            .TryAddEnvironmentFile(options)
            .TryAddConnectionOptions(options)
            .AddBuildActions(configurationActions)
            .Build();

        await using var services = new ServiceCollection()
            .AddBuildActions(serviceCollectionActions)
            .AddConsoleLogging(logLevel, noTerminal)
            .AddSingleton(configuration)
            .BuildServiceProvider();
        
        var callSite = services.GetRequiredKeyedService<IAsyncCallSite<TModel>>(route);
        var logger = services.GetRequiredService<ILogger<FeatureContext>>();

        try
        {
            return await callSite.HandleAsync(options, cancellationToken);
        }
        catch (ApplicationException exception)
        {
            logger.LogError(exception.Message);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Operation stopped");   
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error unhandled");
        }

        return -1;
    }
}