namespace DbForward.Models;

public record MigrationDetail(
    Guid MigrationId,
    string DbVersion,
    DateTime DateApplied,
    Guid LogId,
    string SourcePath,
    string SourceFile,
    string Sha,
    string Agent,
    string Host,
    IDictionary<string, string> Metrics,
    IDictionary<string, string> OperationTags,
    IDictionary<string, string> Metadata)
{
    /// <inheritdoc />
    public override string ToString() => $"{MigrationId} (version={DbVersion})";
}