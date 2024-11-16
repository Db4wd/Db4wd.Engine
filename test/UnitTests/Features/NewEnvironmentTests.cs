using System.Text;
using DbForward.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Shouldly;

namespace UnitTests.Features;

public class NewEnvironmentTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Results()
    {
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        
        var engine = fixture.GetInstance(services =>
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.CreateWriter(Arg.Any<string>(), Arg.Any<bool>())
                // ReSharper disable once AccessToDisposedClosure
                .Returns(writer);
            services.Replace(ServiceDescriptor.Singleton(fileSystem));
        });

        var result = await engine.ExecuteAsync("new-env local-db.env");

        var builder = new StringBuilder();
        builder.AppendLine(fixture.GetVerifiableLogs());
        builder.AppendLine("Stream output:");
        stream.Position = 0;
        builder.AppendLine(Encoding.UTF8.GetString(stream.ToArray()));
        await Verify(builder);
        result.ShouldBe(0);
    }
}