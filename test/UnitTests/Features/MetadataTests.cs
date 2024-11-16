using Shouldly;

namespace UnitTests.Features;

public class MetadataTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Receives_Expected_Results()
    {
        var engine = await fixture.GetStagedInstanceAsync();
        (await engine.ExecuteAsync($"metadata delete --base-path:{fixture.AssetPath} --confirm")).ShouldBe(0);
        (await engine.ExecuteAsync($"metadata insert --base-path:{fixture.AssetPath} --confirm")).ShouldBe(0);

        await Verify(fixture.GetVerifiable());
    }
}