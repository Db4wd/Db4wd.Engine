using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Operations.Rollback;

public sealed class Feature(    
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    IAgentContext agentContext,
    ISourceFileManager sourceProvider,
    ISourceAuditor sourceAuditor,
    IMigrationOperator migrationOperator,
    ILogger<Feature> logger) : OperationCoreFeature<Options>(
        SourceOperation.Rollback,
        extension,
        fileSystem,
        agentContext,
        sourceProvider,
        sourceAuditor,
        migrationOperator,
        logger)
{
    /// <inheritdoc />
    protected override Dictionary<string, string> GetOptionalMetadata(Options options) =>
        new();

    /// <inheritdoc />
    protected override IEnumerable<SourceHeader> GetSourceTargets(
        Options options,
        IList<SourceHeader> sources,
        HashSet<Guid> appliedEntryIds,
        IDbVersionComparer versionComparer) => sources
        .Where(source => appliedEntryIds.Contains(source.MigrationId))
        .OrderByDescending(source => source.DbVersion, versionComparer)
        .TakeUntilInclusive(source => source.MigrationId == options.TargetId 
                                      || source.DbVersion == options.TargetDbVersion);
}