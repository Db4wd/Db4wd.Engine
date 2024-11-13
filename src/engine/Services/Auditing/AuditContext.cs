using DbForward.Constants;
using DbForward.Models;

namespace DbForward.Services.Auditing;

public sealed class AuditingContext(
    SourceOperation operation,
    IList<SourceHeader> sources,
    IList<SourceHeader> sourceTargets,
    IList<MigrationEntry> appliedEntries,
    Guid? targetId,
    string? targetVersion,
    IDbVersionComparer versionComparer)
{
    private readonly Lazy<Dictionary<Guid, SourceHeader>?> lazySourceLookup = new(() => BuildSources(sources));

    public SourceOperation Operation { get; } = operation;
    
    public IList<SourceHeader> Sources { get; } = sources;
    
    public IList<SourceHeader> SourceTargets { get; } = sourceTargets;

    public IList<MigrationEntry> AppliedEntries { get; } = appliedEntries;
    
    public Guid? TargetId { get; } = targetId;
    
    public string? TargetVersion { get; } = targetVersion;

    public IDbVersionComparer VersionComparer { get; } = versionComparer;

    public Dictionary<Guid, SourceHeader>? SourceLookup => lazySourceLookup.Value;

    private static Dictionary<Guid, SourceHeader>? BuildSources(IList<SourceHeader> sourceHeaders)
    {
        try
        {
            return sourceHeaders.ToDictionary(source => source.MigrationId);
        }
        catch
        {
            return null;
        }
    }
}