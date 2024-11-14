using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Models;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace UnitTests.Services.Auditing;

public partial class TargetVersionFormatAuditStepTests
{
    private readonly TargetVersionFormatAuditStep unit = new TargetVersionFormatAuditStep(
        NullLogger<TargetVersionFormatAuditStep>.Instance);

    [Fact]
    public async Task Valid_Version_Pass()
    {
        var result = await RunScenarioAsync("v1");
        result.ShouldBe(0);
    }

    [Fact]
    public async Task Invalid_Version_Fails()
    {
        var result = await RunScenarioAsync("version1");
        result.ShouldNotBe(0);
    }

    private async Task<int> RunScenarioAsync(string version)
    {
        var source = new SourceHeader(
            "path",
            Guid.NewGuid(),
            version,
            ReadOnlyDictionary<string, string>.Empty);

        var entry = new MigrationEntry(
            Guid.Empty,
            version,
            DateTime.Now,
            "path",
            "file",
            "sha");

        var context = new AuditingContext(
            SourceOperation.Migrate,
            [source],
            [],
            [entry],
            null,
            null,
            TestData.VersionComparer);

        return await unit.AuditAsync(context, CancellationToken.None);
    }
}