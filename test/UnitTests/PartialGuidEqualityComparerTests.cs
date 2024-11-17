using DbForward.Utilities;
using Shouldly;

namespace UnitTests;

public class PartialGuidEqualityComparerTests
{
    [Theory]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", "c641c35a-b6c4-4d6f-8900-89b5813a912c", true)]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", "00000000-0000-0000-0000-0000813a912c", true)]
    [InlineData("00000000-0000-0000-0000-0000813a912c", "c641c35a-b6c4-4d6f-8900-89b5813a912c", true)]
    [InlineData(null, null, true)]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", "c641c35a-b6c4-4d6f-8900-89b5813a912b", false)]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", "00000000-0000-0000-0000-0000813a912b", false)]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", "10000000-0000-0000-0000-0000813a912c", false)]
    [InlineData("c641c35a-b6c4-4d6f-8900-89b5813a912c", null, false)]
    [InlineData(null, "c641c35a-b6c4-4d6f-8900-89b5813a912c", false)]
    public void Compare_Returns_Expected(string? x, string? y, bool expected)
    {
        PartialGuidEqualityComparer.Default.Equals(Parse(x), Parse(y)).ShouldBe(expected);
    }

    private static Guid? Parse(string? x) => x != null ? Guid.Parse(x) : null;
}