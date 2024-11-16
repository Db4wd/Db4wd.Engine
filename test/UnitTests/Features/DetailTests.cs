using Shouldly;

namespace UnitTests.Features;

public class DetailTests
{
    private readonly EngineFixture fixture = new();
    
    [Fact]
    public async Task Receives_Excepted_Output()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync("detail d5de9297-b2d1-49c6-a634-18e8113255d5");
        result.ShouldBe(0);

        await Verify(fixture.GetVerifiableLogs());
    }
}