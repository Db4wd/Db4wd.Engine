using DbForward.Constants;
using DbForward.Services;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class TargetIdAuditStepTests
{
    [Fact]
    public async Task Has_No_Target_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            [TestData.Sources[0]],
            TestData.Entries,
            null,
            null,
            Substitute.For<IDbVersionComparer>());

        var unit = new TargetIdAuditStep(NullLogger<TargetIdAuditStep>.Instance);
        var result = await unit.AuditAsync(context, CancellationToken.None);
        
        result.ShouldBe(0);
    }
    
    [Fact]
    public async Task Has_Target_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            [TestData.Sources[0]],
            TestData.Entries,
            TestData.Sources[0].MigrationId,
            null,
            Substitute.For<IDbVersionComparer>());

        var unit = new TargetIdAuditStep(NullLogger<TargetIdAuditStep>.Instance);
        var result = await unit.AuditAsync(context, CancellationToken.None);
        
        result.ShouldBe(0);
    }
    
    [Fact]
    public async Task Has_Missing_Target_Source_Fails()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            TestData.Sources,
            [TestData.Sources[1]],
            TestData.Entries,
            TestData.Sources[0].MigrationId,
            null,
            Substitute.For<IDbVersionComparer>());

        var unit = new TargetIdAuditStep(NullLogger<TargetIdAuditStep>.Instance);
        var result = await unit.AuditAsync(context, CancellationToken.None);
        
        result.ShouldNotBe(0);
    }
}