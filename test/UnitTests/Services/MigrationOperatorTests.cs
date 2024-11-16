using System.Text.Json;
using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace UnitTests.Services;

public class MigrationOperatorTests
{
    [Fact]
    public async Task Returns_Expected_Success_Result_With_Invocations()
    {
        var (extension, scope) = GetUnitExtension();
        var result = await ExecuteAsync(extension, SourceOperation.Migrate);
        result.Response.ShouldBe(OperationResponse.Successful);

        var expected = JsonSerializer.Serialize(new
        {
            collectedScope = scope.GetVerifiableObject(),
            result
        }, JsonOptions.Default);
        
        await Verify(expected);
    }
    
    [Fact]
    public async Task Returns_Expected_Aborted_Result_With_Invocations()
    {
        var (extension, scope) = GetUnitExtension(OperationResponse.Aborted);
        var result = await ExecuteAsync(extension, SourceOperation.Migrate);
        result.Response.ShouldBe(OperationResponse.Aborted);

        var expected = JsonSerializer.Serialize(new
        {
            collectedScope = scope.GetVerifiableObject(),
            result
        }, JsonOptions.Default);
        
        await Verify(expected);
    }
    
    [Fact]
    public async Task Returns_Expected_Rollback_Result_With_Invocations()
    {
        var (extension, scope) = GetUnitExtension(OperationResponse.Rollback);
        var result = await ExecuteAsync(extension, SourceOperation.Migrate);
        result.Response.ShouldBe(OperationResponse.Rollback);

        var expected = JsonSerializer.Serialize(new
        {
            collectedScope = scope.GetVerifiableObject(),
            result
        }, JsonOptions.Default);
        
        await Verify(expected);
    }

    private static async Task<MigrationResult> ExecuteAsync(
        IDatabaseExtension extension,
        SourceOperation operation)
    {
        var services = BuildServices(extension);
        var unit = services.GetRequiredService<IMigrationOperator>();
        var sources = await services.GetRequiredService<ISourceFileManager>()
            .GetSourceHeadersInPathAsync(
                new DirectoryInfo("Assets"),
                "*.sql",
                UnitSourceReader.Instance, 
                TestData.EmptyDictionary,
                CancellationToken.None);
        var parameters = new MigrationParameters(
            operation,
            TestData.AgentContext,
            services.GetRequiredService<IFileSystem>(),
            UnitSourceReader.Instance,
            extension.CreateMigrationScopeAsync,
            null,
            sources,
            TestData.EmptyDictionary,
            TestData.EmptyDictionary,
            LogLevel.Information);

        return await unit.ApplyAsync(parameters, CancellationToken.None);
    }

    private static IServiceProvider BuildServices(IDatabaseExtension extension)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddSingleton(extension)
            .AddSingleton<IFileSystem, ConcurrentFileSystem>()
            .AddSingleton<ISourceFileManager, SourceFileManager>()
            .AddSingleton<IMigrationOperator, MigrationOperator>()
            .AddSingleton(new TimerProvider(() => new StaticTimer()))
            .BuildServiceProvider();
    }
    
    private static (IDatabaseExtension, UnitMigrationScope) GetUnitExtension(
        OperationResponse applyResponse = OperationResponse.Successful,
        OperationResponse discardResponse = OperationResponse.Successful)
    {
        var scope = new UnitMigrationScope(new UnitDatabase(), applyResponse, discardResponse);

        var extension = Substitute.For<IDatabaseExtension>();

        extension.CreateMigrationScopeAsync(
                Arg.Any<SourceOperation>(),
                Arg.Any<OperationTracker>(),
                Arg.Any<CancellationToken>())
            .Returns(scope);

        return (extension, scope);
    }
}