using Shouldly;

namespace UnitTests.Features;

public class AuditTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Output()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        var result = await engine.ExecuteAsync($"audit --base-path:{fixture.AssetPath}");
        result.ShouldBe(0);

        await Verify(fixture.GetVerifiableLogs());
    }
}