using Shouldly;

namespace UnitTests.Features;

public class ListTests
{
    private readonly EngineFixture fixture = new();
    
    [Fact]
    public async Task Receives_Expected_Output()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("list");
        result.ShouldBe(0);

        await Verify(fixture.GetVerifiableLogs());
    }
}