using DbForward.Constants;

namespace DbForward.Models;

public record MigrationOperationDetail(
    Guid MigrationId,
    string DbVersion,
    SourceOperation Operation,
    string SourcePath,
    string SourceFile,
    string Agent,
    string Host,
    CompressedBlobInfo? BlobInfo,
    OperationTracker? OperationTracker,
    IDictionary<string, string>? Metadata)
{
    /// <inheritdoc />
    public override string ToString() => $"{MigrationId}";
}