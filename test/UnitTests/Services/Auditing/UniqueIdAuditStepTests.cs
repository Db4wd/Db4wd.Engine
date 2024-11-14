using DbForward.Constants;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class UniqueIdAuditStepTests
{
    private readonly UniqueIdAuditStep unit = new(NullLogger<UniqueIdAuditStep>.Instance);

    [Fact]
    public async Task Has_Duplicate_Versions_Fails()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources.Append(TestData.Sources[0]).ToArray(),
            [],
            TestData.Entries,
            null,
            null,
            TestData.VersionComparer);
        
        (await unit.AuditAsync(context, CancellationToken.None)).ShouldNotBe(0);
    }
    
    [Fact]
    public async Task Has_No_Duplicate_Versions_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            [],
            TestData.Entries,
            null,
            null,
            TestData.VersionComparer);
        
        (await unit.AuditAsync(context, CancellationToken.None)).ShouldBe(0);
    }
}