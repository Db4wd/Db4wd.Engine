using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Models;
using DbForward.Services;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class HashCodeAuditStepTests
{
    [Fact]
    public async Task Different_Hashes_Fail()
    {
        (await RunScenarioAsync("a1234567890", "b1234567890")).ShouldNotBe(0);
    }

    [Fact]
    public async Task Same_Hashes_Pass()
    {
        (await RunScenarioAsync("a1234567890", "a1234567890")).ShouldBe(0);
    }

    private static async Task<int> RunScenarioAsync(string expectedHash, string actualHash)
    {
        var id = Guid.NewGuid();
        var comparer = Substitute.For<IDbVersionComparer>();
        comparer.Compare(Arg.Any<string?>(), Arg.Any<string?>()).Returns(0);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.ComputeShaAsync(Arg.Any<string>()).Returns(Task.FromResult(actualHash));

        var context = new AuditingContext(
            SourceOperation.Migrate,
            [new SourceHeader("/path/file.sql", id, "v1", ReadOnlyDictionary<string, string>.Empty)],
            [],
            [new MigrationEntry(id, "v1", DateTime.Now, "path", "file.sql", expectedHash)],
            null,
            null,
            comparer);

        var unit = new HashCodeAuditStep(fileSystem, NullLogger<HashCodeAuditStep>.Instance);
        return await unit.AuditAsync(context, CancellationToken.None);
    }
}