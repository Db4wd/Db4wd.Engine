using DbForward.Constants;
using DbForward.Services;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class MissingSourceAuditStepTests
{
    private readonly MissingSourceAuditStep unit = new(NullLogger<MissingSourceAuditStep>.Instance);
    
    [Fact]
    public async Task Missing_Sources_Fails()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            [TestData.Sources[0]],
            [],
            TestData.Entries.Take(2).ToArray(),
            null,
            null,
            Substitute.For<IDbVersionComparer>());

        (await unit.AuditAsync(context, CancellationToken.None)).ShouldNotBe(0);
    }
    
    [Fact]
    public async Task Matches_Sources_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources.Take(2).ToArray(),
            [],
            TestData.Entries.Take(2).ToArray(),
            null,
            null,
            Substitute.For<IDbVersionComparer>());

        var result = await unit.AuditAsync(context, CancellationToken.None);
        
        result.ShouldBe(0);
    }
}