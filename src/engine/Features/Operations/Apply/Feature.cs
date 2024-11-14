using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;
using DbForward.Utilities;
using Microsoft.Extensions.Logging;

namespace DbForward.Features.Operations.Apply;

public sealed class Feature(
    IDatabaseExtension extension,
    IFileSystem fileSystem,
    IAgentContext agentContext,
    ISourceFileManager sourceManager,
    ISourceAuditor sourceAuditor,
    IMigrationOperator migrationOperator,
    ILogger<Feature> logger) : OperationCoreFeature<Options>(
        SourceOperation.Migrate,
        extension,
        fileSystem,
        agentContext,
        sourceManager,
        sourceAuditor,
        migrationOperator,
        logger)
{
    /// <inheritdoc />
    protected override Dictionary<string, string> GetOptionalMetadata(Options options) =>
        new(options.Metadata);

    /// <inheritdoc />
    protected override IEnumerable<SourceHeader> GetSourceTargets(
        Options options,
        IList<SourceHeader> sources,
        HashSet<Guid> appliedEntryIds,
        IDbVersionComparer versionComparer) => sources
        .Where(source => !appliedEntryIds.Contains(source.MigrationId))
        .OrderBy(source => source.DbVersion, versionComparer)
        .TakeUntilInclusive(source => source.MigrationId == options.TargetId ||
                                      source.DbVersion == options.TargetDbVersion);
}