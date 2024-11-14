using System.Collections.ObjectModel;
using DbForward.Constants;
using DbForward.Models;
using DbForward.Services.Auditing;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace UnitTests.Services.Auditing;

public class UniqueFileNameAuditStepTests
{
    private readonly UniqueFileNameAuditStep unit = new(NullLogger<UniqueFileNameAuditStep>.Instance);

    [Fact]
    public async Task Has_Conflicting_Names_Fails()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            [
                new SourceHeader("opt/file.txt", Guid.NewGuid(), "v1", ReadOnlyDictionary<string, string>.Empty),
                new SourceHeader("var/file.txt", Guid.NewGuid(), "v1", ReadOnlyDictionary<string, string>.Empty)
            ],
            [],
            [],
            null,
            null,
            TestData.VersionComparer);
        
        (await unit.AuditAsync(context, CancellationToken.None)).ShouldNotBe(0);
    }
    
    [Fact]
    public async Task Has_Unique_Names_Passes()
    {
        var context = new AuditingContext(
            SourceOperation.Migrate,
            [
                new SourceHeader("opt/file1.txt", Guid.NewGuid(), "v1", ReadOnlyDictionary<string, string>.Empty),
                new SourceHeader("var/file2.txt", Guid.NewGuid(), "v1", ReadOnlyDictionary<string, string>.Empty)
            ],
            [],
            [],
            null,
            null,
            TestData.VersionComparer);
        
        (await unit.AuditAsync(context, CancellationToken.None)).ShouldBe(0);
    }
}