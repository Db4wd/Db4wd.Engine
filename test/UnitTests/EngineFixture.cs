using System.Reflection;
using System.Text;
using System.Text.Json;
using DbForward.Engine;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests;

public sealed class EngineFixture
{
    public UnitDatabase Database { get; } = new();

    public UnitLogger.Provider LoggerProvider { get; } = new();

    public string AssetPath { get; } = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "Assets");

    public void ClearLogger() => LoggerProvider.Logger.Clear();

    public string GetVerifiable()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"db = {JsonSerializer.Serialize(Database.GetVerifiableObject(), JsonOptions.Value)}");
        builder.AppendLine("Logs:");
        builder.Append(LoggerProvider.Logger);

        return builder.ToString();
    }

    public IEngineHost GetInstance()
    {
        var builder = new EngineHostBuilder(
            "unit-test",
            "Run unit tests");

        builder.AddExtension(_ => new UnitExtension(Database));

        builder.ConfigureServices(services => services
            .AddLogging(logging => logging
                .ClearProviders()
                .AddProvider(LoggerProvider))
            .AddSingleton(new TimerProvider(() => new StaticTimer()))
            .AddSingleton<IAgentContext>(_ =>
            {
                var mock = Substitute.For<IAgentContext>();
                mock.Agent.Returns("xunit");
                mock.Host.Returns("test-host");
                mock.TimeZoneOffset.Returns(TimeSpan.Zero);
                return mock;
            })
            .AddSingleton<IFileSystem, ConcurrentFileSystem>());

        return builder.Build();
    }

    public async Task<int> ExecuteAsync(string args) =>
        await GetInstance().ExecuteAsync(args);
}

public static class Extensions
{
    public static async Task<int> ExecuteAsync(
        this IEngineHost host,
        string argString)
    {
        return await host.ExecuteAsync(argString.Split(' '));
    }
}