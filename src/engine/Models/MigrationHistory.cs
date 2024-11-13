namespace DbForward.Models;

public record MigrationHistory(
    Guid MigrationId,
    Guid LogId,
    DateTime DateApplied,
    string Operation,
    string Agent,
    string Host)
{
    /// <inheritdoc />
    public override string ToString() => $"{MigrationId} ({Operation})";
}