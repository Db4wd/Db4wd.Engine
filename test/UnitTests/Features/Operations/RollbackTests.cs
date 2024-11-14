using Shouldly;

namespace UnitTests.Features.Operations;

public class RollbackTests
{
    private readonly EngineFixture fixture = new();

    [Fact]
    public async Task Rollback_One_Sets_Expected_State()
    {
        await StageUpAsync();
        var args = $"rollback --base-path:{fixture.AssetPath} --target-id:50f1f228-9707-4e10-8add-520a1a910c99";
        var result = await fixture.ExecuteAsync(args);

        result.ShouldBe(0);
        await Verify(fixture.GetVerifiable());
    }
    
    [Fact]
    public async Task Rollback_All_Sets_Expected_State()
    {
        await StageUpAsync();
        var args = $"rollback --base-path:{fixture.AssetPath} --target-id:d5de9297-b2d1-49c6-a634-18e8113255d5";
        var result = await fixture.ExecuteAsync(args);

        result.ShouldBe(0);
        await Verify(fixture.GetVerifiable());
    }

    private async Task StageUpAsync()
    {
        await fixture.ExecuteAsync($"apply --base-path:{fixture.AssetPath}");
        fixture.ClearLogger();
    }
}