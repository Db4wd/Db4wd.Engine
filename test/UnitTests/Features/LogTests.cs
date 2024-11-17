using Shouldly;

namespace UnitTests.Features;

public class LogTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Output()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("log");

        await Verify(fixture.GetVerifiableLogs());
        result.ShouldBe(0);
    }
    
    [Fact]
    public async Task Receives_Expected_Output_ForId()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("log --id d5de9297-b2d1-49c6-a634-18e8113255d5");

        await Verify(fixture.GetVerifiableLogs());
        result.ShouldBe(0);
    }
}