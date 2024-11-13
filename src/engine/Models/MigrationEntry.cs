namespace DbForward.Models;

public record MigrationEntry(
    Guid MigrationId,
    string DbVersion,
    DateTime DateApplied,
    string SourcePath,
    string SourceFile,
    string Sha)
{
    /// <inheritdoc />
    public override string ToString() => $"{MigrationId} (version={DbVersion})";
}