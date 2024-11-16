using System.Text;
using System.Text.RegularExpressions;
using DbForward.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Shouldly;

namespace UnitTests.Features;

public partial class NewSourceTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Output()
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

        var result = await engine.ExecuteAsync("new --tag:author=tester --tag:framework=xunit");
        var builder = new StringBuilder();
        builder.AppendLine(fixture.GetVerifiableLogs());
        builder.AppendLine("Stream output:");
        stream.Position = 0;
        builder.AppendLine(Encoding.UTF8.GetString(stream.ToArray()));

        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(inp => MyRegex().Replace(inp, "{version}"));
        await Verify(builder, settings: settings);
        result.ShouldBe(0);
    }

    [GeneratedRegex(@"[\d]{8}-[\d]{6}")]
    private static partial Regex MyRegex();
}