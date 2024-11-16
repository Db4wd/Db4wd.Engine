using System.Text.RegularExpressions;
using Shouldly;

namespace UnitTests.Features;

public partial class StatusTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Result()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("status");
        await Verify(fixture.GetVerifiable());
        result.ShouldBe(0);
    }
}