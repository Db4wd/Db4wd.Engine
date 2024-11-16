using Shouldly;

namespace UnitTests.Features;

public class HistoryTests
{
    private readonly EngineFixture fixture = new();
    
    [Fact]
    public async Task Receives_Expected_Output()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("history 50f1f228-9707-4e10-8add-520a1a910c99");
        result.ShouldBe(0);

        await Verify(fixture.GetVerifiableLogs());
    }
}