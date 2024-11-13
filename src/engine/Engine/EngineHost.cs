using DbForward.Features;
using DbForward.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Vertical.Cli;

namespace DbForward.Engine;

internal sealed class EngineHost(
    IConfigurationBuilder configurationBuilder,
    IServiceCollection extensionServices,
    string context,
    string description)
    : IEngineHost, IFeatureHost
{
    /// <inheritdoc />
    public async Task<int> ExecuteFeatureAsync<TOptions>(
        TOptions options, 
        CancellationToken cancellationToken) where TOptions : GlobalOptions
    {
        TryEnterDebugMode(options.DebugMode);
        
        await using var services = new ServiceCollection()
            .AddConsoleLogging(options.LogLevel)
            .AddEngine()
            .AddExtensionServices(extensionServices)
            .AddConfiguration(configurationBuilder, options)
            .BuildServiceProvider();

        var logger = services.GetRequiredService<ILogger<EngineHost>>();
        StartupLogger.Flush(logger);

        var feature = services.GetRequiredService<IFeature<TOptions>>();

        try
        {
            return await feature.HandleAsync(options, cancellationToken);
        }
        catch (LoggerCallbackException loggerCallbackException)
        {
            loggerCallbackException.Log(logger);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Operation cancelled");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Caught unhandled error");
        }

        return -1;
    }

    private static void TryEnterDebugMode(bool debugMode)
    {
        StartupLogger.Log(LogLevel.Debug, $"Debug mode: {debugMode}");

        if (!debugMode)
            return;
        
        AnsiConsole.Write("[grey85]Press any key after attaching debugger...[/]");
        Console.ReadKey(intercept: true);
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc />
    public async Task<int> ExecuteAsync(string[] arguments, CancellationToken cancellationToken = default)
    {
        var command = FeatureBuilder.Build(this, RootCommand.Create(context, description));

        try
        {
            return await command.InvokeAsync(arguments, cancellationToken);
        }
        catch (CommandLineException exception) when (exception.Error == CommandLineError.NoCallSite)
        {
            command.DisplayHelp(exception.Command);
            return 0;
        }
        catch (CommandLineException exception)
        {
            AnsiConsole.MarkupLineInterpolated($"[red1]{exception.Message}[/]");
            return -1;
        }
        finally
        {
            AnsiConsole.WriteLine();
        }
    }
}