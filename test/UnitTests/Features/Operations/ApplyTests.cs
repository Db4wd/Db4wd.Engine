using Shouldly;

namespace UnitTests.Features.Operations;

public class ApplyTests
{
    private readonly EngineFixture fixture = new();
    
    [Fact]
    public async Task Apply_Sets_Expected_State()
    {
        var engine = fixture.GetInstance();
        var args = $"apply --base-path:{fixture.AssetPath}";
        var result = await engine.ExecuteAsync(args.Split(' '));
        var data = fixture.GetVerifiable();

        result.ShouldBe(0, data);

        await Verify(data);
    }
    
    [Fact]
    public async Task Apply_With_Target_Id_Sets_Expected_State()
    {
        var engine = fixture.GetInstance();
        var args = $"apply --base-path:{fixture.AssetPath} --target-id:d5de9297-b2d1-49c6-a634-18e8113255d5";
        var result = await engine.ExecuteAsync(args.Split(' '));
        var data = fixture.GetVerifiable();

        result.ShouldBe(0, data);

        await Verify(data);
    }
    
    [Fact]
    public async Task Apply_With_Target_Version_Sets_Expected_State()
    {
        var engine = fixture.GetInstance();
        var args = $"apply --base-path:{fixture.AssetPath} --target-version:20241114-161107";
        var result = await engine.ExecuteAsync(args.Split(' '));
        var data = fixture.GetVerifiable();

        result.ShouldBe(0, data);

        await Verify(data);
    }
    
    [Fact]
    public async Task Apply_With_Tags_Sets_Expected_State()
    {
        var engine = fixture.GetInstance();
        var args = $"apply --base-path:{fixture.AssetPath} --tag:build=r28819 --tag:version=v2.4.51092-dev";
        var result = await engine.ExecuteAsync(args.Split(' '));
        var data = fixture.GetVerifiable();

        result.ShouldBe(0, data);

        await Verify(data);
    }
}