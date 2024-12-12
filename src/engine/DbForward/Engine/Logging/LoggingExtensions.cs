using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.SpectreLogger;
using Vertical.SpectreLogger.Options;

namespace DbForward.Engine.Logging;

internal static class LoggingExtensions
{
    public static IServiceCollection AddConsoleLogging(this IServiceCollection services,
        LogLevel logLevel,
        bool noTerminal)
    {
        return noTerminal
            ? services.AddLogging(builder => builder.AddSpectreConsole(spectre => spectre
                .SetMinimumLevel(logLevel)
                .ConfigureProfiles(profile =>
                {
                    profile.OutputTemplate = "[[{DateTime:yyyy-MM-dd hh:mm}/{LogLevel}]] {Message}{NewLine}{Exception}";
                    profile.DefaultLogValueStyle = "[white]";
                    profile
                        .ClearTypeFormatters()
                        .ClearTypeStyles()
                        .ClearValueStyles();
                })))
            : services.AddLogging(builder => builder.AddSpectreConsole(spectre => spectre
                .SetMinimumLevel(logLevel)
                .ConfigureProfile(LogLevel.Debug, profile => profile.OutputTemplate = "[grey35]DBUG: {Message}{NewLine}{Exception}[/]")
                .ConfigureProfile(LogLevel.Information, profile => profile.OutputTemplate = "[grey74]{Message}{NewLine}{Exception}[/]")
                .ConfigureProfile(LogLevel.Warning, profile => profile.OutputTemplate = "[grey74][gold1]WARN:[/] {Message}{NewLine}{Exception}[/]")
                .ConfigureProfile(LogLevel.Error, profile => profile.OutputTemplate = "[grey74][red1]FAIL:[/] {Message}{NewLine}{Exception}[/]")
                .ConfigureProfile(LogLevel.Critical, profile => profile.OutputTemplate = "[grey74][white on red1]CRIT:[/] {Message}{NewLine}{Exception}[/]")
                .ConfigureProfiles(profile => profile
                    .AddTypeStyle<InfoValue>("[grey74]")
                    .AddTypeStyle<OkValue>("[green3]"))));
    }
}