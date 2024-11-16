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

    private readonly JsonCaptureLogger.Provider loggerProvider = new();

    public string AssetPath { get; } = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "Assets");

    public void ClearLogger() => loggerProvider.ClearLogger();

    public string GetVerifiable()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"db = {JsonSerializer.Serialize(Database.GetVerifiableObject(), JsonOptions.Default)}");
        builder.AppendLine("Logs:");
        builder.Append(loggerProvider.GetVerifiable());

        return builder.ToString();
    }

    public string GetVerifiableLogs() => loggerProvider.GetVerifiable();

    public IEngineHost GetInstance(Action<IServiceCollection>? configure = null)
    {
        var builder = new EngineHostBuilder(
            "unit-test",
            "Run unit tests");

        builder.AddExtension(_ => new UnitExtension(Database));

        builder.ConfigureServices(services => services
            .AddSingleton(new TimerProvider(() => new StaticTimer()))
            .AddSingleton<IAgentContext>(_ =>
            {
                var mock = Substitute.For<IAgentContext>();
                mock.Agent.Returns("xunit");
                mock.Host.Returns("test-host");
                mock.TimeZoneOffset.Returns(TimeSpan.Zero);
                return mock;
            })
            .AddSingleton<IFileSystem, ConcurrentFileSystem>()
            .AddLogging(logging => logging
                .ClearProviders()
                .AddProvider(loggerProvider)));

        builder.ConfigureServices(services => configure?.Invoke(services));

        return builder.Build();
    }

    public async Task<IEngineHost> GetStagedInstanceAsync()
    {
        var instance = GetInstance();
        await instance.ExecuteAsync($"apply --base-path:{AssetPath}");
        ClearLogger();
        return instance;
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