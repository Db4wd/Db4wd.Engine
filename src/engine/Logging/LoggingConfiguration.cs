using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.SpectreLogger;
using Vertical.SpectreLogger.Options;

namespace Db4Wd.Logging;

public static class LoggingConfiguration
{
    public static IServiceCollection AddConsoleLogging(
        this IServiceCollection services, 
        LogLevel logLevel)
    {
        return services
            .AddLogging(builder =>
            {
                builder
                    .ClearProviders()
                    .SetMinimumLevel(logLevel)
                    .AddSpectreConsole(console =>
                    {
                        console
                            .SetMinimumLevel(logLevel)
                            .ConfigureProfile(LogLevel.Trace, p =>
                                p.OutputTemplate = "[grey35]TRC: {Message}{NewLine}{Exception}[/]")
                            .ConfigureProfile(LogLevel.Debug, p =>
                                p.OutputTemplate = "[grey46]DBG: {Message}{NewLine}{Exception}[/]")
                            .ConfigureProfile(LogLevel.Information, p =>
                                p.OutputTemplate = "[grey85]{Message}{NewLine}{Exception}[/]")
                            .ConfigureProfile(LogLevel.Warning, p =>
                                p.OutputTemplate = "[grey85][gold1]WRN:[/] {Message}{NewLine}{Exception}[/]")
                            .ConfigureProfile(LogLevel.Error, p =>
                                p.OutputTemplate = "[grey85][red1]ERR:[/] {Message}{NewLine}{Exception}[/]")
                            .ConfigureProfile(LogLevel.Critical, p =>
                                p.OutputTemplate = "[grey85][white on red1]CRT:[/] {Message}{NewLine}{Exception}[/]")
                            .ConfigureProfiles(profiles => profiles
                                .AddTypeStyle<Uri>("[underline dodgerblue1]")
                                .AddTypeStyle<KeyToken>("[darkgoldenrod]")
                                .ConfigureOptions<DestructuringOptions>(opt =>
                                {
                                    opt.WriteIndented = true;
                                    opt.IndentSpaces = 2;
                                }));
                    });
            });
    }
}