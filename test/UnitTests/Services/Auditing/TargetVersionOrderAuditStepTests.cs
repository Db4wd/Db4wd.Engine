using DbForward.Constants;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class TargetVersionOrderAuditStepTests
{
    private readonly TargetVersionOrderAuditStep unit = new(NullLogger<TargetVersionOrderAuditStep>.Instance);

    [Fact]
    public async Task No_Current_Version_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            TestData.Sources,
            [],
            null,
            null,
            TestData.VersionComparer);

        (await unit.AuditAsync(context, CancellationToken.None)).ShouldBe(0);
    }

    [Fact]
    public async Task Has_Version_In_Range_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            TestData.Sources.Skip(1).ToArray(),
            [TestData.Entries[0]],
            null,
            null,
            TestData.VersionComparer);

        (await unit.AuditAsync(context, CancellationToken.None)).ShouldBe(0);
    }
}