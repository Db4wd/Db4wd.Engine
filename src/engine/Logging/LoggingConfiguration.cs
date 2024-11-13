using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Vertical.SpectreLogger;
using Vertical.SpectreLogger.Options;
using Vertical.SpectreLogger.Rendering;

namespace DbForward.Logging;

internal static class LoggingConfiguration
{
    internal static IServiceCollection AddConsoleLogging(
        this IServiceCollection serviceCollection,
        LogLevel logLevel)
    {
        return serviceCollection
            .AddLogging(builder => builder
                .ClearProviders()
                .SetMinimumLevel(logLevel)
                .AddSpectreConsole(console => console
                    .SetMinimumLevel(logLevel)
                    .ConfigureProfile(LogLevel.Debug, profile => profile
                        .OutputTemplate = "[grey46]DBG: {Message}{NewLine}{Exception}[/]")
                    .ConfigureProfile(LogLevel.Information, profile => profile
                        .OutputTemplate = "[grey85]{Message}{NewLine}{Exception}[/]")
                    .ConfigureProfile(LogLevel.Warning, profile => profile
                        .OutputTemplate = "[grey85][gold1]WRN:[/] {Message}{NewLine}{Exception}[/]")
                    .ConfigureProfile(LogLevel.Error, profile => profile
                        .OutputTemplate = "[grey85][red1]ERR:[/] {Message}{NewLine}{Exception}[/]")
                    .ConfigureProfile(LogLevel.Critical, profile => profile
                        .OutputTemplate = "[grey85][white on red1]CRT:[/] {Message}{NewLine}{Exception}[/]")
                    .ConfigureProfiles(profile => profile
                        .AddTypeStyle<OkToken>(Color.Green1)
                        .AddTypeStyle<VerboseToken>(Color.Grey46)
                        .ConfigureOptions<DestructuringOptions>(destr =>
                        {
                            destr.IndentSpaces = 2;
                            destr.WriteIndented = true;
                            destr.MaxProperties = 15;
                        }))
                    .ConfigureProfiles([LogLevel.Error, LogLevel.Critical], profile => profile
                        .ConfigureOptions<ExceptionRenderer.Options>(ex => ex.MaxStackFrames = 50))
                ));
    }
}